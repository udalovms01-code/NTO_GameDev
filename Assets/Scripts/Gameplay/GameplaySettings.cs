using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(fileName = "GameplaySettings", menuName = "GameplaySettings")]
    public class GameplaySettings : ScriptableObject
    {
        public float interactDistance = 1f;
        public LayerMask interactMask;
    }
}