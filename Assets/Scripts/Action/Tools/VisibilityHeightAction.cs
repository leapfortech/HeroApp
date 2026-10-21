using UnityEngine;

using Leap.Core.Tools;
using Leap.UI.Elements;

using Sirenix.OdinInspector;

public class VisibilityHeightAction : MonoBehaviour
{
    [Title("Parameters")]
    [SerializeField]
    RectTransform trfRect = null;

    [SerializeField]
    GameObject gobElement = null;

    [SerializeField]
    Toggle tglAction = null;

    [Title("Events")]
    [SerializeField]
    UnityFloatEvent onVisibilityChanged = null;

    float elementHeight = 0f;
    bool previousVisible = false;
    bool initialized = false;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (initialized)
            return;

        if (trfRect == null || gobElement == null)
            return;

        elementHeight = gobElement.GetComponent<RectTransform>().sizeDelta.y;
        previousVisible = gobElement.activeSelf;
        initialized = true;
    }


    public void Refresh()
    {
        Initialize();

        if (!initialized)
            return;

        if (tglAction != null)
            gobElement.SetActive(tglAction.Checked);

        bool visible = gobElement.activeSelf;

        if (previousVisible == visible)
            return;

        float deltaY = elementHeight * (visible ? 1f : -1f);

        trfRect.sizeDelta = new Vector2(trfRect.sizeDelta.x, trfRect.sizeDelta.y + deltaY);

        previousVisible = visible;

        onVisibilityChanged?.Invoke(deltaY);
    }
}