using System;
using UnityEngine;

public class DragData
{
    public float x;
    public float y;
}

public class HandInput : ManagedBehaviour
{
    /*private IClickable currentHoveredCard;
    void Start()
    {
        GetComponent<DiceZone>().OnClickFish += OnClickDice;
    }

    public void OnClickDice(InteractiveObject dice)
    {
        G.main.TryPlayDice(dice);
    }
    
    void Update() {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(1111);
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit)) {
            IClickable hitCard = hit.collider.GetComponent<IClickable>();
            
            // OnMouseExit для предыдущей карты
            if (currentHoveredCard != null && currentHoveredCard != hitCard) {
                currentHoveredCard.OnMouseExit();
            }
            
            // OnMouseEnter для новой карты
            if (hitCard != null && hitCard != currentHoveredCard) {
                hitCard.OnMouseEnter();
                currentHoveredCard = hitCard;
            }
            
            // Клик
            if (Input.GetMouseButtonDown(0) && hitCard != null) {
                hitCard.OnMouseDown();
            }

            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log(hitCard);
                hitCard.OnMouseUp();
            }
        } else if (currentHoveredCard != null) {
            // Мышь вышла за пределы всех карт
            currentHoveredCard.OnMouseExit();
            currentHoveredCard = null;
            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log(1111);
            }
        }
    }*/

}
