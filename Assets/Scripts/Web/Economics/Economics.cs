using System;

using Sirenix.OdinInspector;

public class Economics
{
    public int Id { get; set; }
    public int InvestmentId { get; set; }

    [ShowInInspector]
    public int IncomeCurrencyId { get; set; }
    [ShowInInspector]
    public double IncomeAmount { get; set; }
    [ShowInInspector]
    public int ExpensesCurrencyId { get; set; }
    [ShowInInspector]
    public double ExpensesAmount { get; set; }
    public String Activity { get; set; }
    public int Status { get; set; } = -1;


    public Economics()
    {
    }

    public Economics(int id, int investmentId, int incomeCurrencyId, double incomeAmount, int expensesCurrencyId,
                     double expensesAmount, String activity, int status)
    {
        Id = id;
        InvestmentId = investmentId;
        IncomeCurrencyId = incomeCurrencyId;
        IncomeAmount = incomeAmount;
        ExpensesCurrencyId = expensesCurrencyId;
        ExpensesAmount = expensesAmount;
        Activity = activity;
        Status = status;
    }
}
