using System.Collections;
using UnityEngine;

public class TutorBreamFish : FishBase
{
    public TutorBreamFish()
    {
        Define<TagFishView>().name = "bream";
        Define<TagFishView>().sprite = SpriteUtil.Load("fishes", "bream");
        Define<TagFishView>().dead_sprite = SpriteUtil.Load("dead_fishes", "bream");
        Define<TagVirusedForm>().sprite = SpriteUtil.Load("virused_fishes", "bream");
        Define<TagFishView>().description = "Получает +1, если напротив Лещ";
        Define<TagTutorBream>();
        Define<TagBream>().delta = 1;
    }
}

public class TagTutorBream : EntityComponentDefinition
{
}

public class TagTutorBreamInteraction : BaseInteraction, IOnChangePos
{
    public IEnumerator OnChangePos(FishState fish)
    {
        if (fish.model.Is<TagTutorBream>(out var tfl))
        {
            var nextFish = G.main.field.FrontFish(fish.view);
            if (nextFish != null)
            {
                if (nextFish.state.model.Is<TagFishView>(out var fv))
                {
                    if (fv.name == "bream")
                    {
                        G.main.TutorFlag = true;
                    }
                }
            }
        }
        yield break;
    }
}

public interface IOnChangePos
{
    public IEnumerator OnChangePos(FishState fish);
}