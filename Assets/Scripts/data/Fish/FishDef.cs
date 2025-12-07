using System.Collections.Generic;
using UnityEngine;

public abstract class FishBase : CMSEntity
{
    public FishBase()
    {
        Define<TagPrefab>().prefab = "prefab/fish_view".Load<InteractiveObject>();
        Define<TagFishView>().name = "???";
        Define<TagStartFishValue>().value = 0;
        Define<TagFishView>().description = "???";
        Define<TagRarity>().rarity = FishRarity.COMMON;
        Define<TagTint>().color = Color.white;
        Define<TagSizes>().possibleSizes = new List<FishSize>()
        {
            FishSize.Small
        };
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
    public string description;
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
