using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InputFieldScrollForwarder : MonoBehaviour,
    IPointerDownHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    public ScrollRect scrollRect;
    public TMP_InputField inputField;

    private bool scrolling;

    public void OnPointerDown(PointerEventData eventData)
    {
        scrolling = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        scrolling = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);

        if (scrolling)
            scrollRect.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (scrolling)
            scrollRect.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (scrolling)
            scrollRect.OnEndDrag(eventData);

        scrolling = false;
    }
}