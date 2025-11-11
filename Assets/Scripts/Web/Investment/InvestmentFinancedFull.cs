using System;
using System.Collections.Generic;

public class InvestmentFinancedFull : InvestmentFull
{
    public int Id { get; set; }
    public int ProductFinancedId { get; set; }
    public double AdvAmount { get; set; }
    public int AdvInstallmentTotal { get; set; }
    public double LoanInterestRate { get; set; }
    public int LoanTerm { get; set; }
    public int Status { get; set; }

    public InvestmentFinancedFull()
    {
    }

    public InvestmentFinancedFull(int id, int investmentId, int productFinancedId,
                                     int projectId, int productTypeId, int appUserId, DateTime effectiveDate, int developmentTerm, int cpiCount,
                                     double reserveAmount, double totalAmount, double dueAmount, double discountRate, double discountAmount,
                                     double balance, DateTime? completionDate, String docuSignReference, int investmentMotiveId, String boardComment, 
                                     int investmentStatusId, double advAmount, int advInstallmentTotal, double loanInterestRate, int loanTerm, int status,
                                     List<InvestmentBankPayment> investmentBankPayments, List<InvestmentInstallmentInfo> investmentInstallmentInfos)
                                     : base(investmentId, projectId, productTypeId, appUserId, effectiveDate, developmentTerm, cpiCount,
                                            totalAmount, reserveAmount, dueAmount, discountRate, discountAmount,
                                            balance, completionDate, docuSignReference, investmentMotiveId, boardComment, investmentStatusId,
                                            investmentBankPayments, investmentInstallmentInfos)
    {
        Id = id;
        ProductFinancedId = productFinancedId;
        AdvAmount = advAmount;
        AdvInstallmentTotal = advInstallmentTotal;
        LoanInterestRate = loanInterestRate;
        LoanTerm = loanTerm;
        Status = status;
    }

    public void Set(InvestmentFinancedFull investmentFinancedFull)
    {
        Id = investmentFinancedFull.Id;
        ProductFinancedId = investmentFinancedFull.ProductFinancedId;
        AdvAmount = investmentFinancedFull.AdvAmount;
        AdvInstallmentTotal = investmentFinancedFull.AdvInstallmentTotal;
        LoanInterestRate = investmentFinancedFull.LoanInterestRate;
        LoanTerm = investmentFinancedFull.LoanTerm;
        Status = investmentFinancedFull.Status;

        InvestmentId = investmentFinancedFull.InvestmentId;
        ProjectId = investmentFinancedFull.ProjectId;
        ProductTypeId = investmentFinancedFull.ProductTypeId;
        AppUserId = investmentFinancedFull.AppUserId;
        EffectiveDate = investmentFinancedFull.EffectiveDate;
        DevelopmentTerm = investmentFinancedFull.DevelopmentTerm;
        CpiCount = investmentFinancedFull.CpiCount;
        TotalAmount = investmentFinancedFull.TotalAmount;
        ReserveAmount = investmentFinancedFull.ReserveAmount;
        DueAmount = investmentFinancedFull.DueAmount;
        DiscountRate = investmentFinancedFull.DiscountRate;
        DiscountAmount = investmentFinancedFull.DiscountAmount;
        Balance = investmentFinancedFull.Balance;
        CompletionDate = investmentFinancedFull.CompletionDate;
        DocuSignReference = investmentFinancedFull.DocuSignReference;
        InvestmentMotiveId = investmentFinancedFull.InvestmentMotiveId;
        BoardComment = investmentFinancedFull.BoardComment;
        InvestmentStatusId = investmentFinancedFull.InvestmentStatusId;
        InvestmentBankPayments = investmentFinancedFull.InvestmentBankPayments;
        InvestmentInstallmentInfos = investmentFinancedFull.InvestmentInstallmentInfos;
    }
}