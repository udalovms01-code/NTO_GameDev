using Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        public void QuitGame()
        {
            Application.Quit();
        }
        
        public void PlayGame()
        {
           if (SceneTransitionController.Instance != null)
           {
               SceneTransitionController.Instance.LoadScene(1);
               return;
           }

           SceneManager.LoadScene(1);
        }
    }
}