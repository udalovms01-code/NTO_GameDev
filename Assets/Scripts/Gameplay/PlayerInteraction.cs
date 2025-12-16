using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    interface IIteractable
    {
        public void Interact();
    
        public void OnClick();
    
        public void UnInteract();
    }
    
    public class PlayerInteraction : ITickable, IInitializable
    {
        private OutlineController _current;
        private IIteractable[] _iterables = Array.Empty<IIteractable>();

        private Camera _camera;
        private GameStateService _gameStateService;
        private LayerMask interactMask;
        private float distance = 1f;
        
        public PlayerInteraction(GameStateService gameStateService, GameplaySettings gameplaySettings)
        {
            _gameStateService = gameStateService;
            interactMask = gameplaySettings.interactMask;
            distance = gameplaySettings.interactDistance;
        }
        
        public void Initialize()
        {
            _camera = Camera.main;
            //Debug.Log("PlayerInteraction initialized.");
            //Debug.Log(_camera ==null);
        }

        public void Tick()
        {
            if (_gameStateService.CurrentState == GameState.SlicedFish) return;
            Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, distance, interactMask))
            {
                OutlineController oc = hit.collider.GetComponent<OutlineController>();
                if (oc != null)
                {
                    if (_current != oc)
                    {
                        if (_current != null) _current.UnInteract();
                        _current = oc;
                        _current.Interact();

                    }
                }

                var iterables = hit.collider.GetComponents<IIteractable>();
                foreach (var iteraction in iterables)
                {
                    iteraction.Interact();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    foreach (var iteraction in iterables)
                    {
                        iteraction.OnClick();
                    }
                }

                foreach (var iteraction in _iterables)
                {
                    if (iterables.Contains(iteraction)) continue;
                    iteraction.UnInteract();
                }

                _iterables = iterables;
            }
            else
            {
                foreach (var iteraction in _iterables)
                {
                    iteraction.UnInteract();
                }

                _iterables = Array.Empty<IIteractable>();
                if (_current != null)
                {
                    _current.UnInteract();
                    _current = null;
                }
            }
        }
    }

}