using System;

public class InvestmentPaymentInfo
{
    public int InvestmentId { get; set; } = -1;
    public int InvestmentPaymentTypeId { get; set; } = -1;
    public double Amount { get; set; }
    public DateTime DueDate { get; set; }
    public double Balance { get; set; }

    public InvestmentPaymentInfo()
    {
    }

    public InvestmentPaymentInfo(int investmentId, int investmentPaymentTypeId, double amount, DateTime dueDate, double balance)
    {
        InvestmentId = investmentId;
        InvestmentPaymentTypeId = investmentPaymentTypeId;
        Amount = amount;
        DueDate = dueDate;
        Balance = balance;
    }
}
