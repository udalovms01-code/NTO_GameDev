using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Gameplay
{
    public class EndScreen : MonoBehaviour
    {
        [SerializeField] private GameObject _winScreen;
        [SerializeField] private GameObject _loseScreen;
        
        public static EndScreen Instance { get; private set; }
        
        private DeathControl _deathControl;
        
        [Inject]
        private void Construct(DeathControl deathControl)
        {
            _deathControl = deathControl;
        }
        
        private void Awake()
        {
            Instance = this;
        }
        
        public void ShowEndScreen(bool win)
        {
            _winScreen.SetActive(win);
            _loseScreen.SetActive(!win);
            Invoke(nameof(LoadMenu), 2f);
        }
        
        public void LoadMenu()
        {
            _deathControl.StartDeath();
        }
    }
}