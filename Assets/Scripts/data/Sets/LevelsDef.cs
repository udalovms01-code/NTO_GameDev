using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

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
    public int fishOnTheBoardCount = 6;

    public Dictionary<string, float> fish_probabilities = new Dictionary<string, float>();
    //public List<FishSpawnData> datas = new List<FishSpawnData>();
}



public class TagDifficulty : EntityComponentDefinition
{
    public int virusPerSet = 1;
    public int cutPerTurn = -1;
}

public class TagLevelScript : EntityComponentDefinition
{
    public Func<IEnumerator> toExecute;
}

public class TagLevelContent : EntityComponentDefinition
{
    public int totalPoints;
    public float levelTime;
    public string startSet;
    public Dictionary<int, string> badFishSpawns = new Dictionary<int, string>();
    public Dictionary<int, string> setChanges = new Dictionary<int, string>();
}

