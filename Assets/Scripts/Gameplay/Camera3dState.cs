using UnityEngine;

namespace Gameplay
{
    public class Camera3dState : MonoBehaviour
    {
        [SerializeField] private GameObject[] _elements;
        
        public static Camera2dState Instance { get; private set; }
        
        public void EnableElements(bool state)
        {
            foreach (var element in _elements)
            {
                element.SetActive(state);
            }
        }
    }
}