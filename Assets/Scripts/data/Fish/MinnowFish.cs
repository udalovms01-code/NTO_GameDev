using System.Collections;
using UnityEngine;

public class MinnowFish : FishBase
{
    public MinnowFish()
    {
        Define<TagFishView>().name = "minnow";
        Define<TagFishView>().sprite = SpriteUtil.Load("fishes", "minnow");
        Define<TagFishView>().dead_sprite = SpriteUtil.Load("dead_fishes", "minnow");
        Define<TagVirusedForm>().sprite = SpriteUtil.Load("virused_fishes", "minnow");
        Define<TagFishView>().description = "Повышает качество на 1, если стоит первый или последний";
        Define<TagFudgeIfFirstOrLastDice>().delta = 1;
    }
}

public class TagFudgeIfFirstOrLastDice : EntityComponentDefinition
{
    public int delta;
}

public class FudgeDiceInteraction : BaseInteraction, IOnEndTurn
{
    public IEnumerator OnEndTurn(FishState fish)
    {
        if (fish.model.Is<TagFudgeIfFirstOrLastDice>(out var tfl))
        {
            InteractiveObject view = fish.view;
            if (G.main.field.IsLast(view) 
                || G.main.field.ZoneIndex(view) == 0)
            {
                view.SetValue(fish.fishValue + tfl.delta);
                view.spriteAnimator.Punch();
                yield return new WaitForSeconds(0.25f * G.CorGameplayConfig.animationMultiplier);
            }
        }
    }
}
