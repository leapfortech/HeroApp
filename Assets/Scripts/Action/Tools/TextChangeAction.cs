using UnityEngine;

using Leap.Core.Tools;
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

    RectTransform txtRect = null;
    float initHeight = 0f;

    private void Awake()
    {
        txtRect = txtText.GetComponent<RectTransform>();
        initHeight = imgRect.sizeDelta.y;
    }

    public void Display()
    {
        float deltaY = 0f;
        if (txtText.TextHeight == 0f)
            deltaY = initHeight - imgRect.sizeDelta.y;
        else
            deltaY = txtText.TextHeight - (imgRect.sizeDelta.y + txtRect.sizeDelta.y);
        
        imgRect.sizeDelta = new Vector2(imgRect.sizeDelta.x, imgRect.sizeDelta.y + deltaY);

        onTxtChanged?.Invoke(deltaY);
    }
}
