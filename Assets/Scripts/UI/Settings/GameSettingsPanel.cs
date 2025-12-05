using System.Linq;
using Localization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Settings
{
    public class GameSettingsPanel : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private Slider _masterVolume;
        [SerializeField] private Slider _musicVolume;
        [SerializeField] private Slider _sfxVolume;

        [Header("Controls")]
        [SerializeField] private Slider _mouseSensitivity;

        [Header("Video")]
        [SerializeField] private Toggle _fullscreenToggle;
        [SerializeField] private TMP_Dropdown _languageDropdown;

        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _resetButton;

        private readonly GameSettingsStorage _storage = new GameSettingsStorage();
        private readonly GameSettingsApplier _applier = new GameSettingsApplier();
        private GameSettings _settings;

        public GameSettingsApplier Applier => _applier;

        private void Awake()
        {
            _settings = _storage.Load();
            InitializeQualityDropdown();
            SyncUIFromSettings();
            _applier.Apply(_settings);
            HookButtons();
            
            LocalizationManager.LanguageChanged += (value) =>
                InitializeQualityDropdown();
        }

        private void OnDestroy()
        {
            UnhookButtons();
        }

        private void HookButtons()
        {
            if (_applyButton != null)
            {
                _applyButton.onClick.AddListener(ApplyChanges);
            }

            if (_resetButton != null)
            {
                _resetButton.onClick.AddListener(ResetToDefaults);
            }
        }

        private void UnhookButtons()
        {
            if (_applyButton != null)
            {
                _applyButton.onClick.RemoveListener(ApplyChanges);
            }

            if (_resetButton != null)
            {
                _resetButton.onClick.RemoveListener(ResetToDefaults);
            }
        }

        private void ApplyChanges()
        {
            if (_settings == null)
            {
                _settings = new GameSettings();
            }
            
            _settings.MasterVolume = _masterVolume != null ? _masterVolume.value : 1f;
            _settings.MusicVolume = _musicVolume != null ? _musicVolume.value : 1f;
            _settings.SfxVolume = _sfxVolume != null ? _sfxVolume.value : 1f;
            _settings.MouseSensitivity = _mouseSensitivity != null ? _mouseSensitivity.value : 1f;
            _settings.Fullscreen = _fullscreenToggle == null || _fullscreenToggle.isOn;
            _settings.Language = _languageDropdown != null ? (LocalizationLanguage)_languageDropdown.value : LocalizationLanguage.Russian;
            
            _storage.Save(_settings);
            _applier.Apply(_settings);
        }

        private void ResetToDefaults()
        {
            _settings = new GameSettings();
            SyncUIFromSettings();
            ApplyChanges();
        }

        private void SyncUIFromSettings()
        {
            if (_masterVolume != null)
            {
                _masterVolume.SetValueWithoutNotify(_settings.MasterVolume);
            }

            if (_musicVolume != null)
            {
                _musicVolume.SetValueWithoutNotify(_settings.MusicVolume);
            }

            if (_sfxVolume != null)
            {
                _sfxVolume.SetValueWithoutNotify(_settings.SfxVolume);
            }

            if (_mouseSensitivity != null)
            {
                _mouseSensitivity.SetValueWithoutNotify(_settings.MouseSensitivity);
            }

            if (_fullscreenToggle != null)
            {
                _fullscreenToggle.SetIsOnWithoutNotify(_settings.Fullscreen);
            }

            if (_languageDropdown != null)
            {
                _languageDropdown.SetValueWithoutNotify((int)_settings.Language);
            }
        }

        private void InitializeQualityDropdown()
        {
            if (_languageDropdown == null)
            {
                return;
            }
            _languageDropdown.ClearOptions();
            _languageDropdown.AddOptions(new[] { LocalizationManager.Get("rus"), LocalizationManager.Get("eng") }.ToList());
            _languageDropdown.SetValueWithoutNotify((int)_settings.Language);
        }
    }
}
