using System;

using Sirenix.OdinInspector;

public class Income
{
    public int Id { get; set; } = -1;
    public int InvestmentId { get; set; }
    [ShowInInspector]
    public int IncomeTypeId { get; set; }
    [ShowInInspector]
    public String Detail { get; set; }
    public int Status { get; set; }


    public Income()
    {
    }

    public Income(int id, int investmentId, int incomeTypeId, String detail, int status)
    {
        Id = id;
        InvestmentId = investmentId;
        IncomeTypeId = incomeTypeId;
        Detail = detail;
        Status = status;
    }
}
