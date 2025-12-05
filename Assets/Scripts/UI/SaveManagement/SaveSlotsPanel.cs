using System.Collections.Generic;
using SaveSystem;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI.SaveManagement
{
    public class SaveSlotsPanel : MonoBehaviour
    {
        [SerializeField] private SaveSlotView _slotPrefab;
        [SerializeField] private Transform _container;
        [SerializeField] private GameObject _emptyState;
        [SerializeField] private TMP_Text _errorText;

        private readonly List<SaveSlotView> _spawnedViews = new List<SaveSlotView>();

        private SaveManager _saveManager;

        [Inject]
        public void Construct(SaveManager saveManager)
        {
            _saveManager = saveManager;
        }

        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            ClearItems();
            _errorText?.SetText(string.Empty);

            if (_saveManager == null || _slotPrefab == null || _container == null)
            {
                if (_errorText != null)
                {
                    _errorText.text = "Save panel is not configured";
                }

                return;
            }

            var saves = _saveManager.GetAvailableSaves();
            foreach (var save in saves)
            {
                var view = Instantiate(_slotPrefab, _container);
                view.Render(save);
                view.LoadRequested += HandleLoadRequested;
                view.DeleteRequested += HandleDeleteRequested;
                _spawnedViews.Add(view);
            }

            if (_emptyState != null)
            {
                _emptyState.SetActive(saves.Count == 0);
            }
        }

        private async void HandleLoadRequested(SaveSlotInfo info)
        {
            if (_saveManager == null)
            {
                return;
            }

            var loaded = await _saveManager.LoadAsync(info.SlotName);
            if (!loaded && _errorText != null)
            {
                _errorText.text = $"Не удалось загрузить слот {info.SlotName}";
            }
        }

        private void HandleDeleteRequested(SaveSlotInfo info)
        {
            if (_saveManager == null)
            {
                return;
            }

            var deleted = _saveManager.Delete(info.SlotName);
            if (!deleted && _errorText != null)
            {
                _errorText.text = $"Не найден слот {info.SlotName}";
            }

            Refresh();
        }

        private void ClearItems()
        {
            foreach (var view in _spawnedViews)
            {
                if (view == null)
                {
                    continue;
                }

                view.LoadRequested -= HandleLoadRequested;
                view.DeleteRequested -= HandleDeleteRequested;
                Destroy(view.gameObject);
            }

            _spawnedViews.Clear();
        }
    }
}
