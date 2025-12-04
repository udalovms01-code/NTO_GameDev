using UnityEngine.EventSystems;

public interface IClickable
{
    public void OnMouseDown();
    public void OnMouseUp();
    public void OnMouseEnter();
    public void OnMouseExit();
}