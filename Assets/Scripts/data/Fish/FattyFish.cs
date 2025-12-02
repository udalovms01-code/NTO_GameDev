using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FattyFish : FishBase
{
    public FattyFish()
    {
        Define<TagFishView>().name = "Рыба-толстопуз";
        Define<TagFishView>().sprite = SpriteUtil.Load("fishes", "fatty");
        Define<TagFishView>().dead_sprite = SpriteUtil.Load("dead_fishes", "fatty");
        Define<TagVirusedForm>().sprite = SpriteUtil.Load("virused_fishes", "fatty");
        Define<TagDescription>().loc = "раздваивает рыбу, перед собой.\n";
        Define<TagCloneFrontFish>().delta = 1;
        Define<TagSizes>().possibleSizes = new List<FishSize>()
        {
            FishSize.Big
        };
    }
}

public class TagCloneFrontFish : EntityComponentDefinition
{
    public int delta;
}

public class CloneFrontInteraction : BaseInteraction, IOnEndTurn
{
    public IEnumerator OnEndTurn(FishState fish)
    {
        if (fish.model.Is<TagCloneFrontFish>(out var tfl))
        {
            InteractiveObject view = fish.view;
            if (G.main.field.IsLast(view) 
                || G.main.field.ZoneIndex(view) == 0)
            {
                view.SetValue(fish.fishValue + tfl.delta);
                view.Punch();
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}
