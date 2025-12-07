using System.Collections;
using UnityEngine;

public class BreamFish : FishBase
{
    public BreamFish()
    {
        Define<TagFishView>().name = "bream";
        Define<TagFishView>().sprite = SpriteUtil.Load("fishes", "bream");
        Define<TagFishView>().dead_sprite = SpriteUtil.Load("dead_fishes", "bream");
        Define<TagVirusedForm>().sprite = SpriteUtil.Load("virused_fishes", "bream");
        Define<TagFishView>().description = "Получает +1, если напротив Лещ";
        Define<TagBream>().delta = 1;
    }
}

public class TagBream : EntityComponentDefinition
{
    public int delta;
}

public class TagBreamInteraction : BaseInteraction, IOnEndTurn
{
    public IEnumerator OnEndTurn(FishState fish)
    {
        if (fish.model.Is<TagBream>(out var tfl))
        {
            var nextFish = G.main.field.FrontFish(fish.view);
            if (nextFish != null)
            {
                if (nextFish.state.model.Is<TagFishView>(out var fv))
                {
                    if (fv.name == "bream")
                    {
                        fish.view.SetValue(fish.fishValue + tfl.delta);
                        fish.view.spriteAnimator.Punch();
                        yield return new WaitForSeconds(0.25f * G.CorGameplayConfig.animationMultiplier);
                    }
                }
            }
        }
    }
}

public class TagFudgeNextDice : EntityComponentDefinition
{
    public int delta;
}

public class FudgeNextDiceInteraction : BaseInteraction, IOnEndTurn
{
    public IEnumerator OnEndTurn(FishState fish)
    {
        if (fish.model.Is<TagFudgeNextDice>(out var tfl))
        {
            var nextFish = G.main.field.FrontFish(fish.view);
            if (nextFish != null)
            {
                nextFish.SetValue(nextFish.state.fishValue + tfl.delta);
                nextFish.spriteAnimator.Punch();
                yield return new WaitForSeconds(0.25f * G.CorGameplayConfig.animationMultiplier);
            }
        }
    }
}

public interface IOnPlace
{
    public IEnumerator OnPlace(FishState fish);
}

public interface IOnEndTurn
{
    public IEnumerator OnEndTurn(FishState fish);
}