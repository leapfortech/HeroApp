using System;

using Sirenix.OdinInspector;

public class Bank
{
    public int Id { get; set; } = -1;
    [ShowInInspector]
    public String Name { get; set; }
    [ShowInInspector]
    public int AccountTypeId { get; set; }
    [ShowInInspector]
    public int AccountCurrencyId { get; set; }
    [ShowInInspector]
    public String AccountNumber { get; set; }
    public int Status { get; set; }


    public Bank()
    {
    }

    public Bank(int id, String name, int accountTypeId, int accountCurrencyId, String accountNumber, int status)
    {
        Id = id;
        Name = name;
        AccountTypeId = accountTypeId;
        AccountCurrencyId = accountCurrencyId;
        AccountNumber = accountNumber;
        Status = status;
    }
}
