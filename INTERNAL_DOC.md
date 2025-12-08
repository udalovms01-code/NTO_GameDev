# INTERNAL_DOC

## Functions
- `LocalizationManager.Initialize(table, language)` loads settings/table (Resources/Localization) and raises `LanguageChanged`; `SetLanguage` persists choice via PlayerPrefs.
- `LocalizationManager.Get(key, params object[])` returns formatted localized string, `GetRaw`/`TryGet` provide raw access.
- `LocalizedText` updates `TMP_Text`/`Text` components when language changes and supports runtime argument injection.
- `LocalizationTable.AddEntry(key, category)`/`RemoveEntry`/`RebuildLookup` manage ScriptableObject storage for RU/EN pairs and track entry categories.
- `LocalizationTable.GetCategories()` returns unique category values from existing entries.
- `LocalizationTranslator.TranslateRuToEn` (Editor) hits Google by default or Yandex (API key via `EditorPrefs`) and is used by both the localization window and dialogue nodes.
- `LocalizationTableWindow` manages table assets, filters entries by category, edits category text per entry, seeds new entries with the active category filter, runs auto-translate, and allows quick translation snippets.
- `DialogueNodeView` now edits localization keys, previews EN text, and triggers auto-translation for nodes/choices, writing results into the default table.
- `DialogueLocalizationExtensions.GetLocalizedText` returns localized dialogue/choice text with fallbacks to raw strings.
- `SaveFileStorage.GetAvailableSaves()` собирает метаданные всех `.json` слотов из `Application.persistentDataPath/Saves` и пытается прочитать `Version`, `SavedAtTicks`, `ActiveScene`.
- `SaveManager.GetAvailableSaves()` возвращает отсортированный по дате список `SaveSlotInfo`.
- `SaveSlotsPanel.Refresh()` создаёт визуальные элементы для каждого `SaveSlotInfo`, подписывается на загрузку/удаление и отображает пустое состояние.
- `SaveSlotView.Render(info)` заполняет текстовые поля слота и пересылает события кнопок `LoadRequested`/`DeleteRequested`.
- `SaveGameButton` запускает `SaveManager.SaveAsync` по нажатию Unity-кнопки и обновляет связанную панель слотов.
- `GameSettingsPanel` инициализирует Dropdown качеств, читает настройки через `GameSettingsStorage`, применяет их через `GameSettingsApplier`, а также сохраняет или сбрасывает значения по кнопкам Apply/Reset.
- `GameSettingsApplier.Apply(settings)` настраивает `AudioListener.volume`, `Screen.fullScreen`, `QualitySettings.SetQualityLevel` и рассылает события изменения громкостей/чувствительности.
- `SoundEffectPlayer.Play(SoundCollection collection)` выбирает случайный клип с учётом громкости и разброса питча и воспроизводит через `AudioSource.PlayOneShot` (компонент гарантируется на объекте).
- `PlayerMovement.HandleFootsteps(move)` отслеживает движение/приземление `CharacterController` и с интервалом проигрывает шаги через `SoundEffectPlayer` при движении.
- `DoorInteract.ToggleDoor(Vector3 playerPos)` теперь дополнительно проигрывает звуки открытия/закрытия двери через `SoundEffectPlayer`.
- `MonsterDialoguesControl.OnChoiceSelected(int index)` перед обработкой выбранного ответа воспроизводит SFX выбора.
- `SceneTransitionController.LoadScene(int buildIndex|string sceneName)` оборачивает загрузку сцен: затемняет экран через `FadeController`, плавно глушит `AudioListener.volume`, ждёт загрузки, затем возвращает прозрачность и громкость.
- `FadeController.Awake()` обеспечивает синглтон, сбрасывает прозрачность и автоматически добавляет `SceneTransitionController` на тот же объект.
- `GameSaveController` (Game scene) при старте пытается загрузить слот `autosave` через `SaveManager`, при отсутствии создаёт его, и сохраняет состояние (в т.ч. текущий день) при каждом событии `OnDayChanged`.
- `Audio.GameStateMusicController` создаётся Zenject-инсталлером, поднимает два `AudioSource` с группой `Music` из `Resources/Music/AudioMixer` и плавно кроссфейдит темы из `Resources/Music/*` при смене `GameState` (четыре разных трека покрывают шесть состояний).
- `UI.Endings.EndingCanvasController` создаётся Zenject-инсталлером, подгружает `EndingLibrary` (Resources/Endings/EndingLibrary) или локальный список, показывает первый слайд концовки при достижении порога дня, и листает слайды по клику с затемнением/прояснением через `FadeController`.

## Types
- `LocalizationLanguage` enum with `Russian`, `English`.
- `LocalizationEntry` holds `Key`, `Category`, `Russian`, `English` fields.
- `LocalizationTable` ScriptableObject storing entry list, lookup dictionary, and exposing category enumeration.
- `LocalizationSettings` ScriptableObject with default table/language/playerPrefs key.
- `LocalizationBootstrapper` MonoBehaviour to initialize localization in scenes.
- `LocalizedText` MonoBehaviour for UI binding.
- `LocalizationTranslatorProvider` enum to select Google/Yandex for editor translations.
- Dialogue data types `DialogueNodeData` and `DialogueChoiceData` gained `LocalizationKey` for binding table entries.
- `UI.Endings.EndingDefinition` содержит `Id` и коллекцию слайдов финала, `EndingSlide` хранит текст+спрайт для отображения на экране.
- `UI.Endings.EndingLibrary` ScriptableObject со списком `EndingDefinition`, используется как внешний источник данных для экрана концовок.

## Data Flow
- At runtime `LocalizationBootstrapper` (or any manual call) initializes `LocalizationManager`, which loads settings and table from `Resources/Localization` (falls back to `LocalizationTable` asset) and sets the language from PlayerPrefs.
- UI components with `LocalizedText` subscribe to `LocalizationManager.LanguageChanged` to refresh displayed text when `SetLanguage` is invoked.
- Dialogue presentation code should call `node.GetLocalizedText()` / `choice.GetLocalizedText()` to resolve keys via `LocalizationManager` and fall back to stored raw text.
- In the editor, `LocalizationTableWindow` or dialogue node buttons request translation (Google/Yandex) and persist RU/EN pairs into the default table under the provided/generated keys.
- Музыка: `GameplayInstaller` создаёт `GameStateMusicController` на отдельном объекте, сервис подписывается на `GameStateService.OnStateChanged`, подбирает клип по состоянию (пути заданы строками ресурсов) и выполняет перекрёстное затухание между двумя `AudioSource`, чтобы плавно переключать темы.
- `GameplayInstaller` теперь создаёт `EndingCanvasController` на отдельном объекте, поэтому оверлей концовок доступен в любой игровой сцене; компонент подписывается на смену дня и использует `FadeController` для анимации переходов между слайдами.

## Notes
- Default asset locations: `Assets/Resources/Localization/LocalizationTable.asset` for table and `Resources/Localization/LocalizationSettings.asset` for settings. The table path aligns with the editor helper used by dialogue nodes.
- Auto-translation uses web requests; failures are logged but do not break the editor UI. Yandex requires an API key stored in `EditorPrefs` (`Localization.YandexApiKey`).
- Keep localization keys unique; `LocalizationTable.AddEntry` auto-appends numeric suffixes for duplicates, and dialogue node auto-generation prefixes keys with `dialogue_`/`choice_` plus GUID/index.
- `SaveSystem.SaveManager.SaveAsync(string slotName = DefaultSlot)` (Assets/Scripts/SaveSystem/SaveManager.cs): собирает данные из `ISaveDataSource`, опрашивает зарегистрированные `SaveableEntity.CaptureState`, затем сериализует контейнер через `SaveFileStorage.WriteAsync`.
- `SaveSystem.SaveManager.LoadAsync(string slotName = DefaultSlot)` (Assets/Scripts/SaveSystem/SaveManager.cs): читает контейнер из `SaveFileStorage.ReadAsync`, передаёт его в `ISaveDataSource.Restore`, затем вызывает `SaveableEntity.Restore` для найденных `Id`.
- `SaveSystem.SaveManager.Delete(string slotName)` (Assets/Scripts/SaveSystem/SaveManager.cs): удаляет сохранение через `SaveFileStorage.Delete`.
- Документ `Assets/Docs/SaveAndSettingsUI.md` описывает подключение UI панелей сохранений и базовых настроек.
- Конфигурация концовок по умолчанию ищется в `Resources/Endings/EndingLibrary.asset`; если список пуст, `EndingCanvasController` создаёт базовый слайд с текстом-заглушкой, который можно заменить через ScriptableObject.
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
- В сцене Game `GameSaveController` инициирует автоматическую загрузку слота `autosave` и подписывается на смену дня, чтобы вызывать `SaveManager.SaveAsync`, сохраняя актуальный день в `SaveDataContainer`.
- Список сохранений: `SaveSlotsPanel` использует `SaveManager.GetAvailableSaves()`, визуализирует каждый слот префабом `SaveSlotView`, а действия загрузки/удаления делегирует обратно в `SaveManager` с последующим обновлением панели.
- Настройки: `GameSettingsPanel` загружает модель через `GameSettingsStorage` при `Awake`, применяет её через `GameSettingsApplier` (мастер-громкость, полноэкранность, качество) и сохраняет/сбрасывает значения при нажатии соответствующих кнопок.
- Музыка: `GameplayInstaller` добавляет `GameStateMusicController`, который реагирует на смены `GameState`, подбирает клипы из `Resources/Music` и кроссфейдит их между двумя `AudioSource` для плавных переходов.

## Notes
- Добавлен пользовательский документ `Docs/Dialogue_SaveSystem_Usage.md` с инструкциями по работе модулей в редакторе и из кода.
- При добавлении новых `ISaveDataSource` нужно регистрировать их в Zenject, чтобы `SaveManager` получил список источников в конструкторе.
- Музыкальные клипы и микшер для состояний перенесены в `Assets/Resources/Music`, чтобы их можно было грузить через `Resources.Load` без правок сцен.
- Диалоговые деревья в `Assets/ScriptableObjects/Dialogues`: `Day1` охватывает стадии отрицания и сделки с ветками «тишина» → «никого не жду» (прерывает диалог), «тишина» → «жду забвения» → обязательный вопрос о намерениях монстра и выбор капитуляции/сделки; `Day2` раскрывает монстра с руками, предлагая ответ «ты не посмеешь войти» (односторонняя реплика) и ветку поиска личности с ответами «Там никого нет» или просьбой оставить прошлое в покое; `Day3` концентрируется на чувстве вины с ответвлениями «Тебя это не касается» и признанием «Я поступаю неверно», ведущим к финальному предупреждению о превращении героя в часть меню.
- Скриптабл-ассеты `Day4.asset`, `Day5.asset`, `Day6.asset`, `Day7.asset` (Assets/ScriptableObjects/Dialogues) описывают диалоговые деревья с ветками страха/рационализации, ассимиляции монстра, финальными решениями по сердцу и положительной концовкой.
