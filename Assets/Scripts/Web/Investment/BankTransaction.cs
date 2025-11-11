using System;

public class BankTransaction
{
    public int Id { get; set; }
    public int AppUserId { get; set; }
    public int AccountHpbId { get; set; }
    public int TransactionTypeId { get; set; }
    public double Amount { get; set; }
    public String Number { get; set; }
    public DateTime SendDateTime { get; set; }
    public String ApprovalCode { get; set; }
    public int Status { get; set; }


    public BankTransaction()
    {
    }

    public BankTransaction(int id, int appUserId, int accountHpbId, int transactionTypeId, double amount, String number, DateTime sendDateTime,
                           String approvalCode, int status)
    {
        Id = id;
        AppUserId = appUserId;
        AccountHpbId = accountHpbId;
        TransactionTypeId = transactionTypeId;
        Amount = amount;
        Number = number;
        SendDateTime = sendDateTime;
        ApprovalCode = approvalCode;
        Status = status;
    }
}
