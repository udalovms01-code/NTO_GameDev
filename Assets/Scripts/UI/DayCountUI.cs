using System;
using Gameplay;
using Localization;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class DayCountUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _dayCountText;
        
        private GameStateService _gameStateService;
        
        [Inject]
        public void Construct(GameStateService gameStateService)
        {
            _gameStateService = gameStateService;
            _gameStateService.OnDayChanged += UpdateDayCount;
        }

        private void Start()
        {
            UpdateDayCount(_gameStateService.CurrentDay);
        }

        private void UpdateDayCount(int day)
        {
            _dayCountText.text = LocalizationManager.Get("ui.day") + " " + day;
        }
    }
}