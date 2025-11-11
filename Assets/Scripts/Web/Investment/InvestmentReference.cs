using System;

public class InvestmentReference
{
    public int Id { get; set; }
    public int AppUserId { get; set; }
    public int InvestmentId { get; set; }
    public int ReferenceTypeId { get; set; }
    public String Name { get; set; }
    public int PhoneCountryId { get; set; }
    public String Phone { get; set; }
    public String Email { get; set; }
    public String Description { get; set; }
    public int Status { get; set; }


    public InvestmentReference()
    {
    }

    public InvestmentReference(int id, int appUserId, int investmentId, int referenceTypeId, String name, int phoneCountryId, String phone, 
                               String email, String description, int status)
    {
        Id = id;
        AppUserId = appUserId;
        InvestmentId = investmentId;
        ReferenceTypeId = referenceTypeId;
        Name = name;
        PhoneCountryId = phoneCountryId;
        Phone = phone;
        Email = email;
        Description = description;
        Status = status;
    }
}