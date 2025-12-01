using UnityEngine;

namespace Localization
{
    public class LocalizationBootstrapper : MonoBehaviour
    {
        [SerializeField]
        private LocalizationTable table;

        [SerializeField]
        private LocalizationLanguage startLanguage = LocalizationLanguage.Russian;

        private void Awake()
        {
            LocalizationManager.Initialize(table, startLanguage);
        }
    }
}
