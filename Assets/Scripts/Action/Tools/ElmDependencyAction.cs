using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.Data.Mapper;
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

    bool[] required = null;

    Type stringType = typeof(String);

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
                int selectedId = comboAdapter.GetSelectedId();

                if (selectedId != Convert.ToInt32(values[i]))
                {
                    Activate(false);
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
                    Activate(false);
                    return;
                }
            }
        }
        Activate(true);
    }

    private void Activate(bool on)
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
                outputs[i].gameObject.SetActive(bOn);
            }
            return;
        }

        if (enable != null && enable.Length > 0)
        {
            for (int i = 0; i < outputs.Length; i++)
                outputs[i].enabled = on == enable[i];
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
            }
            return;
        }
    }
}
