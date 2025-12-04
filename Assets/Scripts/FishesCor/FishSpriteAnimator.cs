using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class FishSpriteAnimator : MonoBehaviour
{   
    public InteractiveObject view;
    public SortingGroup sortingGroup;
    
    private Vector3 originScale;
    private Vector3 originRotation;
    private float sizeUpValue = 1f;
    private Sequence idleSequence;

    [Header("Animation")] 
    public float selectedSize = 1.15f;
    public float dragSize = 1.3f;
    public float tiltAngle = 10f;
    public float breathScale = 1.05f;
    public float balatroRotationAngle = 4;
    public float balatroRotationDuration = 1;

    


    void Start()
    {
        originScale = transform.localScale;
        originRotation = transform.localEulerAngles;
        
        view.Select += Select;
        view.UnSelect += UnSelect;
        view.StartDrag += StartDrag;
        view.EndDrag += EndDrag;
        view.Activate += Activate;
        view.OnDestroy += KillAll;
        
        G.main.SceneChange += KillAll;

        SetIdleAnim();
    }

    public void KillAll()
    {
        idleSequence?.Kill();
        transform.DOKill();
    }

    public void Activate()
    {
        TiltAndReturn();
    }
    private void EndDrag()
    {
        SizeDown(selectedSize);
    }

    public void StartDrag()
    {
        SizeUp(dragSize);
    }

    public void UnSelect()
    {
        sortingGroup.sortingLayerName = "Fish";

        
        SetIdleAnim();
        SizeDown(1);
    }
    public void Select()
    {
        sortingGroup.sortingLayerName = "Selected";
        
        StopIdleAnim();
        SizeUp(selectedSize);
    } 
    

    public Tween Breathe(float cycleDuration = 2f, int loops = -1, bool randomOffset = true)
    {
        Vector3 breathedScale = originScale * breathScale;
        
        Tween tween = transform.DOScale(breathedScale, cycleDuration)
            .SetLoops(loops * 2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
        
        // Если нужен случайный сдвиг, прыгаем на случайный момент в анимации
        if (randomOffset)
        {
            float randomTime = Random.Range(0f, cycleDuration);
            tween.Goto(randomTime, andPlay: true);
        }
        
        return tween;
    }
    
    public Tween BalatroTilting(
        int vibrato = 1)
    {
        float duration = Random.Range(balatroRotationDuration, 2f * balatroRotationDuration);
        float rotation = Random.Range(0, 2) == 1 ? balatroRotationAngle : -balatroRotationAngle;
        
        // Случайное направление: по часовой (1) или против (-1)
        int direction = Random.value > 0.5f ? 1 : -1;
        float finalRotation = rotation * direction;
        
        return transform.DORotate(
            new Vector3(0, 0, finalRotation),
            duration,
            RotateMode.FastBeyond360
        ).SetRelative().SetLoops(-2, LoopType.Yoyo).SetEase(Ease.InOutSine);
        /*return transform.DOShakeRotation(
            1f,
            new Vector3(0, 0, 10f)
        ).SetLoops(-2, LoopType.Yoyo).SetEase(Ease.InOutSine);*/
    }
    
    void SetIdleAnim()
    {
        //transform.DOKill();
        //yield return new WaitForSeconds(Random.Range(0f, 2f));
        idleSequence?.Kill();
        idleSequence = DOTween.Sequence();
        idleSequence.Append(Breathe());
        idleSequence.Join(BalatroTilting());
        float randomOffset = Random.Range(0f, 20f);
        idleSequence.Goto(randomOffset, true);
    }
    
    public void StopIdleAnim()
    {
        idleSequence?.Kill();
        idleSequence = null;
        
        transform.rotation = Quaternion.Euler(originRotation);
        transform.localScale = originScale;
    }
    public void TiltAndReturn( 
        float duration = 0.15f, int vibrato = 0)
    {
        transform.DOKill();
        transform.DOShakeRotation(
            duration,
            new Vector3(0, tiltAngle, 0),
            vibrato
        );
    }
    
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
    
    public void Punch()
    {
        transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f);
    }
}
