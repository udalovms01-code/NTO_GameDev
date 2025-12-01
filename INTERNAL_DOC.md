# INTERNAL_DOC

## Functions
- `SaveSystem.SaveManager.SaveAsync(string slotName = DefaultSlot)` (Assets/Scripts/SaveSystem/SaveManager.cs): собирает данные из `ISaveDataSource`, опрашивает зарегистрированные `SaveableEntity.CaptureState`, затем сериализует контейнер через `SaveFileStorage.WriteAsync`.
- `SaveSystem.SaveManager.LoadAsync(string slotName = DefaultSlot)` (Assets/Scripts/SaveSystem/SaveManager.cs): читает контейнер из `SaveFileStorage.ReadAsync`, передаёт его в `ISaveDataSource.Restore`, затем вызывает `SaveableEntity.Restore` для найденных `Id`.
- `SaveSystem.SaveManager.Delete(string slotName)` (Assets/Scripts/SaveSystem/SaveManager.cs): удаляет сохранение через `SaveFileStorage.Delete`.
- `DialogueSystem.Runtime.DialogueRunner.Begin()` (Assets/Scripts/Dialogues/Runtime/DialogueRunner.cs): переходит на стартовый узел `DialogueTree.StartNodeGuid`, возвращает `false`, если дерево или узел отсутствуют.
- `DialogueSystem.Runtime.DialogueRunner.TryChoose(int index)` (Assets/Scripts/Dialogues/Runtime/DialogueRunner.cs): переходит к целевому узлу по выбору, сбрасывает текущее состояние при ошибке и возвращает `false`.
- `DialogueSystem.Runtime.DialogueRunner.GetChoices()`/`CurrentNode`/`Reset()` (Assets/Scripts/Dialogues/Runtime/DialogueRunner.cs): читают варианты текущего узла и сбрасывают состояние.

## Types
- `DialogueSystem.Runtime.DialogueTree` (ScriptableObject): хранит коллекцию `DialogueNodeData` и `StartNodeGuid`; методы `GetNode`, `AddNode`, `RemoveNode` управляют списком и связями.
- `DialogueSystem.Runtime.DialogueNodeData` и `DialogueChoiceData`: сериализуемые данные узлов (текст, позиция) и вариантов (текст, `TargetNodeGuid`).
- `DialogueSystem.Editor.DialogueGraphWindow`/`DialogueGraphView`/`DialogueNodeView`: редакторские классы, создающие/редактирующие `DialogueTree`, обеспечивают визуальные узлы и подключение портов.
- `SaveSystem.SaveableEntity`: компонент с полями `Id`, `PrefabId`, `PersistAcrossScenes`, регистрируется в `SaveRegistry` и собирает данные через `ISavePayloadProvider`.
- `SaveSystem.ISavePayloadProvider`: интерфейс для добавления данных в `EntityStateData` и восстановления их для конкретной сущности.
- `SaveSystem.SaveRegistry`: реестр активных `SaveableEntity` с методами `Register`, `Unregister`, `Find`.
- `SaveSystem.SaveFileStorage`: файловое хранилище JSON в `Application.persistentDataPath/Saves`, методы `WriteAsync`, `ReadAsync`, `Delete`.
- `SaveSystem.ISaveDataSource`: интерфейс для глобальных сохранённых данных (например, состояние игрока) в `SaveDataContainer`.
- `SaveSystem.SaveDataContainer`, `PlayerStateData`, `EntityStateData`, `SerializableKeyValuePair`: сериализуемые контейнеры для сохранений.
- `GlobalInstaller` (Assets/Scripts/GlobalInstaller.cs): Zenject-установщик, создающий синглтоны `SaveRegistry`, `SaveFileStorage`, `SaveManager`.

## Data Flow
- Редактор диалогов: `DialogueGraphWindow` загружает/создаёт `DialogueTree`, `DialogueGraphView` строит визуальные узлы (`DialogueNodeView`), изменения портов записываются обратно в `DialogueTree` и помечаются `EditorUtility.SetDirty` для сохранения.
- Исполнение диалога: потребитель создаёт `DialogueRunner` с `DialogueTree`, вызывает `Begin()`, затем опрашивает `CurrentNode` и `GetChoices`; `TryChoose` устанавливает следующий узел по `TargetNodeGuid`.
- Сохранение: `SaveManager.SaveAsync` формирует `SaveDataContainer` с версией, сценой и временем, вызывает `ISaveDataSource.Capture`, затем `SaveableEntity.CaptureState` (включая `ISavePayloadProvider`) и записывает JSON через `SaveFileStorage`.
- Загрузка: `SaveManager.LoadAsync` читает JSON, вызывает `ISaveDataSource.Restore`, затем ищет сущности по `Id` в `SaveRegistry` и применяет `SaveableEntity.Restore`; отсутствующие объекты отмечаются предупреждением с `PrefabId`.

## Notes
- Добавлен пользовательский документ `Docs/Dialogue_SaveSystem_Usage.md` с инструкциями по работе модулей в редакторе и из кода.
- При добавлении новых `ISaveDataSource` нужно регистрировать их в Zenject, чтобы `SaveManager` получил список источников в конструкторе.
