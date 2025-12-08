using DG.Tweening;
using Localization;
using TMPro;
using UnityEngine;

public class UITooltip : MonoBehaviour
{
    public TMP_Text name;
    public TMP_Text description;
    RectTransform _rectTransform;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    public void loll()
    {
        
    }


    public void Show(FishState state)
    {
        if (state == null) return;
        
        gameObject.SetActive(true);

        transform.DOKill(true);
        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f);

        _rectTransform.anchoredPosition = G.hud.MousePos();
        if (state.model.Is<TagFishView>(out var fv))
        {
            Debug.Log((fv.name, LocalizationManager.Get(fv.name)));
            name.text = LocalizationManager.Get(fv.name);
            description.text = fv.description;
        }    
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            if(G.drag_dice!=null)
                Hide();
            
            _rectTransform.anchoredPosition = G.hud.MousePos();
        }

        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(_rectTransform.rect.height, /*label.textBounds.size.y +*/ 250, 0.5f));
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}