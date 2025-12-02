using UnityEngine;
using DG.Tweening;

public class KnifeInteractive : MonoBehaviour
{
    private Vector3 originScale;
    public bool isSelected = false;

    void Start()
    {
        originScale = transform.localScale;
    }
    
    public void Punch()
    {
        transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f);
    }

    private float sizeUpValue = 1f;

    private void SizeUp(float value)
    {
        if (value <= sizeUpValue) return;
        sizeUpValue = value;
        transform.DOKill();
        transform.DOBlendableScaleBy(originScale * sizeUpValue - transform.localScale, 
            0.1f);
    }
    private void SizeDown(float value)
    {
        if (value >= sizeUpValue) return;
        sizeUpValue = value;
        transform.DOKill();
        transform.DOBlendableScaleBy(originScale * sizeUpValue - transform.localScale, 
            0.1f);
    }

    public void OnMouseExit()
    {
        if (isSelected)
        {
            SizeDown(1f);
            isSelected = false;
        }
    }

    public void OnMouseEnter()
    {
        if (!isSelected)
        {
            SizeUp(1.075f);
            isSelected = true;
        }
    }

    public void OnMouseDown()
    {
        SizeUp(1.15f);
        OnClickEndTurn();
    }

    public void OnMouseUp()
    {
        SizeDown(1.075f);
    }
    
    void OnClickEndTurn()
    {
        G.main.EndTurn();
    }
}