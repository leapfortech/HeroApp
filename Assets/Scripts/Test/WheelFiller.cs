using System;
using UnityEngine;

using Leap.Core.Debug;
using Leap.UI.Elements;

using Sirenix.OdinInspector;

public class WheelFiller : MonoBehaviour
{
    [Space, ListDrawerSettings(ShowFoldout = false)]
    [SerializeField]
    WheelScroller[] wheelScrollers = null;

    [Space, ListDrawerSettings(ShowFoldout = false)]
    [SerializeField]
    int[] itemCounts = null;

    [Space, ListDrawerSettings(ShowFoldout = false)]
    [SerializeField]
    int[] itemFirsts = null;

    [Space, ListDrawerSettings(ShowFoldout = false)]
    [SerializeField]
    int[] itemSteps = null;

    [Space, ListDrawerSettings(ShowFoldout = false)]
    [SerializeField]
    String[] itemPrefix = null;

    [Space]
    [SerializeField]
    int valueCount = 1;

    readonly String[] months = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };

    public void Fill()
    {
        if (wheelScrollers.Length > 0)
        {
            if (wheelScrollers[0].gameObject.activeSelf)
            {
                wheelScrollers[0].ClearValues();

                for (int i = 0; i < Mathf.Max(itemCounts[0], 12); i++)
                {
                    WheelScrollerValue wheelValue = new WheelScrollerValue(valueCount);
                    for (int k = 0; k < valueCount; k++)
                        wheelValue.SetText(k, months[i]);

                    wheelScrollers[0].AddValue(wheelValue);
                }

                wheelScrollers[0].ApplyValues();
            }
        }

        if (wheelScrollers.Length > 1)
        {
            if (wheelScrollers[1].gameObject.activeSelf)
            {
                wheelScrollers[1].ClearValues();

                for (int i = 0; i < itemCounts[1]; i++)
                {
                    WheelScrollerValue wheelValue = new WheelScrollerValue(valueCount);
                    for (int k = 0; k < valueCount; k++)
                        wheelValue.SetText(k, (i + 1).ToString());

                    wheelScrollers[1].AddValue(wheelValue);
                }

                wheelScrollers[1].ApplyValues();
            }
        }

        if (wheelScrollers.Length > 2)
        {
            if (wheelScrollers[2].gameObject.activeSelf)
            {
                wheelScrollers[2].ClearValues();

                for (int i = 2022 - itemCounts[2]; i <= 2022; i++)
                {
                    WheelScrollerValue wheelValue = new WheelScrollerValue(valueCount);
                    for (int k = 0; k < valueCount; k++)
                        wheelValue.SetText(k, i.ToString());

                    wheelScrollers[2].AddValue(wheelValue);
                }

                wheelScrollers[2].ApplyValues();
                //wheelScrollers[2].SetValueIndex(itemCounts[2]);
            }
        }

        for (int s = 3; s < wheelScrollers.Length; s++)
        {
            if (wheelScrollers[s].gameObject.activeSelf)
            {
                wheelScrollers[s].ClearValues();

                for (int i = 0; i < itemCounts[s]; i++)
                {
                    WheelScrollerValue wheelValue = new WheelScrollerValue(valueCount);
                    for (int k = 0; k < valueCount; k++)
                        wheelValue.SetText(k, itemPrefix[i] + i);

                    wheelScrollers[s].AddValue(wheelValue);
                }

                wheelScrollers[s].ApplyValues();
            }
        }
    }

    public void FillValues()
    {
        for (int s = 0; s < wheelScrollers.Length; s++)
        {
            wheelScrollers[s].ClearValues();

            int n = itemFirsts[s] + itemCounts[s];
            for (int i = itemFirsts[s]; i < n; i += itemSteps[s])
            {
                WheelScrollerValue wheelValue = new WheelScrollerValue(1);
                wheelValue.SetText(0, itemPrefix[s] + i);

                wheelScrollers[s].AddValue(wheelValue);
            }

            wheelScrollers[s].ApplyValues();
        }
    }

    public void DisplayListIndex(int index)
    {
        DebugManager.Instance.DebugInfo("List Index : " + index);
    }
}
