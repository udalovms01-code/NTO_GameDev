using System;
using Localization;
using UnityEngine;

namespace UI.Settings
{
    public class GameSettingsApplier
    {
        public event Action<float> MusicVolumeChanged;
        public event Action<float> SfxVolumeChanged;
        public event Action<float> MouseSensitivityChanged;

        public void Apply(GameSettings settings)
        {
            AudioListener.volume = settings.MasterVolume;
            Debug.Log(settings.Language);
            LocalizationManager.SetLanguage(settings.Language);
            Screen.fullScreen = settings.Fullscreen;

            MusicVolumeChanged?.Invoke(settings.MusicVolume);
            SfxVolumeChanged?.Invoke(settings.SfxVolume);
            MouseSensitivityChanged?.Invoke(settings.MouseSensitivity);
        }
    }
}
