using System;
using TMPro;
using UnityEngine;

[Serializable]
public class TargetSpecification
{
    public int value;

    public bool Matches(InteractiveObject arg0)
    {
        return arg0.state.fishValue == value;
    }
}

public class FishHolder : MonoBehaviour
{
    public TargetSpecification spec;

    //public GameObject highlight;
    //public TMP_Text target;

    public int maxHold;
    
    FishZone zone;

    void Start()
    {
        zone = GetComponent<FishZone>();
        //G.main.OnReleaseDrag += TryClaim;
    }

    void TryClaim(InteractiveObject arg0)
    {
        /*if (IsDiceEntrapped(arg0))
        {
            zone.Claim(arg0);
        }*/
    }

    public bool IsDiceEntrapped(InteractiveObject arg0)
    {
        if (arg0 == null)
            return false;

        var isInRange = Vector2.Distance(arg0.transform.position, transform.position) < 1f;
        if (!isInRange)
            return false;

        if (!spec.Matches(arg0))
            return false;
        
        return true;
    }

    void Update()
    {
        //target.text = spec.value.ToString();
        //highlight.SetActive(IsDiceEntrapped(G.drag_dice));
        if (G.drag_dice != null)
        {
            zone.ChangePos(G.drag_dice);
        }
    }
}