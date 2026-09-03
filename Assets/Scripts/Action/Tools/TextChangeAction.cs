using System;
using UnityEngine;

using Leap.Core.Tools;
using Leap.Data.Collections;
using Leap.UI.Dialog;
using Leap.UI.Extensions;
using Leap.UI.Elements;

using Sirenix.OdinInspector;

public class TextChangeAction : MonoBehaviour
{
    [Title("Parameters")]
    [SerializeField]
    RectTransform imgRect = null;
    [SerializeField]
    Text txtText = null;

    [Title("Events")]
    [SerializeField]
    UnityFloatEvent onTxtChanged = null;


    public void Display()
    {
        RectTransform txtRect = txtText.GetComponent<RectTransform>();
        float deltaY = txtText.TextHeight - (imgRect.sizeDelta.y + txtRect.sizeDelta.y);
        imgRect.sizeDelta = new Vector2(imgRect.sizeDelta.x, imgRect.sizeDelta.y + deltaY);

        onTxtChanged?.Invoke(deltaY);
    }
}
