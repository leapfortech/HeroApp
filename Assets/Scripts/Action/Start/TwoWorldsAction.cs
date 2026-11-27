using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;

using Sirenix.OdinInspector;

public class TwoWorldsAction : MonoBehaviour
{
    [Space, SerializeField]
    ListScroller lstTwoWorlds = null;

    [Space, SerializeField]
    TwoWorldsOption[] options = null;

    public void CreateTwoWorlds()
    {
        //if (lstTwoWorlds.ValuesCount > 0)
        //    return;

        lstTwoWorlds.ClearValues();
        for (int i = 0; i < options.Length; i++)
        {
            ListScrollerValue listValue = new ListScrollerValue(lstTwoWorlds.ListItem, false);

            listValue.SetSprite(0, options[i].Sprite);
            listValue.SetText(1, options[i].Title);
            listValue.SetText(2, options[i].DescLine1);
            listValue.SetText(3, options[i].DescLine2);

            lstTwoWorlds.AddValue(listValue);
        }
        lstTwoWorlds.ApplyValues();
    }

    public void GoToPage(int idx)
    {
        PageManager.Instance.ChangePage(options[idx].Page);
    }
}