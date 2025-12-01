# Работа с модулями Dialogue и SaveSystem

Этот документ описывает, как использовать систему диалогов и систему сохранений в проекте:
- как собирать данные через редактор Unity;
- как запускать диалоги и сохранять/загружать состояние из кода.

## Dialogue

### Настройка в редакторе
1. Откройте окно **Window → Dialogue Graph**.
2. В тулбаре выберите существующий `DialogueTree` или нажмите **New Dialogue**, чтобы создать новый `ScriptableObject`.
3. Добавьте узел через ПКМ в рабочей области или кнопку **Add Dialogue Node** в контекстном меню.
4. Выделите узел и отредактируйте текст реплики в поле **Dialogue**. Заголовок узла обновится автоматически.
5. Для каждого выбора:
   - Измените подпись в текстовом поле на порту выбора.
   - Соедините порт выбора с входом целевого узла перетаскиванием, чтобы задать `TargetNodeGuid`.
   - Удалите выбор кнопкой **X** на соответствующем порту.
   - Добавьте новый выбор кнопкой **+ Choice** под списком портов.
6. Установите стартовый узел в инспекторе справа, отметив **Start Node** для нужной вершины. Без стартового GUID запуск невозможен.
7. Расставьте узлы (перетаскивание) — позиция сохраняется в `DialogueNodeData.Position` и используется редактором.

> Совет: старайтесь избегать дублирования связей между одинаковыми узлами — редактор удаляет некорректные или циклические соединения, если они ведут к самому себе.

### Использование в коде
```csharp
using DialogueSystem.Runtime;
using UnityEngine;

public class DialogueExample : MonoBehaviour
{
    [SerializeField] private DialogueTree tree;
    private DialogueRunner runner;

    private void Start()
    {
        runner = new DialogueRunner(tree);
        if (!runner.Begin())
        {
            Debug.LogError("Диалог не может стартовать: отсутствует стартовый узел или дерево не задано.");
            return;
        }

        ShowCurrentNode();
    }

    public void OnChoiceSelected(int index)
    {
        if (runner.TryChoose(index))
        {
            ShowCurrentNode();
        }
        else
        {
            Debug.Log("Диалог завершён или выбран неверный индекс.");
        }
    }

    private void ShowCurrentNode()
    {
        var node = runner.CurrentNode;
        if (node == null)
        {
            Debug.Log("Диалог завершён.");
            return;
        }

        Debug.Log($"Реплика: {node.Text}");
        var choices = runner.GetChoices();
        for (var i = 0; i < choices.Count; i++)
        {
            Debug.Log($"{i}: {choices[i].Text}");
        }
    }
}
```
Ключевые моменты работы `DialogueRunner`:
- `Begin()` переводит исполнение на узел, идентификатор которого хранится в `DialogueTree.StartNodeGuid`.
- `GetChoices()` возвращает список вариантов текущего узла.
- `TryChoose(index)` переходит по `TargetNodeGuid` выбранного варианта; метод вернёт `false`, если переход невозможен.
- `Reset()` сбрасывает текущее состояние и останавливает диалог.

## SaveSystem

### Настройка в редакторе
1. Добавьте компонент **SaveableEntity** на объекты, которые должны сохраняться.
2. Убедитесь, что поле **Id** уникально в пределах сцены. Оно генерируется автоматически в `Reset/OnValidate`, но можно задать вручную.
3. Укажите **Prefab Id**, если объект нужно восстанавливать из префаба при отсутствии в сцене.
4. Отметьте **Persist Across Scenes**, если объект не должен сбрасываться при смене сцены.
5. Для сохранения специфичных данных добавьте на объект компоненты, реализующие `ISavePayloadProvider` (например, здоровье, инвентарь). Они будут вызваны автоматически при `CaptureState`/`Restore`.

### Использование в коде
`SaveManager` создаётся через `GlobalInstaller` (Zenject) и использует `SaveRegistry` (учёт активных `SaveableEntity`) и `SaveFileStorage` (чтение/запись JSON). Инжектируйте `SaveManager` туда, где нужно запускать операции.

#### Сохранение и загрузка
```csharp
using SaveSystem;
using UnityEngine;
using Zenject;

public class SaveController : MonoBehaviour
{
    [Inject] private SaveManager _saveManager;

    public async void QuickSave()
    {
        await _saveManager.SaveAsync(); // слот autosave
    }

    public async void QuickLoad()
    {
        var ok = await _saveManager.LoadAsync();
        Debug.Log(ok ? "Загрузка успешна" : "Слот не найден");
    }
}
```

#### Регистрация данных игрока
Реализуйте `ISaveDataSource`, чтобы дописать глобальные данные (например, статистику игрока) в `SaveDataContainer`.
```csharp
using SaveSystem;
using UnityEngine;

public class PlayerSaveDataSource : MonoBehaviour, ISaveDataSource
{
    [SerializeField] private Transform player;
    [SerializeField] private int health;
    [SerializeField] private int experience;

    public void Capture(SaveDataContainer container)
    {
        container.Player.Position = player.position;
        container.Player.Health = health;
        container.Player.Experience = experience;
    }

    public void Restore(SaveDataContainer container)
    {
        player.position = container.Player.Position;
        health = container.Player.Health;
        experience = container.Player.Experience;
    }
}
```
Добавьте компонент в сцену и зарегистрируйте его в Zenject (например, через установщик), чтобы `SaveManager` получил его в конструкторе.

#### Сохранение состояния сущности
Реализуйте `ISavePayloadProvider` на компоненте, чтобы записать дополнительные поля в `EntityStateData` конкретного `SaveableEntity`.
```csharp
using SaveSystem;
using UnityEngine;

[RequireComponent(typeof(SaveableEntity))]
public class HealthPayload : MonoBehaviour, ISavePayloadProvider
{
    [SerializeField] private int currentHealth;
    private const string HealthKey = "health";

    public void Capture(EntityStateData data)
    {
        data.SetValue(HealthKey, currentHealth.ToString());
    }

    public void Restore(EntityStateData data)
    {
        if (data.TryGetValue(HealthKey, out var value) && int.TryParse(value, out var parsed))
        {
            currentHealth = parsed;
        }
    }
}
```

### Поток данных при сохранении/загрузке
1. `SaveManager.SaveAsync` собирает глобальные данные из `ISaveDataSource`, затем опрашивает все `SaveableEntity` из `SaveRegistry`.
2. Каждый `SaveableEntity` формирует `EntityStateData` (позиция, `Id`, `PrefabId`, сцена) и вызывает все `ISavePayloadProvider` компонентов на себе.
3. `SaveFileStorage` записывает JSON в `Application.persistentDataPath/Saves/<slot>.json`.
4. При `LoadAsync` данные читаются обратно: источники `ISaveDataSource` восстанавливают глобальное состояние, после чего `SaveableEntity.Restore` применяется к найденным по `Id` объектам.

### Рекомендации
- Старайтесь генерировать **уникальные Id** на редакторском этапе (кнопка **Reset** или поле Id).
- Если объект должен появиться при загрузке, но отсутствует в сцене, используйте совпадающий **Prefab Id** и реализуйте фабрику спавна в одном из `ISaveDataSource`.
- Логируйте ошибки в `ISavePayloadProvider/ISaveDataSource`, чтобы не терять данные из-за исключений.

