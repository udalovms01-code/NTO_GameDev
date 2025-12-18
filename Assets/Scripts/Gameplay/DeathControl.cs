using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Gameplay
{
    public class DeathControl : MonoBehaviour
    {
        public static DeathControl Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }
        
        [Inject]
        public void Construct(GameStateService gameStateService)
        {
            gameStateService.OnHungerChanged += v =>
            {
                if (v - 0.001f < 0)
                {
                    EndScreen.Instance.ShowEndScreen(false);
                }
            };
        }
        
        public void StartDeath()
        {
            FadeController.Instance.FadeIn(() =>
            {
                FadeController.Instance.FadeOut();
                SceneManager.LoadScene(0);
            });
        }
    }
}