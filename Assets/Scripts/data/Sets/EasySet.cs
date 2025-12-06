using System.Collections;
using System.Collections.Generic;

public class EasySet : CMSEntity
{
    public EasySet()
    {
        Define<TagSetDefinition>().fishOnTheBoardCount = 6;
        
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish_id:E.Id<BasicFish>(), 
            new_percentage:0.35f
            ));
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish_id:E.Id<FudgeFish>(), 
            new_percentage:0.35f
            ));
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish_id:E.Id<MinnowFish>(), 
            new_percentage:0.15f
        ));
        Define<TagSetDefinition>().datas.Add(new FishSpawnData(
            fish_id:E.Id<FattyFish>(), 
            new_percentage:0.15f
        ));
    }
}