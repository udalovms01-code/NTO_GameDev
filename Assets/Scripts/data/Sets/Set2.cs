using System.Collections;
using System.Collections.Generic;

public class Set2 : CMSEntity
{
    public Set2()
    {
        Define<TagSetDefinition>().fishOnTheBoardCount = 6;

        Define<TagSetDefinition>().fish_probabilities[E.Id<FattyFish>()] = 0.15f;
        Define<TagSetDefinition>().fish_probabilities[E.Id<PiranhaFish>()] = 0.15f;
        Define<TagSetDefinition>().fish_probabilities[E.Id<ScreamerFish>()] = 0.15f;
        Define<TagSetDefinition>().fish_probabilities[E.Id<LionFish>()] = 0.15f;
        Define<TagSetDefinition>().fish_probabilities[E.Id<MinnowFish>()] = 0.2f;
        Define<TagSetDefinition>().fish_probabilities[E.Id<ZanderFish>()] = 0.2f;
    }
}