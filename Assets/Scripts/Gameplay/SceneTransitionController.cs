using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    [RequireComponent(typeof(FadeController))]
    public class SceneTransitionController : MonoBehaviour
    {
        [SerializeField] private float audioFadeDuration = 0.8f;
        [SerializeField] private float targetFadeVolume = 0f;

        public static SceneTransitionController Instance { get; private set; }

        private FadeController fadeController;
        private bool isTransitionActive;
        private float initialVolume;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            //DontDestroyOnLoad(gameObject);
            Instance = this;
            fadeController = GetComponent<FadeController>();
            initialVolume = AudioListener.volume;
        }

        public void LoadScene(int buildIndex)
        {
            if (!isTransitionActive)
            {
                StartCoroutine(LoadSceneRoutine(() => SceneManager.LoadSceneAsync(buildIndex)));
            }
        }

        public void LoadScene(string sceneName)
        {
            if (!isTransitionActive)
            {
                StartCoroutine(LoadSceneRoutine(() => SceneManager.LoadSceneAsync(sceneName)));
            }
        }

        private IEnumerator LoadSceneRoutine(System.Func<AsyncOperation> loadOperation)
        {
            isTransitionActive = true;
            initialVolume = AudioListener.volume;

            Tween fadeInTween = fadeController != null ? fadeController.FadeIn() : null;
            Tween volumeDownTween = DOTween.To(() => AudioListener.volume, v => AudioListener.volume = v, targetFadeVolume,
                audioFadeDuration).SetEase(Ease.Linear);

            yield return DOTween.Sequence()
                .Join(volumeDownTween)
                .Join(fadeInTween ?? DOTween.Sequence().AppendInterval(audioFadeDuration))
                .WaitForCompletion();

            AsyncOperation operation = loadOperation.Invoke();
            yield return operation;

            Tween fadeOutTween = fadeController != null ? fadeController.FadeOut() : null;
            Tween volumeUpTween = DOTween.To(() => AudioListener.volume, v => AudioListener.volume = v, initialVolume,
                audioFadeDuration).SetEase(Ease.Linear);

            yield return DOTween.Sequence()
                .Join(volumeUpTween)
                .Join(fadeOutTween ?? DOTween.Sequence().AppendInterval(audioFadeDuration))
                .WaitForCompletion();

            isTransitionActive = false;
        }
        
    }
}
