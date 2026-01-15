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
    ValueList vllOption = null;


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

    private void Display()
    {
        lstOption.Clear();

        for (int i = 0; i < vllOption.RecordCount; i++)
        {
            String name = vllOption.GetRecordCellString(i, "Name");

            ListScrollerValue scrollerValue = new ListScrollerValue(2, true);
            scrollerValue.SetText(0, name);

            lstOption.AddValue(scrollerValue);
        }

        lstOption.ApplyValues();
    }
}
