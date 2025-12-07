using System.Collections;
using UnityEngine;

public class ZanderFish : FishBase
{
    public ZanderFish()
    {
        Define<TagFishView>().name = "zander";
        Define<TagFishView>().sprite = SpriteUtil.Load("fishes", "zander");
        Define<TagFishView>().dead_sprite = SpriteUtil.Load("dead_fishes", "zander");
        Define<TagFishView>().description = "+30 если вокруг хищники";
        Define<TagZander>().delta = 3;
    }
}

public class TagZander : EntityComponentDefinition
{
    public int delta = 3;
}

public class FudgeZanderInteraction : BaseInteraction, IOnEndTurn
{
    public IEnumerator OnEndTurn(FishState fish)
    {
        if (fish.model.Is<TagZander>(out var tfl))
        {
            InteractiveObject view = fish.view;
            InteractiveObject next = G.main.field.FrontFish(fish.view);
            InteractiveObject back = G.main.field.BackFish(fish.view);
            
            if (next != null && back != null)
            {
                if (next.state.size == FishSize.Medium && back.state.size == FishSize.Medium)
                {
                    view.SetValue(fish.fishValue + tfl.delta);
                    view.spriteAnimator.Punch();
                    yield return new WaitForSeconds(0.25f * G.CorGameplayConfig.animationMultiplier);
                }
            }
        }
    }
}