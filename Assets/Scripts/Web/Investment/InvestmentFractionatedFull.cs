using System;
using System.Collections.Generic;


public class InvestmentFractionatedFull : InvestmentFull
{
    public int Id { get; set; }
    public int ProductFractionatedId { get; set; }
    public double Amount { get; set; }
    public int InstallmentCount { get; set; }
    public int Status { get; set; }

    public InvestmentFractionatedFull()
    {
    }

    public InvestmentFractionatedFull(int id, int investmentId, int productFractionatedId,
                                      int projectId, int productTypeId, int appUserId, DateTime effectiveDate, int developmentTerm, 
                                      int cpiCount, double totalAmount, double reserveAmount, double dueAmount, double discountRate,
                                      double discountAmount, double balance, DateTime? completionDate, 
                                      String docuSignReference, int investmentMotiveId, String boardComment, int investmentStatusId,
                                      double amount, int intallmentCount, int status,
                                      List<InvestmentBankPayment> investmentBankPayments,
                                      List<InvestmentInstallmentInfo> investmentInstallmentInfos)
                                      : base(investmentId, projectId, productTypeId, appUserId, effectiveDate, developmentTerm, cpiCount,
                                             totalAmount, reserveAmount, dueAmount, discountRate, discountAmount,
                                             balance, completionDate, docuSignReference, investmentMotiveId, boardComment, investmentStatusId,
                                             investmentBankPayments, investmentInstallmentInfos)
    {
        Id = id;
        ProductFractionatedId = productFractionatedId;
        Amount = amount;
        InstallmentCount = intallmentCount;
        Status = status;
    }

    public void Set(InvestmentFractionatedFull investmentFractionatedFull)
    {
        Id = investmentFractionatedFull.Id;
        ProductFractionatedId = investmentFractionatedFull.ProductFractionatedId;
        Amount = investmentFractionatedFull.Amount;
        InstallmentCount = investmentFractionatedFull.InstallmentCount;
        Status = investmentFractionatedFull.Status;

        InvestmentId = investmentFractionatedFull.InvestmentId;
        ProjectId = investmentFractionatedFull.ProjectId;
        ProductTypeId = investmentFractionatedFull.ProductTypeId;
        AppUserId = investmentFractionatedFull.AppUserId;
        EffectiveDate = investmentFractionatedFull.EffectiveDate;
        DevelopmentTerm = investmentFractionatedFull.DevelopmentTerm;
        CpiCount = investmentFractionatedFull.CpiCount;
        TotalAmount = investmentFractionatedFull.TotalAmount;
        ReserveAmount = investmentFractionatedFull.ReserveAmount;
        DueAmount = investmentFractionatedFull.DueAmount;
        DiscountRate = investmentFractionatedFull.DiscountRate;
        DiscountAmount = investmentFractionatedFull.DiscountAmount;
        Balance = investmentFractionatedFull.Balance;
        CompletionDate = investmentFractionatedFull.CompletionDate;
        DocuSignReference = investmentFractionatedFull.DocuSignReference;
        InvestmentMotiveId = investmentFractionatedFull.InvestmentMotiveId;
        BoardComment = investmentFractionatedFull.BoardComment;
        InvestmentStatusId = investmentFractionatedFull.InvestmentStatusId;
        InvestmentBankPayments = investmentFractionatedFull.InvestmentBankPayments;
        InvestmentInstallmentInfos = investmentFractionatedFull.InvestmentInstallmentInfos;
    }
}