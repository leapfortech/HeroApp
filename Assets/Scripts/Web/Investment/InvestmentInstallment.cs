using System;

public class InvestmentInstallment
{
    public int Id { get; set; }
    public int InvestmentId { get; set; }
    public int InvestmentPaymentTypeId { get; set; }
    public double Amount { get; set; }
    public double DiscountAmount { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime DueDate { get; set; }
    public double Balance { get; set; }
    public DateTime? CompletionDate { get; set; }
    public int Status { get; set; }


    public InvestmentInstallment()
    {
    }

    public InvestmentInstallment(int id, int investmentId, int investmentPaymentTypeId, double amount, double discountAmount, 
                                 DateTime effectiveDate, DateTime dueDate, double balance, DateTime? completionDate, int status)
    {
        Id = id;
        InvestmentId = investmentId;
        InvestmentPaymentTypeId = investmentPaymentTypeId;
        Amount = amount;
        DiscountAmount = discountAmount;
        EffectiveDate = effectiveDate;
        DueDate = dueDate;
        Balance = balance;
        CompletionDate = completionDate;
        Status = status;
    }
}
