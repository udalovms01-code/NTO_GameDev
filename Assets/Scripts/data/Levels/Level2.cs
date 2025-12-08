using System.Collections;
using System.Collections.Generic;

public class Level2 : CMSEntity
{
    public Level2()
    {
        Define<TagLevelScript>().toExecute = Script;
        Define<TagDifficulty>().virusPerSet = 1;
        Define<TagLevelContent>().totalPoints = 1;
        Define<TagLevelContent>().startSet = E.Id<Set1>();
    }

    IEnumerator Script()
    {
        yield break;
    }
}