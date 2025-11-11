using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;

using Sirenix.OdinInspector;

public class ValidateElementAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Action")]
    [SerializeField]
    Button btnValidate = null;

    [Title("Page")]
    [SerializeField]
    Page nextPage = null;

    [Title("Event")]
    [SerializeField]
    UnityEvent onValidated = null;

    private void Start()
    {
        btnValidate?.AddAction(Validate);
    }

    public void Clear()
    {
        for (int i = 0; i < elementValues.Length; i++)
            elementValues[i].Clear();
    }

    public void Validate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        onValidated?.Invoke();

        if (nextPage != null)
            PageManager.Instance.ChangePage(nextPage);
    }
}