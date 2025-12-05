using System;
using SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SaveManagement
{
    public class SaveSlotView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _slotNameText;
        [SerializeField] private TMP_Text _detailsText;
        [SerializeField] private Button _loadButton;
        [SerializeField] private Button _deleteButton;

        private SaveSlotInfo _info;

        public event Action<SaveSlotInfo> LoadRequested;
        public event Action<SaveSlotInfo> DeleteRequested;

        private void Awake()
        {
            if (_loadButton != null)
            {
                _loadButton.onClick.AddListener(HandleLoadClicked);
            }

            if (_deleteButton != null)
            {
                _deleteButton.onClick.AddListener(HandleDeleteClicked);
            }
        }

        private void OnDestroy()
        {
            if (_loadButton != null)
            {
                _loadButton.onClick.RemoveListener(HandleLoadClicked);
            }

            if (_deleteButton != null)
            {
                _deleteButton.onClick.RemoveListener(HandleDeleteClicked);
            }
        }

        public void Render(SaveSlotInfo info)
        {
            _info = info;
            if (_slotNameText != null)
            {
                _slotNameText.text = info.SlotName;
            }

            if (_detailsText != null)
            {
                var savedAt = info.SavedAt.ToLocalTime();
                _detailsText.text = $"{savedAt:G} | {info.SceneName}";
            }
        }

        private void HandleLoadClicked()
        {
            LoadRequested?.Invoke(_info);
        }

        private void HandleDeleteClicked()
        {
            DeleteRequested?.Invoke(_info);
        }
    }
}
