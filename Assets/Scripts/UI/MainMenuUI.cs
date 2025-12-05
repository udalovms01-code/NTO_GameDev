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
           SceneManager.LoadScene(1);
        }
    }
}