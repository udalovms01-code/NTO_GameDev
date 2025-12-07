using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class Bed : MonoBehaviour, IIteractable
    {
        private GameStateService _gameStateService;
        private Outline _outline;
        
        private void Awake()
        {
            _outline = GetComponent<Outline>();
            _outline.enabled = false;
        }
        
        [Inject]
        protected virtual void Construct(GameStateService gameStateService)
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
            _outline.enabled = state == GameState.WaitingForSleep;
        }
        
        public void Interact()
        {
            
        }

        public void OnClick()
        {
            if(_gameStateService.CurrentState != GameState.WaitingForSleep) return;
            FadeController.Instance.FadeIn(() =>
            {
                _gameStateService.SetDay(_gameStateService.CurrentDay + 1);
                FadeController.Instance.FadeOut();
            });
        }

        public void UnInteract()
        {
            
        }
    }
}