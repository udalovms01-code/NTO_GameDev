using System;
using System.Collections;
using System.Collections.Generic;

/*public class TagListSets : EntityComponentDefinition
{
    public List<FishSet> all = new List<FishSet>();
}

public class FishSet : EntityComponentDefinition
{
    public List<FishSpawnData> spawnDatas;
    public FishSet()
    {
        List<FishSpawnData> spawnDatas = new List<FishSpawnData>();
    }
}*/

public class TagSetDefinition : EntityComponentDefinition
{
    public int fishCount;
    public List<FishSpawnData> datas = new List<FishSpawnData>();
}

public class TagDifficulty : EntityComponentDefinition
{
    public int virusPerSet = 1;
}

public class TagLevelScript : EntityComponentDefinition
{
    public Func<IEnumerator> toExecute;
}

public class TagLevelTarget : EntityComponentDefinition
{
    public int poits;
}

