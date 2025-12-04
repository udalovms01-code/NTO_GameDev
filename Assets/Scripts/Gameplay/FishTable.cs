using UnityEngine;
using Zenject;

namespace Gameplay
{
    [RequireComponent(typeof(OutlineController))]
    public class FishTable : MonoBehaviour, IIteractable
    {
        [SerializeField] private GameObject _camera;
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
            G.main.StartGame();
            _camera.SetActive(true);
            _cameraSwitch.SwitchCamera(false);
            _gameStateService.SetFishesSliced(true);
        }

        public void UnInteract()
        {
            
        }
    }
}