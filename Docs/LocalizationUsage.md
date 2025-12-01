# Модуль локализации (RU/EN)

## Возможности
- Хранение строк в `LocalizationTable` (ScriptableObject) с ключами и парами переводов RU/EN.
- Переключение языка на лету через `LocalizationManager` с оповещением подписчиков.
- Поддержка форматирования `string.Format` через `LocalizedText` и прямые вызовы `LocalizationManager.Get`.
- Совместимость с диалоговой системой: узлы и выборы содержат `LocalizationKey`, есть автоперевод и привязка таблицы из окна графа.
- Автоперевод из редактора через Google Translate (без ключа) или Yandex (нужен API ключ).

## Настройка данных
1. Откройте окно **Window ▸ Localization ▸ Table**.
2. Нажмите **Create/Use Default Table**, чтобы создать `Assets/Resources/Localization/LocalizationTable.asset`.
3. Добавляйте или редактируйте записи. Кнопка **Auto translate from RU** заполнит английское поле на основе русского текста.
4. Для использования Yandex переводчика укажите API-ключ в поле **Yandex API Key**. Без ключа используется Google.
5. (Опционально) создайте `LocalizationSettings` через **Create ▸ Localization ▸ Settings** и укажите таблицу/язык по умолчанию. Файл должен лежать в `Resources/Localization`.

## Использование в сцене
- Добавьте компонент `LocalizationBootstrapper` на объект сцены и задайте таблицу и стартовый язык. Он вызывает `LocalizationManager.Initialize(...)` на `Awake`.
- Для UI-текста добавьте компонент `LocalizedText` к `TextMeshProUGUI` или `Text` и укажите `key`. При смене языка текст обновится автоматически.
- Смена языка из кода: `LocalizationManager.SetLanguage(LocalizationLanguage.English);`.
- Получение строки вручную: `var title = LocalizationManager.Get("ui.title", playerName);` где `playerName` попадёт в `{0}`.

## Локализация диалогов
- Каждый узел (`DialogueNodeData`) и выбор (`DialogueChoiceData`) теперь содержит `LocalizationKey`.
- В окне графа диалога:
  - Поле **Localization Key** задаёт ключ строки (генерируется автоматически при автопереводе).
  - Кнопка **Auto translate to EN** создаёт/обновляет запись в таблице, переводит русский текст узла на английский и выводит предпросмотр.
  - Для каждого выбора есть поле `Key` и кнопка **Auto EN** с таким же поведением.
- На экране показ диалоговых текстов следует получать строки через расширения: `node.GetLocalizedText()` и `choice.GetLocalizedText()`. При отсутствии ключа используется оригинальный текст.

## Рекомендации
- Храните длину строк в пределах UI-элементов и проверяйте шрифты для кириллицы/латиницы.
- Проверяйте, что все ключи уникальны; окно таблицы автоматически добавляет суффиксы при дублировании.
- Добавляйте обработку смены языка в UI, подписываясь на `LocalizationManager.LanguageChanged`.
