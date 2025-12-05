# INTERNAL_DOC

## Functions
- `LocalizationManager.Initialize(table, language)` loads settings/table (Resources/Localization) and raises `LanguageChanged`; `SetLanguage` persists choice via PlayerPrefs.
- `LocalizationManager.Get(key, params object[])` returns formatted localized string, `GetRaw`/`TryGet` provide raw access.
- `LocalizedText` updates `TMP_Text`/`Text` components when language changes and supports runtime argument injection.
- `LocalizationTable.AddEntry/RemoveEntry/RebuildLookup` manage ScriptableObject storage for RU/EN pairs.
- `LocalizationTranslator.TranslateRuToEn` (Editor) hits Google by default or Yandex (API key via `EditorPrefs`) and is used by both the localization window and dialogue nodes.
- `LocalizationTableWindow` manages table assets, runs auto-translate, and allows quick translation snippets.
- `DialogueNodeView` now edits localization keys, previews EN text, and triggers auto-translation for nodes/choices, writing results into the default table.
- `DialogueLocalizationExtensions.GetLocalizedText` returns localized dialogue/choice text with fallbacks to raw strings.
- `SaveFileStorage.GetAvailableSaves()` собирает метаданные всех `.json` слотов из `Application.persistentDataPath/Saves` и пытается прочитать `Version`, `SavedAtTicks`, `ActiveScene`.
- `SaveManager.GetAvailableSaves()` возвращает отсортированный по дате список `SaveSlotInfo`.
- `SaveSlotsPanel.Refresh()` создаёт визуальные элементы для каждого `SaveSlotInfo`, подписывается на загрузку/удаление и отображает пустое состояние.
- `SaveSlotView.Render(info)` заполняет текстовые поля слота и пересылает события кнопок `LoadRequested`/`DeleteRequested`.
- `SaveGameButton` запускает `SaveManager.SaveAsync` по нажатию Unity-кнопки и обновляет связанную панель слотов.
- `GameSettingsPanel` инициализирует Dropdown качеств, читает настройки через `GameSettingsStorage`, применяет их через `GameSettingsApplier`, а также сохраняет или сбрасывает значения по кнопкам Apply/Reset.
- `GameSettingsApplier.Apply(settings)` настраивает `AudioListener.volume`, `Screen.fullScreen`, `QualitySettings.SetQualityLevel` и рассылает события изменения громкостей/чувствительности.

## Types
- `LocalizationLanguage` enum with `Russian`, `English`.
- `LocalizationEntry` holds `Key`, `Russian`, `English` fields.
- `LocalizationTable` ScriptableObject storing entry list and lookup dictionary.
- `LocalizationSettings` ScriptableObject with default table/language/playerPrefs key.
- `LocalizationBootstrapper` MonoBehaviour to initialize localization in scenes.
- `LocalizedText` MonoBehaviour for UI binding.
- `LocalizationTranslatorProvider` enum to select Google/Yandex for editor translations.
- Dialogue data types `DialogueNodeData` and `DialogueChoiceData` gained `LocalizationKey` for binding table entries.

## Data Flow
- At runtime `LocalizationBootstrapper` (or any manual call) initializes `LocalizationManager`, which loads settings and table from `Resources/Localization` (falls back to `LocalizationTable` asset) and sets the language from PlayerPrefs.
- UI components with `LocalizedText` subscribe to `LocalizationManager.LanguageChanged` to refresh displayed text when `SetLanguage` is invoked.
- Dialogue presentation code should call `node.GetLocalizedText()` / `choice.GetLocalizedText()` to resolve keys via `LocalizationManager` and fall back to stored raw text.
- In the editor, `LocalizationTableWindow` or dialogue node buttons request translation (Google/Yandex) and persist RU/EN pairs into the default table under the provided/generated keys.

## Notes
- Default asset locations: `Assets/Resources/Localization/LocalizationTable.asset` for table and `Resources/Localization/LocalizationSettings.asset` for settings. The table path aligns with the editor helper used by dialogue nodes.
- Auto-translation uses web requests; failures are logged but do not break the editor UI. Yandex requires an API key stored in `EditorPrefs` (`Localization.YandexApiKey`).
- Keep localization keys unique; `LocalizationTable.AddEntry` auto-appends numeric suffixes for duplicates, and dialogue node auto-generation prefixes keys with `dialogue_`/`choice_` plus GUID/index.
- `SaveSystem.SaveManager.SaveAsync(string slotName = DefaultSlot)` (Assets/Scripts/SaveSystem/SaveManager.cs): собирает данные из `ISaveDataSource`, опрашивает зарегистрированные `SaveableEntity.CaptureState`, затем сериализует контейнер через `SaveFileStorage.WriteAsync`.
- `SaveSystem.SaveManager.LoadAsync(string slotName = DefaultSlot)` (Assets/Scripts/SaveSystem/SaveManager.cs): читает контейнер из `SaveFileStorage.ReadAsync`, передаёт его в `ISaveDataSource.Restore`, затем вызывает `SaveableEntity.Restore` для найденных `Id`.
- `SaveSystem.SaveManager.Delete(string slotName)` (Assets/Scripts/SaveSystem/SaveManager.cs): удаляет сохранение через `SaveFileStorage.Delete`.
- Документ `Assets/Docs/SaveAndSettingsUI.md` описывает подключение UI панелей сохранений и базовых настроек.
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
- `SaveSystem.SaveSlotInfo`: метаданные слота сохранения (имя, версия, время сохранения, сцена) с геттером `SavedAt` для `DateTime`.
- `UI.Settings.GameSettings`: сериализуемая модель базовых настроек (громкости, полноэкранность, качество, чувствительность мыши).
- `UI.Settings.GameSettingsStorage`: слой сохранения/загрузки `GameSettings` в `PlayerPrefs`.
- `UI.Settings.GameSettingsApplier`: применяет настройки и генерирует события для подписчиков аудио/инпут систем.
- `UI.Settings.GameSettingsPanel`: MonoBehaviour, связывающий UI-контролы с моделью/хранилищем/апплаером настроек.
- `UI.SaveManagement.SaveSlotsPanel`, `SaveSlotView`, `SaveGameButton`: компоненты UI для списка, визуализации и создания сохранений.

## Data Flow
- Редактор диалогов: `DialogueGraphWindow` загружает/создаёт `DialogueTree`, `DialogueGraphView` строит визуальные узлы (`DialogueNodeView`), изменения портов записываются обратно в `DialogueTree` и помечаются `EditorUtility.SetDirty` для сохранения.
- Исполнение диалога: потребитель создаёт `DialogueRunner` с `DialogueTree`, вызывает `Begin()`, затем опрашивает `CurrentNode` и `GetChoices`; `TryChoose` устанавливает следующий узел по `TargetNodeGuid`.
- Сохранение: `SaveManager.SaveAsync` формирует `SaveDataContainer` с версией, сценой и временем, вызывает `ISaveDataSource.Capture`, затем `SaveableEntity.CaptureState` (включая `ISavePayloadProvider`) и записывает JSON через `SaveFileStorage`.
- Загрузка: `SaveManager.LoadAsync` читает JSON, вызывает `ISaveDataSource.Restore`, затем ищет сущности по `Id` в `SaveRegistry` и применяет `SaveableEntity.Restore`; отсутствующие объекты отмечаются предупреждением с `PrefabId`.
- Список сохранений: `SaveSlotsPanel` использует `SaveManager.GetAvailableSaves()`, визуализирует каждый слот префабом `SaveSlotView`, а действия загрузки/удаления делегирует обратно в `SaveManager` с последующим обновлением панели.
- Настройки: `GameSettingsPanel` загружает модель через `GameSettingsStorage` при `Awake`, применяет её через `GameSettingsApplier` (мастер-громкость, полноэкранность, качество) и сохраняет/сбрасывает значения при нажатии соответствующих кнопок.

## Notes
- Добавлен пользовательский документ `Docs/Dialogue_SaveSystem_Usage.md` с инструкциями по работе модулей в редакторе и из кода.
- При добавлении новых `ISaveDataSource` нужно регистрировать их в Zenject, чтобы `SaveManager` получил список источников в конструкторе.
