using System.Collections;
using System.Collections.Generic;

public class Level1 : CMSEntity
{
    public Level1()
    {
        Define<TagLevelScript>().toExecute = Script;
        Define<TagDifficulty>().virusPerSet = 1;
        Define<TagLevelContent>().totalPoints = 10;
        Define<TagLevelContent>().startSet = E.Id<EasySet>();
        //Define<TagLevelContent>().badFishSpawns[5] = E.Id<FattyFish>();
    }


    IEnumerator Script()
    {
        yield break;
    }
}