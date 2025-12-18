using System.Collections;
using System.Collections.Generic;

public class Level4 : CMSEntity
{
    public Level4()
    {
        Define<TagLevelScript>().toExecute = Script;
        Define<TagDifficulty>().virusPerSet = 1;
        Define<TagLevelContent>().totalPoints = 6;
        Define<TagLevelContent>().startSet = E.Id<Set1>();
        Define<TagLevelContent>().levelTime = 60f;
    }

    IEnumerator Script()
    {
        yield break;
    }
}