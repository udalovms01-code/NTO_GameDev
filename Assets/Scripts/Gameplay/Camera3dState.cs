using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay
{
    public class Camera3dState : MonoBehaviour
    {
        public Transform camera3d;
        [SerializeField] private GameObject[] _elements;
        
        public static Camera3dState Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }
        
        public void EnableElements(bool state)
        {
            foreach (var element in _elements)
            {
                element.SetActive(state);
            }
        }
    }
}