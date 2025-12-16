using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableSmoothDamp : MonoBehaviour, IClickable
{
    public MoveableBase moveable;
    public bool isDragging = false; 
    public float smoothSpeed = 10f;

    private Camera mainCamera;

    Vector2 origin;
    Vector3 offset;// = new Vector3(0, 1.28f, 0);
    private Vector3 screenPoint;

    private void Start()
    {
        isDragging = false;
        mainCamera = Camera.main;
        moveable.targetPosition = transform.localPosition; 
    }

    public void OnMouseDown()
    {
        if(G.main.field.canDrag)
        {
            G.main.StartDrag(this);

            isDragging = true;
        }    
    }

    void OnMouseDrag() {
        if (G.main.field.canDrag)
        {
            if (isDragging)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane tablePlane = new Plane(Vector3.up, -G.main.tableHeith);
                if (tablePlane.Raycast(ray, out float distance))
                {
                    Vector3 worldTarget = ray.GetPoint(distance) + offset;


                    //worldTarget += Vector3.up * 0.2f;

                    // Конвертируем в ЛОКАЛЬНЫЕ координаты относительно родителя
                    Vector3 localTarget = transform.parent.InverseTransformPoint(worldTarget);
                    localTarget.x = Math.Min(localTarget.x, G.main.fishFeelConfig.maxX);
                    localTarget.y = Math.Min(localTarget.y, G.main.fishFeelConfig.maxY);
                    localTarget.z = Math.Min(localTarget.z, G.main.fishFeelConfig.maxZ);

                    // Теперь используйте localTarget для движения
                    moveable.targetPosition = localTarget; // Предполагая, что moveable работает с localPosition
                }
            }
        }
    }


    public void OnMouseExit()
    {
    }

    public void OnMouseEnter()
    {
        
    }

    public void OnMouseUp()
    {
        if (G.main.field.canDrag)
        {
            G.main.StopDrag();

            isDragging = false;
            moveable.targetPosition = origin;
        }
            
    }
}