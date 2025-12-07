using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    public class DeathControl : MonoBehaviour
    {
        public static DeathControl Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }
        
        public void StartDeath()
        {
            Debug.Log("Death");
            FadeController.Instance.FadeIn(() =>
            {
                FadeController.Instance.FadeOut();
                SceneManager.LoadScene(0);
            }); 
        }
    }
}