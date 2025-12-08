using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiranhaFish : FishBase
{
    public PiranhaFish()
    {
        Define<TagFishView>().name = "piranha";
        Define<TagFishView>().sprite = SpriteUtil.Load("fishes", "piranha");
        Define<TagFishView>().description = "Хищник\n Получает +1, когда съедает рыбу";
        Define<TagPiranha>().delta = 1;
        Define<TagSizes>().possibleSizes = new List<FishSize>()
        {
            FishSize.Medium
        };
    }
}
public class TagPiranha : EntityComponentDefinition
{
    public int delta = 1;
}

public class PiranhaInteraction : BaseInteraction, IOnEat
{
    public IEnumerator OnEat(FishState obj_state, FishState subj_state)
    {
        if (obj_state.model.Is<TagPiranha>(out var tfl))
        {
            InteractiveObject obj_view = obj_state.view;
            InteractiveObject subj_view = subj_state.view;
            if (true)
            {
                obj_view.SetValue(obj_state.fishValue + tfl.delta);
                obj_view.spriteAnimator.Punch();
                yield return new WaitForSeconds(0.25f * G.CorGameplayConfig.animationMultiplier);
            }
        }
    }
}