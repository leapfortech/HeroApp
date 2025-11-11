using UnityEngine;

using Leap.UI.Elements;
using Sirenix.OdinInspector;
using Leap.Data.Collections;
using Leap.Data.Mapper;
using System.Collections.Generic;

public class FillListBankAccountHpbAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ListScroller lstAccountHpb = null;

    [Title("Values")]
    [SerializeField]
    ValueList vllAccountType = null;
    [SerializeField]
    ValueList vllCurrency = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmAccountHpbVLL = null;

    public void FillBankAccountVoucher()
    {
        lstAccountHpb.ClearValues();
        
        List<BankAccountHpb> accountsHpb = dtmAccountHpbVLL.BuildClassList<BankAccountHpb>();

        for (int i = 0; i < accountsHpb.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(5, true);

            scrollerValue.SetText(0, accountsHpb[i].BankName);
            scrollerValue.SetText(1, accountsHpb[i].Number);
            scrollerValue.SetText(2, vllAccountType.FindRecordCellString(accountsHpb[i].TypeId, "Name"));
            scrollerValue.SetText(3, vllCurrency.FindRecordCellString(accountsHpb[i].CurrencyId, "Name"));
            scrollerValue.SetSprite(4, accountsHpb[i].Logo);

            lstAccountHpb.AddValue(scrollerValue);
        }
        lstAccountHpb.ApplyValues();
    }
}
