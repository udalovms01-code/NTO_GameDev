using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(Outline))]
    public class OutlineController : MonoBehaviour, IIteractable
    {
        private Outline _outline;

        private void Start()
        { 
            _outline = GetComponent<Outline>();
            _outline.enabled = false;
        }

        public void EnableOutline()
        {
            _outline.enabled = true;
        }

        public void DisableOutline()
        {
            _outline.enabled = false;
        }

        public void Interact()
        {
            EnableOutline();
        }

        public void OnClick()
        {
            
        }

        public void UnInteract()
        {
            DisableOutline();
        }
    }
}