using System.Collections.Generic;
using UnityEngine;

public abstract class FishBase : CMSEntity
{
    public FishBase()
    {
        Define<TagPrefab>().prefab = "prefab/fish_view".Load<InteractiveObject>();
        Define<TagFishView>().name = "???";
        Define<TagStartFishValue>().value = 1;
        Define<TagDescription>().loc = "???";
        Define<TagRarity>().rarity = FishRarity.COMMON;
        Define<TagTint>().color = Color.white;
        Define<TagSizes>().possibleSizes = new List<FishSize>()
        {
            FishSize.Small
        };
    }
}
public class BasicFish : FishBase
{
    public BasicFish()
    {
        Define<TagFishView>().name = "Гуппи";
        Define<TagFishView>().sprite = SpriteUtil.Load("fishes", "guppy");
        Define<TagFishView>().dead_sprite = SpriteUtil.Load("dead_fishes", "guppy");
        Define<TagDescription>().loc = "Ничего не делает";
        Define<TagCannotBeVirused>();
    }
}

public class TagVirusedForm : EntityComponentDefinition
{
    public Sprite sprite;
    public int negativeValue = -2;
}
public class TagFishView : EntityComponentDefinition
{
    public string name;
    public Sprite sprite;
    public Sprite dead_sprite;
}

public class TagCannotBeVirused : EntityComponentDefinition
{
    
}

public enum FishRarity
{
    COMMON,
    UNCOMMON,
    RARE
}

public enum FishDirection {Left, Right};
public enum FishSize {Zero, Small, Medium, Big, Giant}

public class TagSizes : EntityComponentDefinition
{
    public List<FishSize> possibleSizes = new List<FishSize>();
}
public class TagTint : EntityComponentDefinition
{
    public Color color;
}

public class TagDescription : EntityComponentDefinition
{
    public string loc;
}

public class TagRarity : EntityComponentDefinition
{
    public FishRarity rarity;
}
public class TagPrefab : EntityComponentDefinition
{
    public InteractiveObject prefab;
}

public class TagStartFishValue : EntityComponentDefinition
{
    public int value;
}
