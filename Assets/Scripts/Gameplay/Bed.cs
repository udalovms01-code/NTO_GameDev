using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class Bed : MonoBehaviour, IIteractable
    {
        private GameStateService _gameStateService;
        
        [Inject]
        protected virtual void Construct(GameStateService gameStateService)
        {
            _gameStateService = gameStateService;
        }
        
        public void Interact()
        {
            
        }

        public void OnClick()
        {
            // if(!_gameStateService.IsFishesSliced || !_gameStateService.IsDialogEnded) return;
            
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