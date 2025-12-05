# UI: Сохранения и базовые настройки

## Панель сохранений
- **SaveSlotsPanel** (Scripts/UI/SaveManagement/SaveSlotsPanel.cs): строит список существующих слотов через `SaveManager.GetAvailableSaves()`. Требует префаб **SaveSlotView** и контейнер для размещения элементов. Обновляет пустое состояние `_emptyState`, показывает ошибки в `_errorText`.
- **SaveSlotView** (Scripts/UI/SaveManagement/SaveSlotView.cs): визуализация одного слота (имя, дата, сцена) с кнопками загрузки/удаления. Генерирует события `LoadRequested` и `DeleteRequested`.
- **SaveGameButton** (Scripts/UI/SaveManagement/SaveGameButton.cs): запускает `SaveManager.SaveAsync` для слота из поля ввода или использует `SaveManager.DefaultSlot`. По завершении может обновить панель слотов.

**Подключение**: разместите `SaveSlotsPanel` на Canvas, назначьте префаб `SaveSlotView`, контейнер, текст ошибок и (опционально) объект пустого состояния. Кнопку сохранения повесьте на `SaveGameButton`, передайте ссылку на поле ввода слота и на ту же панель для автообновления списка. Zenject инжектирует `SaveManager` через `GlobalInstaller`.

## Базовые настройки
- **GameSettingsPanel** (Scripts/UI/Settings/GameSettingsPanel.cs): связывает слайдеры/кнопки для звука, видео и управления. Поддерживает применение изменений (`_applyButton`) и сброс к дефолту (`_resetButton`). При старте загружает значения из `PlayerPrefs` и сразу применяет.
- **GameSettingsStorage** (Scripts/UI/Settings/GameSettingsStorage.cs): сохраняет/загружает `GameSettings` (громкости, полноэкранный режим, качество, чувствительность мыши) в `PlayerPrefs`.
- **GameSettingsApplier** (Scripts/UI/Settings/GameSettingsApplier.cs): применяет мастер-громкость (`AudioListener.volume`), качество (`QualitySettings.SetQualityLevel`), полноэкранный режим (`Screen.fullScreen`) и рассылает события изменения громкостей и чувствительности для привязки аудио/инпут систем.
- **GameSettings** (Scripts/UI/Settings/GameSettings.cs): сериализуемая модель значений настроек.

**Подключение**: разместите `GameSettingsPanel` на экране настроек, назначьте UI-элементы (Slider/Toggle/Dropdown/Button). При необходимости подпишитесь на события `GameSettingsPanel.Applier` (например, для перенастройки микшера или инпут системы) после инициализации компонента.
