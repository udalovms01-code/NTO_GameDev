using UnityEngine;
using Zenject;

namespace Gameplay
{
    [RequireComponent(typeof(OutlineController))]
    public class FishTable : MonoBehaviour, IIteractable
    {
        private GameStateService _gameStateService;
        private CameraSwitch _cameraSwitch;
        
        [Inject]
        private void Construct(GameStateService gameStateService, CameraSwitch cameraSwitch)
        {
            _gameStateService = gameStateService;
            _cameraSwitch = cameraSwitch;
        }
        
        public void Interact()
        {
            
        }

        public void OnClick()
        {
            // _gameStateService.SetFishesSlicedStarted(true);
            G.main.StartGame();
            _cameraSwitch.SwitchCamera(false);
            G.main.OnGameEnd += Close;
        }
        
        public void Close()
        {
            G.main.OnGameEnd -= Close;
            // _gameStateService.SetFishesSlicedStarted(false);
            _cameraSwitch.SwitchCamera(true);
            // _gameStateService.SetFishesSliced(true);
        }

        public void UnInteract()
        {
            
        }
    }
}