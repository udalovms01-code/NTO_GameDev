using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class FishState
{
    public CMSEntity model;
    public int fishValue;
    public FishDirection direction = FishDirection.Right;
    public FishSize size = FishSize.Small;
    public InteractiveObject view;
    public bool virused;
}

public class InteractiveObject : MonoBehaviour, IClickable
{
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer shadowSpriteRenderer;
    public FishSpriteAnimator spriteAnimator;

    public FishState state;

    public TMP_Text value;

    public MoveableBase moveable;
    public DraggableSmoothDamp draggable;

    public FishZone zone;
    private Vector3 originScale;
    public bool isSelected = false;

    public UnityAction Select;
    public UnityAction UnSelect;
    public UnityAction StartDrag;
    public UnityAction EndDrag;
    public UnityAction OnActivate;
    public UnityAction OnDestroy;


    void Start()
    {
        value.text = state.model.Get<TagStartFishValue>().value.ToString();
        draggable = GetComponent<DraggableSmoothDamp>();
        originScale = transform.localScale;

        G.main.SceneChange += KillTweens;
    }

    public void SetState(FishState fishState)
    {
        state = fishState;
        state.view = this;

        if (state.model.Is<TagTint>(out var tint))
        {
            spriteRenderer.color = tint.color;
        }

        if (state.model.Is<TagFishView>(out var fv))
        {
            spriteRenderer.sprite = fv.sprite;
            shadowSpriteRenderer.sprite = fv.sprite;
        }


        if (state.model.Is<TagStartFishValue>(out var sfv))
            SetValue(sfv.value);


        //state.direction = Random.Range(0, 2) == 0 ? FishDirection.Right : FishDirection.Left;

        if (state.direction == FishDirection.Right)
        {
            shadowSpriteRenderer.flipX = false;
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
            shadowSpriteRenderer.flipX = true;
        }


        if (state.model.Is<TagSizes>(out var sz))
        {
            //state.size = sz.possibleSizes[Random.Range(0, sz.possibleSizes.Count - 1)];

            /*if (sz.possibleSizes.Count > 1)
            {*/
            switch (state.size)
            {
                case FishSize.Medium:
                    spriteRenderer.gameObject.transform.localScale *= 1.1f;
                    break;
                case FishSize.Big:
                    spriteRenderer.gameObject.transform.localScale *= 1.2f;
                    break;
            }
        }
    }

    public void InitState(FishState fishState, bool strategies = false, FishDirection dir = FishDirection.Right)
    {
        state = fishState;
        state.view = this;

        if (!strategies)
            state.direction = Random.Range(0, 2) == 0 ? FishDirection.Right : FishDirection.Left;
        else
            state.direction = dir;

        if (state.model.Is<TagSizes>(out var sz))
        {
            state.size = sz.possibleSizes[Random.Range(0, sz.possibleSizes.Count - 1)];
        }
        
        SetState(fishState);
    }
    public IEnumerator Activate()
    {
        OnActivate?.Invoke();
        
        var endTurn = G.main.interactor.FindAll<IOnEndTurn>();
        foreach (var et in endTurn)
            yield return et.OnEndTurn(state);
    }

    public void SetValue(int val)
    {
        state.fishValue = val;
        value.text = val.ToString();
    }

    private float sizeUpValue = 1f;

    public void OnMouseExit()
    {
        if (isSelected && !draggable.isDragging)
        {
            G.hud.tooltip.Hide();
            UnSelect.Invoke();
            isSelected = false;
        }
    }

    public void OnMouseEnter()
    {
        if (zone.canDrag)
        {
            if (!isSelected)
            {
                G.hud.tooltip.Show(state);
                Select.Invoke();
                isSelected = true;
            }
        }
    }


    public void OnMouseDown()
    {
        if (zone.canDrag)
        {
            if (!draggable.isDragging)
                StartDrag.Invoke();

            if (zone != null)
            {
                zone.OnClickFish?.Invoke(this);
            }
        }    
    }

    public void OnMouseUp()
    {
        EndDrag.Invoke();
    }

    void KillTweens()
    {
        transform.DOKill();
    }

    public void Eaten()
    {
        StartCoroutine(EatenCoroutine());
    }

    public IEnumerator EatenCoroutine()
    {
        yield return DieAnim();
        yield return new WaitForSeconds(0.2f);
        yield return Die();
    }

    IEnumerator GetPoints()
    {
        G.main.AddHunger(state.fishValue / 10f);
        if (state.fishValue > 0)
            G.run.pointsSum += state.fishValue;
        yield break;
    }

    public IEnumerator CutCoroutine()
    {
        yield return DieAnim();
        yield return GetPoints();
        //G.run.pointsSum += state.fishValue;
        yield return new WaitForSeconds(0.2f * G.CorGameplayConfig.animationMultiplier);
        yield return Die();
    }

    public IEnumerator DieAnim()
    {
        if (state.model.Is<TagFishView>(out var fv))
        {
            spriteRenderer.sprite = fv.dead_sprite;
        }
        else if (state.model.Is<TagTint>(out var tint))
        {
            spriteRenderer.color = Color.gray;
        }

        yield break;
    }

    public IEnumerator Die()
    {
        OnDestroy?.Invoke();
        
        //G.main.DeleteFish(this);

        transform.DOKill();
        Destroy(gameObject);
        yield break;
    }

    private void OnDisable()
    {
        G.main.SceneChange -= KillTweens;
    }

    public void MakeVirused()
    {
        state.virused = true;
        if (state.model.Is<TagVirusedForm>(out var vf))
        {
            spriteRenderer.sprite = vf.sprite;
            shadowSpriteRenderer.sprite = vf.sprite;
            state.fishValue = vf.negativeValue;
        }
        else
        {
            spriteRenderer.color = Color.green;
            state.fishValue = -2;
        }
    }
}