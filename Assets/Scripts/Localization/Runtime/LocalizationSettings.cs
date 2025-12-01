using UnityEngine;

namespace Localization
{
    [CreateAssetMenu(fileName = "LocalizationSettings", menuName = "Localization/Settings")]
    public class LocalizationSettings : ScriptableObject
    {
        [SerializeField]
        private LocalizationTable defaultTable;

        [SerializeField]
        private LocalizationLanguage defaultLanguage = LocalizationLanguage.Russian;

        [SerializeField]
        private string playerPrefsKey = "Localization.Language";

        public LocalizationTable DefaultTable => defaultTable;

        public LocalizationLanguage DefaultLanguage => defaultLanguage;

        public string PlayerPrefsKey => playerPrefsKey;
    }
}
