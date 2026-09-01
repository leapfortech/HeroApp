using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class NestedInputField : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ScrollRect parentScrollRect;
    public TMP_InputField currentInputField;
    public float deltaX = 10f;

    private bool routeToParent = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        float x = Mathf.Abs(eventData.delta.x);

        routeToParent = x < deltaX;

        if (routeToParent)
            parentScrollRect.OnBeginDrag(eventData);
        else
            currentInputField.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (routeToParent)
            parentScrollRect.OnDrag(eventData);
        else
            currentInputField.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (routeToParent)
            parentScrollRect.OnEndDrag(eventData);
        else
            currentInputField.OnEndDrag(eventData);

        routeToParent = false;
    }
}
