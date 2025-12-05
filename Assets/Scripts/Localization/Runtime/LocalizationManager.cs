using System;
using UnityEngine;

namespace Localization
{
    public static class LocalizationManager
    {
        public static event Action<LocalizationLanguage> LanguageChanged;

        private static LocalizationTable activeTable;
        private static LocalizationLanguage currentLanguage = LocalizationLanguage.Russian;
        private static LocalizationSettings settings;
        private static bool initialized;

        public static LocalizationLanguage CurrentLanguage => currentLanguage;

        public static LocalizationTable ActiveTable => activeTable;

        public static void Initialize(LocalizationTable table = null, LocalizationLanguage? language = null)
        {
            if (initialized)
            {
                if (table != null && table != activeTable)
                {
                    activeTable = table;
                }

                return;
            }

            initialized = true;
            settings = Resources.Load<LocalizationSettings>("Localization/LocalizationSettings");
            activeTable = table != null ? table : settings?.DefaultTable ?? Resources.Load<LocalizationTable>("Localization/LocalizationTable");
            currentLanguage = language ?? LoadLanguageFromPrefs(settings?.PlayerPrefsKey);

            LanguageChanged?.Invoke(currentLanguage);
        }

        public static void SetLanguage(LocalizationLanguage language, bool persist = true)
        {
            Initialize();

            if (currentLanguage == language)
            {
                return;
            }

            currentLanguage = language;

            if (persist && settings != null && !string.IsNullOrEmpty(settings.PlayerPrefsKey))
            {
                PlayerPrefs.SetInt(settings.PlayerPrefsKey, (int)currentLanguage);
                PlayerPrefs.Save();
            }

            LanguageChanged?.Invoke(currentLanguage);
        }

        public static string Get(string key, params object[] args)
        {
            var value = GetRaw(key);
            return args == null || args.Length == 0 ? value : string.Format(value, args);
        }

        public static string GetRaw(string key, string fallback = "")
        {
            Initialize();

            if (string.IsNullOrEmpty(key))
            {
                return fallback;
            }

            if (activeTable == null)
            {
                return fallback;
            }

            var value = activeTable.GetValue(key, currentLanguage, fallback);
            return string.IsNullOrEmpty(value) ? fallback : value;
        }

        public static bool TryGet(string key, out string value)
        {
            value = GetRaw(key);
            return !string.IsNullOrEmpty(value);
        }

        private static LocalizationLanguage LoadLanguageFromPrefs(string prefsKey)
        {
            if (string.IsNullOrEmpty(prefsKey))
            {
                return LocalizationLanguage.Russian;
            }

            var stored = PlayerPrefs.GetInt(prefsKey, -1);
            return stored < 0 ? LocalizationLanguage.Russian : (LocalizationLanguage)stored;
        }
    }
}
