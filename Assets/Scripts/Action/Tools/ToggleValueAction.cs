using System;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;

public class ToggleValueAction : MonoBehaviour
{
    [Title("Toggle")]
    [SerializeField]
    ToggleGroup[] tggs = null;
    [Space, SerializeField]
    String value = null;

    [Title("Action")]
    [SerializeField]
    Button btnValidate = null;

    [Title("Event")]
    [SerializeField]
    UnityEvent onValidated = null;


    private void Start()
    {
        btnValidate?.AddAction(Validate);
    }

    public void Clear()
    {
        for (int i = 0; i < tggs.Length; i++)
            tggs[i].Clear();
    }

    public void Validate()
    {
        if (tggs == null || tggs.Length == 0)
            return;

        for (int i = 0; i < tggs.Length; i++)
        {
            if (String.Equals(tggs[i].Value, value, StringComparison.Ordinal))
            {
                onValidated?.Invoke();
                return;
            }
        }

        ChoiceDialog.Instance.Error("Es necesario que coloques al menos un contacto");
    }
}