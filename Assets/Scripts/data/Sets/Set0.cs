using System.Collections;
using System.Collections.Generic;

public class Set0 : CMSEntity
{
    public Set0()
    {
        Define<TagSetDefinition>().fishOnTheBoardCount = 6;

        Define<TagSetDefinition>().fish_probabilities[E.Id<FattyFish>()] = 0.25f;
        Define<TagSetDefinition>().fish_probabilities[E.Id<LionFish>()] = 0.25f;
        Define<TagSetDefinition>().fish_probabilities[E.Id<GuppyFish>()] = 0.25f;
        Define<TagSetDefinition>().fish_probabilities[E.Id<BreamFish>()] = 0.25f;
    }
}