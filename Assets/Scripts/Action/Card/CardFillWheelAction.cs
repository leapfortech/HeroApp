using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Extensions;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;

public class CardFillWheelAction : MonoBehaviour
{
    [Title("Expiration Date")]
    [SerializeField]
    DataMapper dtmExpirationMonthValueList = null;

    [SerializeField]
    DataMapper dtmExpirationMonthWheelScroller = null;

    [SerializeField]
    DataMapper dtmExpirationYearValueList = null;

    [SerializeField]
    DataMapper dtmExpirationYearWheelScroller = null;

    [SerializeField, Space]
    MultiWheel mwlExpirationDate = null;

    const float waitDelay = 0.01f;

    public void FillExpirationDate()
    {
        Invoke(nameof(FillExpirationDateDelay), waitDelay);
    }

    private void FillExpirationDateDelay()
    {
        if (mwlExpirationDate.GetValuesCount(0) == 0)
        {
            dtmExpirationMonthWheelScroller.PopulateBuiltInList(dtmExpirationMonthValueList.BuildBuiltInList<String>());
            dtmExpirationYearWheelScroller.PopulateBuiltInList(dtmExpirationYearValueList.BuildBuiltInList<String>());
        }

        mwlExpirationDate.SetSelectedIndexes(5, 3);
    }
}