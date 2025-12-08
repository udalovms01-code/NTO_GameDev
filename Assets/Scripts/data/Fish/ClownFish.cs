using System.Collections;
using UnityEngine;

public class ClownFish : FishBase
{
    public ClownFish()
    {
        Define<TagFishView>().name = "clown";
        Define<TagFishView>().sprite = SpriteUtil.Load("clown", "clown1");
        Define<TagFishView>().dead_sprite = SpriteUtil.Load("clown", "clown1");
        Define<TagFishView>().description = "";
    }
}
