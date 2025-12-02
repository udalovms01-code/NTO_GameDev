using System.Collections;
using System.Collections.Generic;

public class EasySet : CMSEntity
{
    public EasySet()
    {
        Define<TagSetDefinition>().fishCount = 6;
        
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish:E.Id<BasicFish>(), 
            percentage:0.35f
            ));
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish:E.Id<FudgeFish>(), 
            percentage:0.35f
            ));
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish:E.Id<MinnowFish>(), 
            percentage:0.15f
        ));
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish:E.Id<FattyFish>(), 
            percentage:0.15f
        ));
    }
}