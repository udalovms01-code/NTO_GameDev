using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Visuals
{
    public class UnderlineHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public RectTransform underline;
        public float animSpeed = 8f;

        private float targetWidth;
        private float fullWidth;

        private void Start()
        {
            if (underline == null)
                Debug.LogError("Assign underline RectTransform!");

            fullWidth = transform.GetComponent<RectTransform>().rect.width;
            underline.sizeDelta = new Vector2(0, underline.sizeDelta.y);
        }

        private void Update()
        {
            float newWidth = Mathf.Lerp(underline.sizeDelta.x, targetWidth, Time.deltaTime * animSpeed);
            underline.sizeDelta = new Vector2(newWidth, underline.sizeDelta.y);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            targetWidth = fullWidth;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            targetWidth = 0f;
        }
    }
}