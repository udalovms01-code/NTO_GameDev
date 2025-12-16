
using UnityEngine;

[CreateAssetMenu(fileName = "FishesFeelConfig", menuName = "FishesFeelConfig")]
public class FishFeelConfig : ScriptableObject
{
    public float maxX = 1_000f;
    public float maxY = 1_000f;
    public float maxZ = 1_000f;

    public float dragSpeed = 1f;
}