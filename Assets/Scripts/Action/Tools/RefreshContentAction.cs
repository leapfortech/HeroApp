using System;
using UnityEngine;

using Leap.UI.Elements;

using Sirenix.OdinInspector;

public class RefreshContentAction : MonoBehaviour
{
    [Space]
    [Title("ScrollView")]
    [SerializeField]
    UnityEngine.UI.ScrollRect scrollRect = null;
    [SerializeField]
    RectTransform content =  null;

    public void ChangeHeight(float deltaY)
    {
        content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y + deltaY);
    }

    public void ResetPosition()
    {
        scrollRect.verticalNormalizedPosition = 1f;
    }
}