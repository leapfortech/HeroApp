using UnityEngine;

using Leap.Core.Tools;
using Leap.UI.Elements;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;

public class MultilineChangeAction : MonoBehaviour
{
    [Title("Parameters")]
    [SerializeField]
    InputMultiline imlText = null;

    [Title("Events")]
    [SerializeField]
    UnityFloatEvent onImlChanged = null;

    RectTransform imgRect = null;
    RectTransform txtRect = null;
    float initHeight = 0f;

    private void Awake()
    {
        imgRect = imlText.GetComponent<RectTransform>();
        txtRect = imgRect.GetChild(0).GetChild(3).GetComponent<RectTransform>();
        initHeight = imgRect.sizeDelta.y;
    }

    public void Display()
    {
        float deltaY = 0f;
        if (imlText.TextHeight == 0f)
            deltaY = initHeight - imgRect.sizeDelta.y;
        else
            deltaY = imlText.TextHeight - (imgRect.sizeDelta.y + txtRect.sizeDelta.y);
        
        imgRect.sizeDelta = new Vector2(imgRect.sizeDelta.x, imgRect.sizeDelta.y + deltaY);

        onImlChanged?.Invoke(deltaY);
    }
}
