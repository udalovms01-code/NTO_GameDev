using Localization;
using UnityEngine;

namespace UI.Settings
{
    public class GameSettingsStorage
    {
        private const string MasterVolumeKey = "settings.masterVolume";
        private const string MusicVolumeKey = "settings.musicVolume";
        private const string SfxVolumeKey = "settings.sfxVolume";
        private const string MouseSensitivityKey = "settings.mouseSensitivity";
        private const string FullscreenKey = "settings.fullscreen";
        private const string LanguageKey = "settings.quality";

        public GameSettings Load()
        {
            var settings = new GameSettings
            {
                MasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 0f),
                MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0f),
                SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 0f),
                MouseSensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey, 200f),
                Fullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1,
                Language = (LocalizationLanguage)PlayerPrefs.GetInt(LanguageKey, 0)
            };

            return settings;
        }

        public void Save(GameSettings settings)
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, settings.MasterVolume);
            PlayerPrefs.SetFloat(MusicVolumeKey, settings.MusicVolume);
            PlayerPrefs.SetFloat(SfxVolumeKey, settings.SfxVolume);
            PlayerPrefs.SetFloat(MouseSensitivityKey, settings.MouseSensitivity);
            PlayerPrefs.SetInt(FullscreenKey, settings.Fullscreen ? 1 : 0);
            PlayerPrefs.SetInt(LanguageKey, (int)settings.Language);
            PlayerPrefs.Save();
        }
    }
}
