using System.Collections;
using UnityEngine;

public class LionFish : FishBase
{
    public LionFish()
    {
        Define<TagFishView>().name = "lion";
        Define<TagFishView>().sprite = SpriteUtil.Load("fishes", "lion");
        Define<TagFishView>().dead_sprite = SpriteUtil.Load("dead_fishes", "lion");
        Define<TagStartFishValue>().value = -1;
        Define<TagFishView>().description = "Яд с начала игры";
    }
}