using System.Collections;
using System.Collections.Generic;

public class Level3 : CMSEntity
{
    public Level3()
    {
        Define<TagLevelScript>().toExecute = Script;
        Define<TagDifficulty>().virusPerSet = 1;
        Define<TagLevelContent>().totalPoints = 7;
        Define<TagLevelContent>().startSet = E.Id<Set1>();
    }

    IEnumerator Script()
    {
        yield break;
    }
}