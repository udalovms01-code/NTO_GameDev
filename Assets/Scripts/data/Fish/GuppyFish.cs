using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GuppyFish : FishBase
{
    public GuppyFish()
    {
        Define<TagFishView>().name = "guppy";
        Define<TagFishView>().sprite = SpriteUtil.Load("fishes", "guppy");
        Define<TagFishView>().dead_sprite = SpriteUtil.Load("dead_fishes", "guppy");
        Define<TagFishView>().description = "Разворачивает рыбу перед собой";
        Define<TagRotator>();
        Define<TagCannotBeVirused>();
    }
}

public class TagRotator : EntityComponentDefinition
{
    
}

public class RotatorInteraction : BaseInteraction, IOnEndTurn
{
    public IEnumerator OnEndTurn(FishState fish)
    {
        if (fish.model.Is<TagRotator>(out var tfl))
        {
            var nextFish = G.main.field.FrontFish(fish.view);
            if (nextFish != null)
            {
                nextFish.state.direction = nextFish.state.direction == FishDirection.Right
                    ? FishDirection.Left
                    : FishDirection.Right;
                nextFish.SetState(nextFish.state);
                nextFish.spriteAnimator.Punch();
                yield return new WaitForSeconds(0.25f * G.CorGameplayConfig.animationSpeed);
            }
        }
    }
}
