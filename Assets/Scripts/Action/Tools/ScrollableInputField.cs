using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ScrollableInputField : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IMoveHandler
{
    public ScrollRect parentScrollRect;
    public TMP_InputField currentInputField;
    public float deltaX = 6f;

    private bool routeToParent = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        float x = Mathf.Abs(eventData.delta.x);

        routeToParent = x < deltaX;

        if (routeToParent)
        {
            parentScrollRect.OnBeginDrag(eventData);
            ClearSelection();
        }
        else
            currentInputField.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (routeToParent)
        {
            parentScrollRect.OnDrag(eventData);
            ClearSelection();
        }
        else
            currentInputField.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (routeToParent)
        {
            parentScrollRect.OnEndDrag(eventData);
            ClearSelection();
        }
        else
            currentInputField.OnEndDrag(eventData);

        routeToParent = false;
    }

    public void OnMove(AxisEventData eventData)
    {
        if (!routeToParent)
            currentInputField.OnMove(eventData);
    }

    private void ClearSelection()
    {
        currentInputField.selectionAnchorPosition = currentInputField.caretPosition;
        currentInputField.selectionFocusPosition = currentInputField.caretPosition;
        currentInputField.stringPosition = currentInputField.caretPosition;
    }
}
