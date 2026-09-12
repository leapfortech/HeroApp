using UnityEngine;

using Leap.Core.Tools;
using Leap.UI.Elements;

using Sirenix.OdinInspector;

public class VisibilityChangeAction : MonoBehaviour
{
    [Title("Parameters")]
    [SerializeField]
    RectTransform trfRect = null;
    [Space, SerializeField]
    GameObject[] elements = null;

    [Title("Events")]
    [SerializeField]
    UnityFloatEvent onVisibilityChanged = null;

    float initHeight = 0f;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (initHeight != 0f)
            return;
        
        initHeight = trfRect.sizeDelta.y;
    
        if (elements[0].activeSelf == false)
        {
            elements[0].SetActive(true);
            Display(false);
        }
    }

    public void Display(bool visible)
    {
        Initialize();

        if ((trfRect.sizeDelta.y > 0f) == visible)
            return;
            
        float deltaY = initHeight * (visible ? 1f : -1f);

        for (int i = 0; i < elements.Length; i++)
            elements[i].SetActive(visible);

        trfRect.sizeDelta = new Vector2(trfRect.sizeDelta.x, trfRect.sizeDelta.y + deltaY);

        onVisibilityChanged?.Invoke(deltaY);
    }
}
