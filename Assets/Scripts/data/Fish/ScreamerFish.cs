using System.Collections;
using UnityEngine;

public class ScreamerFish : FishBase
{
    public ScreamerFish()
    {
        Define<TagFishView>().name = "screamer";
        Define<TagFishView>().sprite = SpriteUtil.Load("fishes", "screamer");
        Define<TagFishView>().dead_sprite = SpriteUtil.Load("dead_fishes", "screamer");
        Define<TagStartFishValue>().value = 0;
        Define<TagFishView>().description = "-1 рыбе напротив";
        Define<TagScreamer>().delta = -1;
        Define<TagNegative>();
    }
}

public class TagScreamer : EntityComponentDefinition
{
    public int delta = -1;
}

public class FudgeScreamerInteraction : BaseInteraction, IOnEndTurn
{
    public IEnumerator OnEndTurn(FishState fish)
    {
        if (fish.model.Is<TagScreamer>(out var tfl))
        {
            InteractiveObject next = G.main.field.FrontFish(fish.view);
            
            if (next != null)
            {
                next.SetValue(next.state.fishValue + tfl.delta);
                next.spriteAnimator.Punch();
                yield return new WaitForSeconds(0.25f * G.CorGameplayConfig.animationMultiplier);
            }
        }
    }
}