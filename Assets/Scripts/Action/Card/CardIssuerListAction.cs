using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.Data.Collections;

using Sirenix.OdinInspector;

public class CardIssuerListAction : MonoBehaviour
{
    [Space]
    [SerializeField]
    ListScroller lstCardIssuers = null;

    [Title("Data")]
    [SerializeField]
    ValueList vllCardBrand = null;

    public void FillCardIssuers()
    {
        if (lstCardIssuers.ValuesCount > 0)
            return;

        lstCardIssuers.Clear();

        ListScrollerValue scrollerValue;
        for (int i = 0; i < vllCardBrand.RecordCount; i++)
        {
            scrollerValue = new ListScrollerValue(2, true);
            scrollerValue.SetText(0, (String)vllCardBrand[i]["Name"]);
            scrollerValue.SetSprite(1, (Sprite)vllCardBrand[i]["Brand"]);

            lstCardIssuers.AddValue(scrollerValue);
        }

        lstCardIssuers.ApplyValues();
    }
}
