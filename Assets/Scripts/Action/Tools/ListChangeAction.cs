using System;
using UnityEngine;

using Leap.Core.Tools;
using Leap.Data.Collections;
using Leap.UI.Dialog;
using Leap.UI.Extensions;
using Leap.UI.Elements;

using Sirenix.OdinInspector;

public class ListChangeAction : MonoBehaviour
{
    [Title("Parameters")]
    [SerializeField]
    Image imgBkg = null;
    
    [Space, SerializeField]
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

    [Title("Events")]
    [SerializeField]
    UnityFloatEvent onLstChanged = null;

    public void Clear()
    {
        lstOption.Clear();
        vllOption.ClearRecords();

        SendDeltaY();
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
            cmbOption.Clear();
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
        Clear();
        
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
            ListScrollerValue scrollerValue = new ListScrollerValue(lstOption.ListItem, true);
            scrollerValue.SetText(0, vllOption.GetRecordCellString(i, "Name"));

            lstOption.AddValue(scrollerValue);
        }

        lstOption.ApplyValues();

        SendDeltaY();
    }

    private void SendDeltaY()
    {
        RectTransform imgRect = imgBkg.GetComponent<RectTransform>();
        RectTransform lstRect = lstOption.GetComponent<RectTransform>();
        RectTransform itmRect = lstOption.ListItem.GetComponent<RectTransform>();
        float deltaY = vllOption.RecordCount * itmRect.sizeDelta.y - (imgRect.sizeDelta.y + lstRect.sizeDelta.y);
        imgRect.sizeDelta = new Vector2(imgRect.sizeDelta.x, imgRect.sizeDelta.y + deltaY);

        onLstChanged?.Invoke(deltaY);
    }
}
