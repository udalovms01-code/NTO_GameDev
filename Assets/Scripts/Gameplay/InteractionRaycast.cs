using System;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    interface IIteractable
    {
        public void Interact();
    
        public void OnClick();
    
        public void UnInteract();
    }
    
    public class InteractionRaycast : MonoBehaviour
    {
        public float distance = 5f;
        public LayerMask interactMask;

        private OutlineController _current;
        private IIteractable[] _iterables = Array.Empty<IIteractable>();

        private void Update()
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
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
                if(Input.GetMouseButtonDown(0))
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