using System;
using UnityEngine;

namespace Localization
{
    [Serializable]
    public class LocalizationEntry
    {
        [SerializeField]
        private string key = string.Empty;

        [SerializeField]
        [TextArea]
        private string russian = string.Empty;

        [SerializeField]
        [TextArea]
        private string english = string.Empty;

        public string Key
        {
            get => key;
            set => key = value;
        }

        public string Russian
        {
            get => russian;
            set => russian = value;
        }

        public string English
        {
            get => english;
            set => english = value;
        }

        public string GetValue(LocalizationLanguage language)
        {
            return language == LocalizationLanguage.Russian ? russian : english;
        }

        public void SetValue(LocalizationLanguage language, string value)
        {
            if (language == LocalizationLanguage.Russian)
            {
                russian = value;
                return;
            }

            english = value;
        }
    }
}
