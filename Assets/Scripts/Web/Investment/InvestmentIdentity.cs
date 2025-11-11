using System;

public class InvestmentIdentity
{
    public int Id { get; set; }
    public int InvestmentId { get; set; }
    public int IdentityId { get; set; }
    public int InvestmentIdentityTypeId { get; set; }
    public String Relationship { get; set; }
    public double Pourcentage { get; set; }
    public int Status { get; set; }


    public InvestmentIdentity()
    {
    }

    public InvestmentIdentity(int id, int investmentId, int identityId, int investmentIdentityTypeId, String relationship, double pourcentage,
                              int status)
    {
        Id = id;
        InvestmentId = investmentId;
        IdentityId = identityId;
        InvestmentIdentityTypeId = investmentIdentityTypeId;
        Relationship = relationship;
        Pourcentage = pourcentage;
        Status = status;
    }
}