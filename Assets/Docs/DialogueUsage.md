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
