using System.Collections;
using System.Collections.Generic;

public class Level5 : CMSEntity
{
    public Level5()
    {
        Define<TagLevelScript>().toExecute = Script;
        Define<TagDifficulty>().virusPerSet = 1;
        Define<TagLevelContent>().totalPoints = 7;
        Define<TagLevelContent>().startSet = E.Id<Set2>();
    }

    IEnumerator Script()
    {
        yield break;
    }
}