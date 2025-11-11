using System;

public class InvestmentFinanced
{
    public int Id { get; set; }
    public int InvestmentId { get; set; }
    public int ProductFinancedId { get; set; }
    public double AdvAmount { get; set; }
    public int AdvInstallmentTotal { get; set; }
    public double LoanInterestRate { get; set; }
    public int LoanTerm { get; set; }
    public int Status { get; set; }


    public InvestmentFinanced()
    {
    }

    public InvestmentFinanced(int id, int investmentId, int productFinancedId, double advAmount, int advInstallmentTotal,
                              double loanInterestRate, int loanTerm, int status)
    {
        Id = id;
        InvestmentId = investmentId;
        ProductFinancedId = productFinancedId;
        AdvAmount = advAmount;
        AdvInstallmentTotal = advInstallmentTotal;
        LoanInterestRate = loanInterestRate;
        LoanTerm = loanTerm;
        Status = status;
    }
}