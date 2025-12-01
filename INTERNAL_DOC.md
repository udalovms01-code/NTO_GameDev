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
