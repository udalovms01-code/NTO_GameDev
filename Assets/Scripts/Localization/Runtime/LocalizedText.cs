using Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Localization
{
    [DisallowMultipleComponent]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField]
        private string key = string.Empty;

        [SerializeField]
        [Tooltip("Optional formatting arguments that will be injected into the localized string at runtime.")]
        private string[] formatArguments = new string[0];

        [SerializeField]
        private bool initializeManager = true;

        private TMP_Text tmpText;
        private Text uiText;

        private void Awake()
        {
            tmpText = GetComponent<TMP_Text>();
            uiText = GetComponent<Text>();

            if (initializeManager)
            {
                LocalizationManager.Initialize();
            }
        }

        private void OnEnable()
        {
            LocalizationManager.LanguageChanged += OnLanguageChanged;
            Refresh();
        }

        private void OnDisable()
        {
            LocalizationManager.LanguageChanged -= OnLanguageChanged;
        }

        public void SetArguments(params string[] args)
        {
            formatArguments = args;
            Refresh();
        }

        public void SetKey(string newKey)
        {
            key = newKey;
            Refresh();
        }

        private void OnLanguageChanged(LocalizationLanguage _)
        {
            Refresh();
        }

        private void Refresh()
        {
            var value = LocalizationManager.Get(key, formatArguments);

            if (tmpText != null)
            {
                tmpText.text = value;
            }

            if (uiText != null)
            {
                uiText.text = value;
            }
        }
    }
}
