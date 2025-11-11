using UnityEngine;

using Leap.UI.Elements;

public class ScrolledText : MonoBehaviour
{
    RectTransform trf;
    Text text;

    private void Awake()
    {
        trf = GetComponent<RectTransform>();
        text = trf.GetChild(0).GetComponent<Text>();
    }

    public void DelayScale()
    {
        Invoke(nameof(Scale),0.1f);
    }
    
    public void Scale()
    {
        trf.sizeDelta = new Vector2(trf.sizeDelta.x, text.LinesHeight);
    }
}
