using System;
using System.Collections.Generic;

public class InvestmentPrepaidFull : InvestmentFull
{
    public int Id { get; set; }
    public int ProductPrepaidId { get; set; }
    public int Status { get; set; }

    public InvestmentPrepaidFull()
    {
    }

    public InvestmentPrepaidFull(int id, int investmentId, int productPrepaidId,
                                int projectId, int productTypeId, int appUserId, DateTime effectiveDate, int developmentTerm, int cpiCount,
                                double totalAmount, double reserveAmount, double dueAmount, double discountRate, double discountAmount, 
                                double balance, DateTime? completionDate, String docuSignReference, int investmentMotiveId, String boardComment, 
                                int investmentStatusId, int status, List<InvestmentBankPayment> investmentBankPayments,
                                List<InvestmentInstallmentInfo> investmentInstallmentInfos)
                                : base(investmentId, projectId, productTypeId, appUserId, effectiveDate, developmentTerm, cpiCount,
                                       totalAmount, reserveAmount, dueAmount, discountRate, discountAmount,
                                       balance, completionDate, docuSignReference, investmentMotiveId, boardComment, investmentStatusId,
                                       investmentBankPayments, investmentInstallmentInfos)
    {
        Id = id;
        ProductPrepaidId = productPrepaidId;
        Status = status;
    }

    public void Set(InvestmentPrepaidFull investmentPrepaidFull)
    {
        Id = investmentPrepaidFull.Id;
        ProductPrepaidId = investmentPrepaidFull.ProductPrepaidId;
        Status = investmentPrepaidFull.Status;

        InvestmentId = investmentPrepaidFull.InvestmentId;
        ProjectId = investmentPrepaidFull.ProjectId;
        ProductTypeId = investmentPrepaidFull.ProductTypeId;
        AppUserId = investmentPrepaidFull.AppUserId;
        EffectiveDate = investmentPrepaidFull.EffectiveDate;
        DevelopmentTerm = investmentPrepaidFull.DevelopmentTerm;
        CpiCount = investmentPrepaidFull.CpiCount;
        TotalAmount = investmentPrepaidFull.TotalAmount;
        ReserveAmount = investmentPrepaidFull.ReserveAmount;
        DueAmount = investmentPrepaidFull.DueAmount;
        DiscountRate = investmentPrepaidFull.DiscountRate;
        DiscountAmount = investmentPrepaidFull.DiscountAmount;
        Balance = investmentPrepaidFull.Balance;
        CompletionDate = investmentPrepaidFull.CompletionDate;
        DocuSignReference = investmentPrepaidFull.DocuSignReference;
        InvestmentMotiveId = investmentPrepaidFull.InvestmentMotiveId;
        BoardComment = investmentPrepaidFull.BoardComment;
        InvestmentStatusId = investmentPrepaidFull.InvestmentStatusId;
        InvestmentBankPayments = investmentPrepaidFull.InvestmentBankPayments;
        InvestmentInstallmentInfos = investmentPrepaidFull.InvestmentInstallmentInfos;
    }
}