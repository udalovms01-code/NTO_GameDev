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
    Vector3 offset;
    private Vector3 screenPoint;

    private void Start()
    {
        isDragging = false;
        mainCamera = Camera.main; 
        moveable.targetPosition = transform.position; 
    }

    public void OnMouseDown()
    {
        G.main.StartDrag(this);
        
        isDragging = true;
        
        /*screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane tablePlane = new Plane(Vector3.up, 0); // Плоскость стола Y=0
        if (tablePlane.Raycast(ray, out float distance)) {
            offset = transform.position - ray.GetPoint(distance);
        }*/
    }

    void OnMouseDrag() {
        if (isDragging) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane tablePlane = new Plane(Vector3.up, 0);
            if (tablePlane.Raycast(ray, out float distance))
            {
                Vector3 newTarget = ray.GetPoint(distance) + offset;
                newTarget += Vector3.up * 0.2f;
                //transform.position = Vector3.Lerp(transform.position, newPos, smoothSpeed * Time.deltaTime);
                moveable.targetPosition = newTarget;
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
        G.main.StopDrag();
        
        isDragging = false; 
        moveable.targetPosition = origin;
            
    }
}