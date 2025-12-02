using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class FishZone : MonoBehaviour
{
    public List<InteractiveObject> objects = new List<InteractiveObject>();
    public float spacing = 1f;
    public bool canDrag = true;
    public bool freezeAligning = false;

    public UnityAction<InteractiveObject> OnClickFish;
    
    Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.position;
    }

    public void Claim(InteractiveObject toClaim)
    {
        if (toClaim.zone != null)
            toClaim.zone.Release(toClaim);

        toClaim.zone = this;
        objects.Insert(0, toClaim);
    }

    public void Release(InteractiveObject toClaim)
    {
        if (objects.Contains(toClaim))
            objects.Remove(toClaim);
    }

    List<InteractiveObject> alignedSet = new List<InteractiveObject>();

    public void ChangePos(InteractiveObject toChange)
    {
        int ind = objects.IndexOf(toChange);
        float oldOffsetX = ind * spacing - (objects.Count / 2f - 0.5f) * spacing;
        float oldPosX = originalPosition.x + oldOffsetX;

        float dif = toChange.transform.position.x - oldPosX;

        if (dif > spacing && ind != objects.Count - 1)
        {
            (objects[ind], objects[ind + 1]) = (objects[ind + 1], objects[ind]);
        }
        else if (dif < -spacing && ind != 0)
        {
            (objects[ind], objects[ind - 1]) = (objects[ind - 1], objects[ind]);
        }
    }

    void Update()
    {
        if (!freezeAligning)
        {
            for (var i = 0; i < objects.Count; i++)
            {
                if ((!objects[i].draggable.isDragging) ||  objects[i] == null)
                {
                    var targetPos = GetTargetPos(i, objects);
                    objects[i].moveable.targetPosition = targetPos;
                    //alignedSet[i].moveable.targetPosition = targetPos;
                }
            }
        }
    }

    public void Align()
    {
        var copy = objects;
        for (var i = 0; i < copy.Count; i++)
        {
            if (copy[i] == null)
            {
                objects.RemoveAt(i);
            }
        }
        for (var i = 0; i < objects.Count; i++)
        {
            var targetPos = GetTargetPos(i, objects);
            objects[i].moveable.targetPosition = targetPos;
        }
    }
    public void FreezeAligning()
    {
        freezeAligning = true;
    }
    public void UnreezeAligning()
    {
        freezeAligning = false;
    }

    Vector3 GetTargetPos(int i, List<InteractiveObject> setToWatch)
    {
        var offset = i * spacing - (setToWatch.Count / 2f - 0.5f) * spacing;
        var targetPos = transform.position + Vector3.right * offset;
        return targetPos;
    }
    
    public void AlignSetForCutting()
    {
        int cnt = objects.Count;
        for (var i = 0; i < cnt; i++)
        {
            var offset = 2.5f * spacing - (cnt - i - 1) * spacing;
            var targetPos = transform.position + Vector3.right * offset;
            objects[i].moveable.targetPosition = targetPos;
            //alignedSet[i].moveable.targetPosition = targetPos;
        }
    }
    public IEnumerator TryToEat(InteractiveObject io)
    {
        InteractiveObject subj = FrontFish(io);
        if (subj == null) yield break;
        
        switch (io.state.size){
            case FishSize.Medium:
                if (subj.state.size == FishSize.Small)
                    yield return Eat(io, subj);
                break;
            case FishSize.Big:
                if (subj.state.size == FishSize.Small || subj.state.size == FishSize.Medium)
                    yield return Eat(io, subj);
                break;
            default:
                yield break;
        }
    }
    
    public IEnumerator Eat(InteractiveObject obj, InteractiveObject subj)
    {
        objects[ZoneIndex(subj)] = null;
        yield return subj.EatenCoroutine();


        //objects[ZoneIndex(subj)] = objects[ZoneIndex(obj)];

        yield break;
    }
    
    public InteractiveObject LastFish()
    {
        if (objects.Count == 0)
            return null;

        return objects.Last();
    }
    
    public InteractiveObject FrontFish(InteractiveObject view)
    {
        if (objects.Count == 0)
            return null;
        
        int ind = objects.IndexOf(view);


        if (view.state.direction == FishDirection.Right)
            ind++;
        else
            ind--;  

        if (ind > objects.Count - 1)
            return null;
        if (ind < 0)
            return null;

        return objects[ind];
    }

    public int ZoneIndex(InteractiveObject view)
    {
        return objects.IndexOf(view);
    }

    public bool IsLast(InteractiveObject view)
    {
        return objects.IndexOf(view) == objects.Count - 1;
    }
}