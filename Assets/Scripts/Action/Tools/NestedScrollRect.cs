using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NestedScrollRect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ScrollRect parentScrollRect;
    public ScrollRect currentScrollRect;

    private bool routeToParent = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        float x = Mathf.Abs(eventData.delta.x);
        float y = Mathf.Abs(eventData.delta.y);

        routeToParent = x > y;

        if (routeToParent)
            parentScrollRect.OnBeginDrag(eventData);
        else
            currentScrollRect.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (routeToParent)
            parentScrollRect.OnDrag(eventData);
        else
            currentScrollRect.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (routeToParent)
            parentScrollRect.OnEndDrag(eventData);
        else
            currentScrollRect.OnEndDrag(eventData);

        routeToParent = false;
    }
}
