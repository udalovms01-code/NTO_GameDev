using System.Collections;
using System.Collections.Generic;

public class Level6 : CMSEntity
{
    public Level6()
    {
        Define<TagLevelScript>().toExecute = Script;
        Define<TagDifficulty>().virusPerSet = 1;
        Define<TagLevelContent>().totalPoints = 7;
        Define<TagLevelContent>().startSet = E.Id<Set3>();
    }

    IEnumerator Script()
    {
        yield break;
    }
}