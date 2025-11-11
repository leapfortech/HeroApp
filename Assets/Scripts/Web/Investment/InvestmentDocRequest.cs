using System;

public class InvestmentDocRequest
{
    public int InvestmentId { get; set; }
    public String[] Docs { get; set; }

    public InvestmentDocRequest()
    {
    }

    public InvestmentDocRequest(int investmentId, String[] docs)
    {
        InvestmentId = investmentId;
        Docs = docs;
    }
}
