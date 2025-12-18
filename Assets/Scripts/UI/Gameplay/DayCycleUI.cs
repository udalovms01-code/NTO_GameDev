using System;
using DG.Tweening;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Gameplay
{
    public class DayCycleUI : MonoBehaviour
    {
        [SerializeField] private GameObject sleepState;
        [SerializeField] private GameObject slicedFishState;
        [SerializeField] private GameObject dialogState;
        
        [SerializeField] private Image slider;
        public RectTransform uiImageRect;

        private GameStateService _gameStateService;

        // Параметры пульсации
        [SerializeField] private float minPulseDuration = 0.2f; // при почти нулевом голоде (value ~ 0) – очень частый пульс
        [SerializeField] private float maxPulseDuration = 1.2f; // при полном голоде (value ~ 1) – редкий пульс
        [SerializeField] private float pulseScaleMultiplier = 1.06f; // насколько масштаб увеличивается в пике

        private Tween _pulseTween;
        private Vector3 _baseScale = Vector3.one;

        private void Awake()
        {
            if (uiImageRect != null)
                _baseScale = uiImageRect.localScale;
        }
        
        [Inject]
        public void Construct(GameStateService gameStateService)
        {
            _gameStateService = gameStateService;
            gameStateService.OnStateChanged += SetState;
            gameStateService.OnHungerChanged += ChangeSliderState;
            gameStateService.OnHungerChangedByFishvalue += ChangeSliderByFishvalue;
        }
        
        private void OnDestroy()
        {
            if (_gameStateService != null)
            {
                _gameStateService.OnStateChanged -= SetState;
                _gameStateService.OnHungerChanged -= ChangeSliderState;
                _gameStateService.OnHungerChangedByFishvalue -= ChangeSliderByFishvalue;
            }

            KillPulseTween();
        }

        private void KillPulseTween()
        {
            if (_pulseTween != null && _pulseTween.IsActive())
            {
                _pulseTween.Kill();
                _pulseTween = null;
            }

            if (uiImageRect != null)
                uiImageRect.localScale = _baseScale;
        }

        private void StartPulse(float hunger01)
        {
            /*if (uiImageRect == null)
                return;

            // hunger01 в [0,1]; чем меньше hunger01, тем короче длительность и выше частота
            float t = Mathf.Clamp01(hunger01);
            float duration = Mathf.Lerp(minPulseDuration, maxPulseDuration, t);

            KillPulseTween();

            // Пульс: масштаб чуть растёт и возвращается обратно
            _pulseTween = uiImageRect
                .DOScale(_baseScale * pulseScaleMultiplier, duration * 0.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);*/
        }

        public void ChangeSliderState(float value)
        {
            // value предполагается в [0,1]
            float clamped = Mathf.Clamp01(value);
            slider.fillAmount = (float)Math.Round(clamped, 2);

            // Обновляем частоту пульса в зависимости от голода
            StartPulse(clamped);
        }

        public void ChangeSliderByFishvalue(float value)
        {
            if (value == 0f) return;
            if (uiImageRect == null) return;

            // Не трогаем бесконечный пульс, а поверх него делаем короткий punch
            uiImageRect.DOKill(); // убьёт все твины у этого transform'а, в т.ч. пульс
            // поэтому сразу после punch восстановим пульс с текущим голодом (slider.fillAmount)

            uiImageRect.localScale = _baseScale;
            uiImageRect
                .DOPunchScale(value > 0 ? Vector3.one * 0.08f : Vector3.one * -0.08f, 0.5f, 0, 0f)
                .OnComplete(() =>
                {
                    // восстановить базовый масштаб и перезапустить пульс с текущим значением
                    uiImageRect.localScale = _baseScale;
                    StartPulse(slider.fillAmount);
                });
        }

        public void SetState(GameState state)
        {
            slider.fillAmount = 0f;
            KillPulseTween(); // при смене стейта сбрасываем анимацию (по желанию можно закомментить)

            switch (state)
            {
                case GameState.WaitingForSleep:
                    sleepState.SetActive(true);
                    slicedFishState.SetActive(false);
                    dialogState.SetActive(false);
                    break;
                case GameState.WaitingForSlicedFish:
                    sleepState.SetActive(false);
                    slicedFishState.SetActive(true);
                    dialogState.SetActive(false);
                    break;
                case GameState.WaitingForDialog:
                    sleepState.SetActive(false);
                    slicedFishState.SetActive(false);
                    dialogState.SetActive(true);
                    break;
            }
        }
    }
}
