using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

public class EconomicsInfo
{
    [ShowInInspector]
    public Economics Economics { get; set; }
    
    [ShowInInspector]
    public List<Income> Incomes { get; set; }
    public String[] DocIncomes { get; set; }

    public EconomicsInfo()
    {
    }

    public EconomicsInfo(Economics economics, List<Income> incomes, String[] docIncomes)
    {
        Economics = economics;
        Incomes = incomes;
        DocIncomes = docIncomes;
    }
}
