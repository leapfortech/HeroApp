using System;

using Sirenix.OdinInspector;

public class Card
{
    public int Id { get; set; }
    public int AppUserId { get; set; }
    [ShowInInspector]
    public int TypeId { get; set; }
    [ShowInInspector]
    public String Number { get; set; }
    public int Digits { get; set; }
    public DateTime ExpirationDate { get; set; }
    [ShowInInspector]
    private String _ExpirationDate { get { return $"{ExpirationDate:MM/dd/yyyy}"; } }
    [ShowInInspector]
    public String Holder { get; set; }
    [ShowInInspector]
    public String CSToken { get; set; }
    public int Status { get; set; }


    public Card()
    {
    }

    public Card(int id, int appUserId, String csToken, int typeId, String number, int digits, DateTime expirationDate, String holder, int status)
    {
        Id = id;
        AppUserId = appUserId;
        CSToken = csToken;
        TypeId = typeId;
        Number = number;
        Digits = digits;
        ExpirationDate = expirationDate;
        Holder = holder;
        Status = status;
    }
}
