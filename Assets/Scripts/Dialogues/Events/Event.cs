using UnityEngine;

namespace Dialogues.Events
{
    [System.Serializable]
    public class Event : ScriptableObject
    {
        public void Invoke()
        {
            Debug.Log("Event invoked.");
        }
    }
}