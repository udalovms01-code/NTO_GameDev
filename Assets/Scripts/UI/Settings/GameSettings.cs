using System;
using Localization;
using UnityEngine.Serialization;

namespace UI.Settings
{
    [Serializable]
    public class GameSettings
    {
        public float MasterVolume = 1f;
        public float MusicVolume = 1f;
        public float SfxVolume = 1f;
        public float MouseSensitivity = 1f;
        public bool Fullscreen = true;
        public LocalizationLanguage Language = LocalizationLanguage.Russian;
    }
}
