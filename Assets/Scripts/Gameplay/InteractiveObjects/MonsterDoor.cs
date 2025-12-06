using System;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class MonsterDoor : DoorTo2DInteract
    {
        private Outline _outline; 
        private GameStateService _gameStateService;

        private void Awake()
        {
            _outline = GetComponent<Outline>();
        }

        [Inject]
        private void Construct(GameStateService gameStateService)
        {
            _gameStateService = gameStateService;
            _gameStateService.OnStateChanged += GameStateChanged;
        }
        
        private void OnDestroy()
        {
            _gameStateService.OnStateChanged -= GameStateChanged;
        }
        
        public void GameStateChanged(GameState state)
        {
            _outline.enabled = state == GameState.WaitingForDialog;
        }
        
        public override void OnClick()
        {
            if (_gameStateService.CurrentState!= GameState.WaitingForDialog) return;
            _gameStateService.SetState(GameState.Dialog);
            base.OnClick();
        }
    }
}