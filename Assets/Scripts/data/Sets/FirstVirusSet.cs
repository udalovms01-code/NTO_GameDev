using System.Collections;
using System.Collections.Generic;

public class FirstVirusSet : CMSEntity
{
    public FirstVirusSet()
    {
        Define<TagSetDefinition>().fishOnTheBoardCount = 7;
        
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish_id:E.Id<BasicFish>(), 
            new_percentage:0.25f
            ));
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish_id:E.Id<FudgeFish>(), 
            new_percentage:0.25f
            ));
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish_id:E.Id<MinnowFish>(), 
            new_percentage:0.25f
        ));
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish_id:E.Id<FattyFish>(), 
            new_percentage:0.25f
        ));
    }
}