using System;

public class ProductRequest
{
    public ProjectFull ProjectFull { get; set; }
    public int ProductType { get; set; }
    public int CpiCount { get; set; }
    public double DueAmount { get; set; }
    public double LoanInstallmentAmount { get; set; }
    public int InvestmentTerm { get; set; }
    public double ReserveAmount { get; set; }
    public double DiscountRate { get; set; }
    public double DiscountAmount { get; set; }


    public ProductRequest()
    {
    }

    public ProductRequest(ProjectFull projectFull, int productType, int cpiCount, double dueAmount, double loanInstallmentAmount,
                          int investmentTerm, double reserveAmount, double discountRate, double discountAmount)
    {
        ProjectFull = projectFull;
        ProductType = productType;
        CpiCount = cpiCount;
        DueAmount = dueAmount;
        LoanInstallmentAmount = loanInstallmentAmount;
        InvestmentTerm = investmentTerm;
        ReserveAmount = reserveAmount;
        DiscountRate = discountRate;
        DiscountAmount = discountAmount;
    }
}