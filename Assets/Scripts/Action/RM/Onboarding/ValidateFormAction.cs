using System;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;

using Sirenix.OdinInspector;

public class ValidateFormAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Action")]
    [SerializeField]
    Button btnValidate = null;

    [Title("Pages")]
    [SerializeField]
    Page nextPage = null;

    [Title("Event")]
    [SerializeField]
    UnityEvent onValidated = null;


    private void Start()
    {
        btnValidate?.AddAction(NameDateValidate);
    }

    private void NameDateValidate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        if (nextPage == null)
            return;

        onValidated?.Invoke();

        PageManager.Instance.ChangePage(nextPage);
    }

    public void Clear()
    {
        for (int i = 0; i < elementValues.Length; i++)
            elementValues[i].Clear();
    }
}