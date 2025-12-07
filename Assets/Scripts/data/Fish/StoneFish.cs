using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneFish : FishBase
{
    public StoneFish()
    {
        Define<TagFishView>().name = "Рыба-камень";
        Define<TagFishView>().sprite = SpriteUtil.Load("fishes", "stonefish");
        Define<TagFishView>().dead_sprite = SpriteUtil.Load("dead_fishes", "stonefish");
        Define<TagFishView>().description = "Бессмертен для хищников";
        Define<TagBream>().delta = 1;
    }
}

public class TagStonefish : EntityComponentDefinition
{
    
}

public class TagStonefishInteraction : BaseInteraction, IOnTryToEat
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