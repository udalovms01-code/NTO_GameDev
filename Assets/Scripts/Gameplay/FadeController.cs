using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Gameplay
{
    public class FadeController : MonoBehaviour
    {
        [SerializeField] private Image fadeImage;
        [SerializeField] private float duration = 0.8f;

        public static FadeController Instance { get; private set; }

        public float Duration => duration;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Instance = this;
            if (fadeImage != null)
            {
                // Делаем изображение полностью прозрачным при старте
                var c = fadeImage.color;
                c.a = 0f;
                fadeImage.color = c;
            }

            if (GetComponent<SceneTransitionController>() == null)
            {
                gameObject.AddComponent<SceneTransitionController>();
            }
        }

        /// <summary>
        /// Затемнить экран (до черного)
        /// </summary>
        public Tween FadeIn()
        {
            return fadeImage.DOFade(1f, duration).SetEase(Ease.Linear);
        }

        public Tween FadeIn(Action onComplete)
        {
            return fadeImage.DOFade(1f, duration).SetEase(Ease.Linear).OnComplete(onComplete.Invoke);
        }

        /// <summary>
        /// Осветлить экран (убрать затемнение)
        /// </summary>
        public Tween FadeOut()
        {
            return fadeImage.DOFade(0f, duration).SetEase(Ease.Linear);
        }

        public Tween FadeOut(Action onComplete)
        {
            return fadeImage.DOFade(0f, duration).SetEase(Ease.Linear).OnComplete(onComplete.Invoke);
        }
    }
}
