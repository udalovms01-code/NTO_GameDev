using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordFish : FishBase
{
    public SwordFish()
    {
        Define<TagFishView>().name = "swordfish";
        Define<TagFishView>().sprite = SpriteUtil.Load("fishes", "swordfish");
        Define<TagFishView>().dead_sprite = SpriteUtil.Load("dead_fishes", "swordfish");
        Define<TagFishView>().description = "Получает +1, если напротив Лещ";
        Define<TagBream>().delta = 1;
        Define<TagSizes>().possibleSizes = new List<FishSize>()
        {
            FishSize.Medium
        };
        Define<TagLegendary>();
    }
}

public class TagSwordfish : EntityComponentDefinition
{
    
}

public class TagSwordInteraction : BaseInteraction, IOnTryToEat
{
    public IEnumerator OnTryToEat(FishState obj_state, FishState subj_state)
    {
        if (obj_state.model.Is<TagSwordfish>(out var tfl))
        {
            if (subj_state != null)
            {
                if (subj_state.view != null)
                {
                    if (subj_state.model.Is<TagFishView>(out var fv))
                    {
                        //subj_state.view.spriteAnimator.TiltAndReturn();
                        yield return subj_state.view.Activate();
                        yield return new WaitForSeconds(0.25f);
                    }
                }
            }
        }
    }
}

public interface IOnTryToEat
{
    public IEnumerator OnTryToEat(FishState obj_state, FishState subj_state);
}