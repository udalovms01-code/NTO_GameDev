using System.Collections;
using System.Collections.Generic;

public class Level2 : CMSEntity
{
    public Level2()
    {
        Define<TagLevelScript>().toExecute = Script;
        Define<TagDifficulty>().virusPerSet = 1;
        Define<TagLevelContent>().totalPoints = 7;
    }

    IEnumerator Script()
    {
        yield break;
    }
}