using System.Collections;
using System.Collections.Generic;
/*[System.Serializable]
public class FishSpawnData : EntityComponentDefinition
{
    public string fish;
    public float percentage;
    
    public FishSpawnData(string fish_id, float new_percentage)
    {
        fish = fish_id;
        percentage = new_percentage;
    }
}*/

public class EasySet : CMSEntity
{
    public EasySet()
    {
        Define<TagSetDefinition>().fishOnTheBoardCount = 6;

        Define<TagSetDefinition>().fish_probabilities[E.Id<GuppyFish>()] = 0.35f;
        Define<TagSetDefinition>().fish_probabilities[E.Id<BreamFish>()] = 0.35f;
        Define<TagSetDefinition>().fish_probabilities[E.Id<MinnowFish>()] = 0.15f;
        Define<TagSetDefinition>().fish_probabilities[E.Id<FattyFish>()] = 0.15f;

        /*Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish_id:E.Id<GuppyFish>(),
            new_percentage:0.35f
            ));
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish_id:E.Id<BreamFish>(),
            new_percentage:0.35f
            ));
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish_id:E.Id<MinnowFish>(),
            new_percentage:0.15f
        ));
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish_id:E.Id<FattyFish>(),
            new_percentage:0.15f
        ));*/
    }
}