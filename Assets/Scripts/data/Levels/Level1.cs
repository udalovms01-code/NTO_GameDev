using System.Collections;
using System.Collections.Generic;

public class Level1 : CMSEntity
{
    public Level1()
    {
        Define<TagLevelScript>().toExecute = Script;
        Define<TagDifficulty>().virusPerSet = 1;
        Define<TagLevelTarget>().poits = 7;
    }

    IEnumerator Script()
    {
        yield break;
    }
}