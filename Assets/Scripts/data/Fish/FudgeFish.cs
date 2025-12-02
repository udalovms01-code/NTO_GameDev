using System.Collections;
using UnityEngine;

public class FudgeFish : FishBase
{
    public FudgeFish()
    {
        Define<TagFishView>().name = "Лещ";
        Define<TagFishView>().sprite = SpriteUtil.Load("fishes", "bream");
        Define<TagFishView>().dead_sprite = SpriteUtil.Load("dead_fishes", "bream");
        Define<TagVirusedForm>().sprite = SpriteUtil.Load("virused_fishes", "bream");
        Define<TagDescription>().loc = "Повышает качество соседнего леща на 1";
        Define<TagFudgeNextDice>().delta = 1;
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
                nextFish.Punch();
                yield return new WaitForSeconds(0.25f);
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