using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FattyFish : FishBase
{
    public FattyFish()
    {
        Define<TagFishView>().name = "fatty";
        Define<TagFishView>().sprite = SpriteUtil.Load("fishes", "fatty");
        Define<TagFishView>().dead_sprite = SpriteUtil.Load("dead_fishes", "fatty");
        Define<TagVirusedForm>().sprite = SpriteUtil.Load("virused_fishes", "fatty");
        Define<TagFishView>().description = "Хищник\n";
        Define<TagFatty>().delta = 1;
        Define<TagSizes>().possibleSizes = new List<FishSize>()
        {
            FishSize.Medium
        };
    }
}

public class TagCloneFrontFish : EntityComponentDefinition
{
    public int delta;
}

public class TagFatty : EntityComponentDefinition
{
    public int delta = 1;
}

public class FattyInteraction : BaseInteraction//, IOnEat
{
    /*public IEnumerator OnEat(FishState obj_state, FishState subj_state)
    {
        if (obj_state.model.Is<TagFatty>(out var tfl))
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
    }*/
}

public interface IOnEat
{
    public IEnumerator OnEat(FishState obj_state, FishState subj_state);
}
