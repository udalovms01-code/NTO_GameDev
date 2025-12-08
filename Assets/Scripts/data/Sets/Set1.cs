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

public class Set1 : CMSEntity
{
    public Set1()
    {
        Define<TagSetDefinition>().fishOnTheBoardCount = 6;

        Define<TagSetDefinition>().fish_probabilities[E.Id<FattyFish>()] = 0.15f;
        Define<TagSetDefinition>().fish_probabilities[E.Id<PiranhaFish>()] = 0.15f;
        Define<TagSetDefinition>().fish_probabilities[E.Id<ScreamerFish>()] = 0.15f;
        Define<TagSetDefinition>().fish_probabilities[E.Id<LionFish>()] = 0.15f;
        Define<TagSetDefinition>().fish_probabilities[E.Id<GuppyFish>()] = 0.15f;
        Define<TagSetDefinition>().fish_probabilities[E.Id<BreamFish>()] = 0.25f;
        Define<TagSetDefinition>().fish_probabilities[E.Id<MinnowFish>()] = 0.1f;
    }
}