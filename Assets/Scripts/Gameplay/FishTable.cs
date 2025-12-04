using UnityEngine;
using Zenject;

namespace Gameplay
{
    [RequireComponent(typeof(OutlineController))]
    public class FishTable : MonoBehaviour, IIteractable
    {
        
        private GameStateService _gameStateService;
        
        [Inject]
        private void Construct(GameStateService gameStateService)
        {
            _gameStateService = gameStateService;
        }
        
        public void Interact()
        {
            
        }

        public void OnClick()
        {
            _gameStateService.SetFishesSliced(true);
        }

        public void UnInteract()
        {
            
        }
    }
}