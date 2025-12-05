using SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.SaveManagement
{
    public class SaveGameButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_InputField _slotNameInput;
        [SerializeField] private string _fallbackSlotName = SaveManager.DefaultSlot;
        [SerializeField] private SaveSlotsPanel _slotsPanel;

        private SaveManager _saveManager;

        [Inject]
        public void Construct(SaveManager saveManager)
        {
            _saveManager = saveManager;
        }

        private void Awake()
        {
            if (_button != null)
            {
                _button.onClick.AddListener(HandleSaveClicked);
            }
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(HandleSaveClicked);
            }
        }

        private async void HandleSaveClicked()
        {
            if (_saveManager == null || _button == null)
            {
                return;
            }

            _button.interactable = false;

            var slotName = GetSlotName();
            await _saveManager.SaveAsync(slotName);

            if (_slotsPanel != null)
            {
                _slotsPanel.Refresh();
            }

            _button.interactable = true;
        }

        private string GetSlotName()
        {
            var text = _slotNameInput != null ? _slotNameInput.text : string.Empty;
            return string.IsNullOrWhiteSpace(text) ? _fallbackSlotName : text.Trim();
        }
    }
}
