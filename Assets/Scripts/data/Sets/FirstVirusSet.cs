using System.Collections;
using System.Collections.Generic;

public class FirstVirusSet : CMSEntity
{
    public FirstVirusSet()
    {
        Define<TagSetDefinition>().fishCount = 7;
        
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish:E.Id<BasicFish>(), 
            percentage:0.25f
            ));
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish:E.Id<FudgeFish>(), 
            percentage:0.25f
            ));
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish:E.Id<MinnowFish>(), 
            percentage:0.25f
        ));
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish:E.Id<FattyFish>(), 
            percentage:0.25f
        ));
    }
}