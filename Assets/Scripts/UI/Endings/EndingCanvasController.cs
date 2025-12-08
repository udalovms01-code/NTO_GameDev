using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace UI.Endings
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(CanvasGroup))]
    public class EndingCanvasController : MonoBehaviour, IPointerClickHandler
    {
        [Header("UI")]
        [SerializeField] private Image backgroundCatcher;
        [SerializeField] private Image illustrationImage;
        [SerializeField] private TMP_Text endingText;

        [Header("Content Source")]
        [SerializeField] private EndingLibrary library;
        [SerializeField] private string libraryResourcePath = "Endings/EndingLibrary";

        [Header("Flow")]
        [SerializeField] private List<EndingDefinition> endings = new();
        [SerializeField] private string defaultEndingId = "default";
        [SerializeField] private int startAfterDay = 7;
        [SerializeField] private bool startOnDayReached = true;

        private GameStateService _gameStateService;
        private CanvasGroup _canvasGroup;
        private EndingDefinition _activeEnding;
        private string _requestedEndingId;
        private int _slideIndex;
        private bool _isTransitioning;
        private bool _endingShown;

        [Inject]
        public void Construct(GameStateService gameStateService)
        {
            _gameStateService = gameStateService;
            _gameStateService.OnDayChanged += HandleDayChanged;
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            LoadLibrary();
            EnsureUiBuilt();
            HideCanvas();
        }

        private void Start()
        {
            if (!startOnDayReached) return;
            if (_gameStateService != null && HasReachedEndingDay(_gameStateService.CurrentDay))
            {
                ShowEndingIfAvailable();
            }
        }

        private void OnDestroy()
        {
            if (_gameStateService != null)
            {
                _gameStateService.OnDayChanged -= HandleDayChanged;
            }
        }

        public void SetEnding(string endingId)
        {
            _requestedEndingId = endingId;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_endingShown || _isTransitioning || _activeEnding == null)
            {
                return;
            }

            if (_slideIndex + 1 >= _activeEnding.Slides.Count)
            {
                return;
            }

            GoToSlide(_slideIndex + 1);
        }

        private void EnsureUiBuilt()
        {
            var canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 2;

            var scaler = GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            GetComponent<GraphicRaycaster>();

            backgroundCatcher ??= GetComponent<Image>() ?? gameObject.AddComponent<Image>();

            var backgroundRect = backgroundCatcher.rectTransform;
            backgroundRect.anchorMin = Vector2.zero;
            backgroundRect.anchorMax = Vector2.one;
            backgroundRect.offsetMin = Vector2.zero;
            backgroundRect.offsetMax = Vector2.zero;
            backgroundRect.anchoredPosition = Vector2.zero;

            backgroundCatcher.color = new Color(0f, 0f, 0f, 0f);
            backgroundCatcher.raycastTarget = true;
            _canvasGroup.blocksRaycasts = false;

            var contentParent = backgroundCatcher.rectTransform;
            if (illustrationImage == null)
            {
                illustrationImage = CreateIllustration(contentParent);
            }
            illustrationImage.raycastTarget = false;

            if (endingText == null)
            {
                endingText = CreateText(contentParent);
            }
            endingText.raycastTarget = false;
        }

        private void LoadLibrary()
        {
            endings ??= new List<EndingDefinition>();

            if (library == null && !string.IsNullOrWhiteSpace(libraryResourcePath))
            {
                library = Resources.Load<EndingLibrary>(libraryResourcePath);
            }

            if (endings.Count == 0 && library != null && library.Endings != null)
            {
                endings = new List<EndingDefinition>(library.Endings.Where(e => e != null));
            }

            if (endings.Count == 0)
            {
                var ending = new EndingDefinition();
                ending.AddSlide("Спасибо за игру!", null);
                endings.Add(ending);
            }
        }

        private Image CreateIllustration(RectTransform parent)
        {
            var illustrationObject = new GameObject("EndingIllustration", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            var rect = illustrationObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(900f, 500f);
            rect.anchoredPosition = new Vector2(0f, 140f);

            var image = illustrationObject.GetComponent<Image>();
            image.preserveAspect = true;
            image.color = Color.white;
            return image;
        }

        private TMP_Text CreateText(RectTransform parent)
        {
            var textObject = new GameObject("EndingText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            var rect = textObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0.1f, 0.05f);
            rect.anchorMax = new Vector2(0.9f, 0.35f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;

            var text = textObject.GetComponent<TextMeshProUGUI>();
            text.fontSize = 42f;
            text.alignment = TextAlignmentOptions.Center;
            text.enableWordWrapping = true;
            text.text = string.Empty;
            return text;
        }

        private void HandleDayChanged(int day)
        {
            if (!startOnDayReached || _endingShown)
            {
                return;
            }

            if (HasReachedEndingDay(day))
            {
                ShowEndingIfAvailable();
            }
        }

        private bool HasReachedEndingDay(int day)
        {
            return day >= startAfterDay;
        }

        private void ShowEndingIfAvailable()
        {
            _activeEnding = ResolveEnding();
            if (_activeEnding == null)
            {
                Debug.LogWarning("EndingCanvasController: no endings configured");
                return;
            }

            _endingShown = true;
            _slideIndex = 0;
            ApplySlide();
            ShowCanvas();
        }

        private EndingDefinition ResolveEnding()
        {
            var endingId = string.IsNullOrWhiteSpace(_requestedEndingId) ? defaultEndingId : _requestedEndingId;
            var ending = endings.FirstOrDefault(e => e != null && e.Id == endingId);
            if (ending != null)
            {
                return ending;
            }

            return endings.FirstOrDefault(e => e != null);
        }

        private void GoToSlide(int index)
        {
            if (_activeEnding == null || index < 0 || index >= _activeEnding.Slides.Count)
            {
                return;
            }

            if (FadeController.Instance == null)
            {
                _slideIndex = index;
                ApplySlide();
                return;
            }

            _isTransitioning = true;
            FadeController.Instance.FadeIn(() =>
            {
                _slideIndex = index;
                ApplySlide();
                FadeController.Instance.FadeOut(() => _isTransitioning = false);
            });
        }

        private void ApplySlide()
        {
            if (_activeEnding == null || _activeEnding.Slides.Count == 0)
            {
                return;
            }

            var slide = _activeEnding.Slides[_slideIndex];
            if (endingText != null)
            {
                endingText.text = slide?.Text ?? string.Empty;
            }

            if (illustrationImage != null)
            {
                illustrationImage.sprite = slide?.Image;
                illustrationImage.enabled = slide?.Image != null;
            }
        }

        private void ShowCanvas()
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }

        private void HideCanvas()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }
    }
}
