using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public bool Testing;
    
    public TextMeshProUGUI DiceView;
    //public Button EndTurn;
    public UITooltip tooltip;
    public Text setsCount;
    public Slider Health;

    public ActivationArrow activationArrow;
    //public TMP_Text HealthValue;

    void Awake()
    {
        G.hud = this;
        G.hud.tooltip.Hide();
    }
    void OnClickEndTurn()
    {
        G.main.EndTurn();
    }

    public void DisableHud()
    {
        G.main.field.canDrag = false;
        KnifeInteractive.interactable = false;
        //InteractiveObject.globalInteractive = false;
    }

    public void EnableHud()
    {
        G.main.field.canDrag = true;
        KnifeInteractive.interactable = true;
        //InteractiveObject.globalInteractive = true;
    }
    
    public void ArrowDisappear()
    {
        activationArrow.transform.localPosition = new Vector3(-6, 0, 0);
        activationArrow.gameObject.SetActive(false);
    }    
    
    public void ArrowSelect(Vector3 endPosition, float duration = .3f, Ease ease = Ease.InOutQuad)
    {
        gameObject.SetActive(true);
        activationArrow.gameObject.transform.DOMove(endPosition, duration).SetEase(ease);
    }    

    void Update()
    {
        //DiceView.text = "Dice left:" + G.main.diceBag.Count;
    }

    public static Vector2 MousePositionToCanvasPosition(Canvas canvas, RectTransform rectTransform)
    {
        Vector2 localPoint;
        Vector2 screenPosition = Input.mousePosition;
        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            screenPosition,
            uiCamera,
            out localPoint
        );

        return localPoint;
    }

    public Vector2 MousePos()
    {
        return MousePositionToCanvasPosition(G.hud.GetComponent<Canvas>(), G.hud.GetComponent<RectTransform>());
    }

    public void PunchEndTurn()
    {
        //G.ui.Punch(EndTurn.transform);
    }
}