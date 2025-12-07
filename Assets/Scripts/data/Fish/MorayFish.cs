using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorayFish : FishBase
{
    public MorayFish()
    {
        Define<TagFishView>().name = "moray";
        Define<TagFishView>().sprite = SpriteUtil.Load("fishes", "moray_eel");
        Define<TagFishView>().dead_sprite = SpriteUtil.Load("dead_fishes", "moray_eel");
        Define<TagFishView>().description = "Получает +1, если съедает рыбу";
        Define<TagFatty>().delta = 1;
        Define<TagSizes>().possibleSizes = new List<FishSize>()
        {
            FishSize.Big
        };
    }
}

public class TagMorayFish : EntityComponentDefinition
{
    public int delta;
}

public class MorayInteraction : BaseInteraction, IOnEat
{
    public IEnumerator OnEat(FishState obj_state, FishState subj_state)
    {
        if (obj_state.model.Is<TagFatty>(out var tfl))
        {
            InteractiveObject obj_view = obj_state.view;
            InteractiveObject subj_view = subj_state.view;
            if (true)
            {
                obj_view.SetValue(obj_state.fishValue + tfl.delta);
                obj_view.spriteAnimator.Punch();
                yield return new WaitForSeconds(0.25f * G.CorGameplayConfig.animationSpeed);
            }
        }
    }
}