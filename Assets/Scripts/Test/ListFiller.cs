using System;
using UnityEngine;

using Leap.Core.Debug;
using Leap.UI.Elements;

using Sirenix.OdinInspector;

public class ListFiller : MonoBehaviour
{
    [SerializeField, Space]
    ListScroller listScroller = null;

    [Space]
    [SerializeField]
    FillType fillType = ListFiller.FillType.Base;

    [SerializeField, ShowIf(nameof(fillType), ListFiller.FillType.Base)]
    int itemCount = 10;

    [SerializeField, ShowIf(nameof(fillType), ListFiller.FillType.Base)]
    Sprite[] sprites = new Sprite[0];

    [Space]
    [SerializeField]
    bool hasToggle = false;

    enum FillType { Base, Months }

    readonly String[] months = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };

    public void Fill()
    {
        listScroller.ClearValues();

        ListScrollerValue listValue;
        if (fillType == ListFiller.FillType.Base)
        {
            int elementCount;
            for (int i = 0; i < itemCount; i++)
            {
                elementCount = listScroller.ListItem.ElementCount;
                listValue = new ListScrollerValue(elementCount, hasToggle);
                for (int k = 0; k < elementCount; k++)
                    if (listScroller.ListItem.IsText(k))
                        listValue.SetText(k, "Itm " + i + " - Txt " + k);
                    else
                        listValue.SetSprite(k, sprites[i % sprites.Length]);

                listScroller.AddValue(listValue);
            }
        }
        else if (fillType == ListFiller.FillType.Months)
        {
            for (int i = 0; i < months.Length; i++)
            {
                listValue = new ListScrollerValue(1, hasToggle);
                listValue.SetText(0, months[i]);

                listScroller.AddValue(listValue);
            }
        }

        listScroller.ApplyValues();
    }

    public void DisplayListIndex(int index)
    {
        DebugManager.Instance.DebugInfo("List Index : " + index);
    }

    int selectedIdx = 0;

    public void SaveSelection(int idx)
    {
        if (idx % 2 == 0)
            selectedIdx = idx;
    }

    public void LoadSelection()
    {
        listScroller.SelectedIndex = selectedIdx;
        if (listScroller.HasToggle)
            listScroller.CheckToggle(selectedIdx);
    }
}
