
using System;
using DG.Tweening;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Gameplay
{
    public class DayCycleUI : MonoBehaviour
    {
        [SerializeField] private GameObject sleepState;
        [SerializeField] private GameObject slicedFishState;
        [SerializeField] private GameObject dialogState;
        
        //private Slider slider;
        [SerializeField] private Image slider; // debuggg helo helo
        public RectTransform uiImageRect;
        private GameStateService _gameStateService;

        private void Awake()
        {
            //slider = GetComponent<Slider>();
        }
        
        [Inject]
        public void Construct(GameStateService gameStateService)
        {
            _gameStateService = gameStateService;
            gameStateService.OnStateChanged += SetState;
            gameStateService.OnHungerChanged += ChangeSliderState;
        }
        
        private void OnDestroy()
        {
            _gameStateService.OnStateChanged -= SetState;
            _gameStateService.OnHungerChanged -= ChangeSliderState;
        }
        
        public void ChangeSliderState(float value)
        {
            uiImageRect.DOPunchScale(Vector3.one, 0.5f, 8, 1f);
            slider.fillAmount = (float)Math.Round(value, 2);
        }

        public void SetState(GameState state)
        {
            slider.fillAmount = 0f;
            switch (state)
            {
                case GameState.WaitingForSleep:
                    sleepState.SetActive(true);
                    slicedFishState.SetActive(false);
                    dialogState.SetActive(false);
                    break;
                case GameState.WaitingForSlicedFish:
                    sleepState.SetActive(false);
                    slicedFishState.SetActive(true);
                    dialogState.SetActive(false);
                    break;
                case GameState.WaitingForDialog:
                    sleepState.SetActive(false);
                    slicedFishState.SetActive(false);
                    dialogState.SetActive(true);
                    break;
            }
        }
    }
}