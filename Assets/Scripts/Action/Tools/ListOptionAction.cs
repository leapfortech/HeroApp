using System.Collections.Generic;
using System;
using UnityEngine;

using Leap.UI.Dialog;
using Leap.UI.Extensions;
using Leap.Data.Collections;
using Leap.UI.Elements;

using Sirenix.OdinInspector;

public class ListOptionAction : MonoBehaviour
{
    [Title("Parameters")]
    [SerializeField]
    ComboAdapter cmbOption = null;

    [SerializeField]
    int maxOptions = 10;

    [Title("Data")]
    [SerializeField]
    ListScroller lstOption = null;

    [SerializeField]
    ValueList vllOptionType = null;
    [SerializeField]
    ValueList vllOption = null;

    public void Clear()
    {
        lstOption.Clear();
        vllOption.ClearRecords();
    }

    public void AddRecord()
    {
        if (cmbOption.Combo.IsEmpty())
            return;

        long selectedOptionId = cmbOption.GetSelectedId();

        if (vllOption.RecordCount >= maxOptions)
        {
            ChoiceDialog.Instance.Error("No se pueden ingresar más de " + maxOptions + " valores.");
            return;
        }

        String vllName = vllOption.FindRecordCellString(selectedOptionId, "Name");
        
        if (vllName != null)
        {
            ChoiceDialog.Instance.Error("<b>" + vllName + "</b> ya está en la lista.");
            return;
        }

        vllOption.AddRecord(Convert.ToInt32(selectedOptionId), cmbOption.Combo.Text);

        Display();
        cmbOption.Clear();
    }

    public void RemoveRecord(int recordIdx)
    {
        vllOption.RemoveRecord(recordIdx);

        Display();
    }

    public void AddRecords(long[] optionIds)
    {
        if (vllOptionType == null)
            return;

        if (optionIds == null || optionIds.Length == 0)
            return;

        for (int i = 0; i < optionIds.Length; i++)
        {
            long optionId = optionIds[i];

            String optionName = vllOptionType?.FindRecordCellString(optionId, "Name");
            if (optionName == null)
                continue;

            vllOption.AddRecord((int)optionId, optionName);
        }

        Display();
    }

    public void Display()
    {
        lstOption.Clear();

        for (int i = 0; i < vllOption.RecordCount; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(1, true);
            scrollerValue.SetText(0, vllOption.GetRecordCellString(i, "Name"));

            lstOption.AddValue(scrollerValue);
        }

        lstOption.ApplyValues();
    }
}
