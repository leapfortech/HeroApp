using System;

public class CardTransaction
{
    public int Id { get; set; }
    public int CardId { get; set; }
    public String Reference { get; set; }
    public String TransactionCode { get; set; }
    public double Amount { get; set; }
    public String ApprovalCode { get; set; }
    public String DeclinedCode { get; set; }
    public String DeclinedMotive { get; set; }
    public String CancellationReference { get; set; }
    public String CancellationTransactionCode { get; set; }
    public String CancellationStatus { get; set; }
    public int Status { get; set; }


    public CardTransaction()
    {
    }

    public CardTransaction(int id, int cardId, String reference, String transactionCode, double amount, String approvalCode, String declinedCode, String declinedMotive,
                           String cancellationReference, String cancellationTransactionCode, String cancellationStatus, int status)
    {
        Id = id;
        CardId = cardId;
        Reference = reference;
        TransactionCode = transactionCode;
        Amount = amount;
        ApprovalCode = approvalCode;
        DeclinedCode = declinedCode;
        DeclinedMotive = declinedMotive;
        CancellationReference = cancellationReference;
        CancellationTransactionCode = cancellationTransactionCode;
        CancellationStatus = cancellationStatus;
        Status = status;
    }
}
