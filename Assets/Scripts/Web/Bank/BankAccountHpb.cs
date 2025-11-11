using System;

using Sirenix.OdinInspector;
using UnityEngine;

public class BankAccountHpb
{
    [ShowInInspector]
    public String Number { get; set; }
    [ShowInInspector]
    public String BankName { get; set; }
    [ShowInInspector]
    public int TypeId { get; set; }
    [ShowInInspector]
    public int CurrencyId { get; set; }
    public Sprite Logo { get; set; }
    [ShowInInspector]
    public int BankId { get; set; }


    public BankAccountHpb()
    {
    }

    public BankAccountHpb(String bankName, int typeId, int currencyId, String number, Sprite logo, int bankId)
    {
        BankName = bankName;    
        TypeId = typeId;
        CurrencyId = currencyId;
        Number = number;
        Logo = logo;
        BankId = bankId;
    }
}
