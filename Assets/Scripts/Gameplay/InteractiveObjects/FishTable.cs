using System;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class FishTable : MonoBehaviour, IIteractable
    {
        private GameStateService _gameStateService;
        private CameraSwitch _cameraSwitch;
        private Outline _outline;

        private void Awake()
        {
            _outline = GetComponent<Outline>();
        }

        [Inject]
        private void Construct(GameStateService gameStateService, CameraSwitch cameraSwitch)
        {
            _gameStateService = gameStateService;
            _cameraSwitch = cameraSwitch;
            _gameStateService.OnStateChanged += GameStateChanged;
        }
        
        private void OnDestroy()
        {
            _gameStateService.OnStateChanged -= GameStateChanged;
        }
        
        public void GameStateChanged(GameState state)
        {
            _outline.enabled = state == GameState.WaitingForSlicedFish;
        }
        
        public void Interact()
        {
             
        }

        public void OnClick()
        {
            if (_gameStateService.CurrentState != GameState.WaitingForSlicedFish) return;
            _gameStateService.SetState(GameState.SlicedFish);
            G.main.StartGame();
            _cameraSwitch.SwitchCamera(false);
            G.main.OnGameEnd += Close;
        }
        
        public void Close()
        {
            if (_gameStateService.CurrentState != GameState.SlicedFish) return;
            G.main.OnGameEnd -= Close;
            _gameStateService.SetState(GameState.WaitingForDialog);
            _cameraSwitch.SwitchCamera(true);
        }

        public void UnInteract()
        {
            
        }
    }
}