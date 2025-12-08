using SaveSystem;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class GameSaveController : MonoBehaviour
    {
        [SerializeField] private string _slotName = SaveManager.DefaultSlot;

        private SaveManager _saveManager;
        private GameStateService _gameStateService;

        [Inject]
        public void Construct(SaveManager saveManager, GameStateService gameStateService)
        {
            _saveManager = saveManager;
            _gameStateService = gameStateService;
        }

        private void OnEnable()
        {
            if (_gameStateService != null)
            {
                _gameStateService.OnDayChanged += HandleDayChanged;
            }
        }

        private void OnDisable()
        {
            if (_gameStateService != null)
            {
                _gameStateService.OnDayChanged -= HandleDayChanged;
            }
        }

        private async void Start()
        {
            if (_saveManager == null || _gameStateService == null)
            {
                return;
            }

            var loaded = await _saveManager.LoadAsync(_slotName);
            if (!loaded)
            {
                await _saveManager.SaveAsync(_slotName);
            }
        }

        private void HandleDayChanged(int day)
        {
            SaveCurrentState();
        }

        private async void SaveCurrentState()
        {
            if (_saveManager == null)
            {
                return;
            }

            await _saveManager.SaveAsync(_slotName);
        }
    }
}
