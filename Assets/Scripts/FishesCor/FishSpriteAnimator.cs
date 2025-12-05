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
        SetIdleAnim();
        SizeDown(selectedSize);
    }

    public void StartDrag()
    {
        StopIdleAnim();
        SizeUp(dragSize);
    }

    public void UnSelect()
    {
        sortingGroup.sortingLayerName = "Fish";


        //SizeDown(1);
        sizeUpValue = 1f;
        SetIdleAnim();
    }
    public void Select()
    {
        sortingGroup.sortingLayerName = "Selected";
        
        StopIdleAnim();
        SizeUp(selectedSize);
    } 
    

    public Tween Breathe(float cycleDuration = 2f, bool randomOffset = true)
    {
        Vector3 breathedScale = originScale * breathScale;
    
        Tween tween = transform.DOScale(breathedScale, cycleDuration)
            .SetLoops(2, LoopType.Yoyo)  // Без -1, просто один цикл
            .SetEase(Ease.InOutSine);
    
        if (randomOffset)
        {
            float randomTime = Random.Range(0f, cycleDuration);
            tween.Goto(randomTime, andPlay: true);
        }
    
        return tween;
    }

    public Tween BalatroTilting(int vibrato = 1)
    {
        float duration = Random.Range(balatroRotationDuration, 2f * balatroRotationDuration);
        int direction = Random.value > 0.5f ? 1 : -1;
        float finalRotation = Random.Range(0, 2) == 1 ? balatroRotationAngle : -balatroRotationAngle;
        finalRotation *= direction;
    
        return transform.DOLocalRotate(
            new Vector3(0, 0, finalRotation),
            duration,
            RotateMode.FastBeyond360
        ).SetRelative().SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    void SetIdleAnim()
    {
        transform.localRotation = Quaternion.Euler(originRotation);
        transform.localScale = originScale;
    
        idleSequence?.Kill();
        idleSequence = DOTween.Sequence();
    
        // Добавляем анимации БЕЗ циклов внутри
        idleSequence.Append(Breathe(2f, true));
        idleSequence.Join(BalatroTilting());
    
        // Бесконечный цикл только на самой Sequence
        idleSequence.SetLoops(-1, LoopType.Restart);
    
        // Случайный сдвиг для разнообразия
        float randomOffset = Random.Range(0f, 20f);
        idleSequence.Goto(randomOffset, true);
    }

    
    public void StopIdleAnim()
    {
        idleSequence?.Kill();
        idleSequence = null;
        
        transform.localRotation = Quaternion.Euler(originRotation);
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
        transform.DOScale(originScale * sizeUpValue, 0.1f);

    }
    
    private void SizeDown(float value)
    {
        if (value >= sizeUpValue) return;
        sizeUpValue = value;
        transform.DOKill();
        transform.DOScale(originScale * sizeUpValue, 0.1f);

    }
    
    public void Punch()
    {
        transform.DOKill();
        transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f);
    }
}
