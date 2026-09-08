using System;
using UnityEngine;

using Leap.Core.Tools;
using Leap.Data.Mapper;
using Leap.UI.Elements;
using Leap.UI.Extensions;

using Sirenix.OdinInspector;

public class ElmDependencyAction : MonoBehaviour
{
    [Title("Clear")]
    [SerializeField]
    bool clearSlaves = false;

    [Title("Masters")]
    [SerializeField]
    ElementValue[] inputs = null;

    [SerializeField]
    String[] values = null;

    // TODO
    //[SerializeField]
    //int[] valuesIdx = null;

    [Title("Slaves")]
    [SerializeField]
    ElementValue[] outputs = null;

    [SerializeField]
    bool[] active = null;

    [SerializeField]
    bool[] enable = null;

    [SerializeField]
    bool[] interactable = null;

    [Title("Events")]
    [SerializeField]
    UnityBoolEvent[] onVisibilityChanged = null;

    bool[] required = null;

    void Initialize()
    {
        if (required != null)
            return;

        required = new bool[outputs.Length];
        for (int i = 0; i < required.Length; i++)
            required[i] = outputs[i].Required;
    }

    public void Activate()
    {
        //Debug.Log("Element : " + element);
        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i] is Combo)
            {
                ComboAdapter comboAdapter = inputs[i].GetComponent<ComboAdapter>();
                long selectedId = comboAdapter.GetSelectedId();

                if (selectedId != Convert.ToInt64(values[i]))
                {
                    ApplyActive(false);
                    return;
                }
            }
            else
            {
                String value = inputs[i].GetValue<String>();
                if (value == null)
                    value = "";
                
                if (value != values[i])
                {
                    ApplyActive(false);
                    return;
                }
            }
        }
        ApplyActive(true);
    }

    private void ApplyActive(bool on)
    {
        Initialize();

        if (active != null && active.Length > 0)
        {
            bool bOn;
            for (int i = 0; i < outputs.Length; i++)
            {
                bOn = on == active[i];
                if (!bOn)
                    outputs[i].Clear();
                outputs[i].Required = bOn && required[i];

                Activate(i, bOn);
            }
            return;
        }

        if (enable != null && enable.Length > 0)
        {
            for (int i = 0; i < outputs.Length; i++)
                Enable(i, on == enable[i]);
            return;
        }

        if (interactable != null && interactable.Length > 0)
        {
            for (int i = 0; i < outputs.Length; i++)
            {
                if (clearSlaves)
                    outputs[i].Clear();

                if (outputs[i] is Combo)
                    ((Combo)outputs[i]).Interactable = on == interactable[i];
                else if (outputs[i] is InputField)
                    ((InputField)outputs[i]).Interactable = on == interactable[i];

                outputs[i].Required = on == interactable[i];
                Interact(i, on);
            }
            return;
        }
    }

    private void Activate(int idx, bool active)
    {
        outputs[idx].gameObject.SetActive(active);

        if (idx >= onVisibilityChanged.Length)
            return;
        
        onVisibilityChanged[idx]?.Invoke(active);
    }

    private void Enable(int idx, bool enabled)
    {
        outputs[idx].enabled = enabled;

        if (idx >= onVisibilityChanged.Length)
            return;

        onVisibilityChanged[idx]?.Invoke(enabled);
    }

    private void Interact(int idx, bool interactable)
    {
        if (idx >= onVisibilityChanged.Length)
            return;

        onVisibilityChanged[idx]?.Invoke(interactable);
    }
}
