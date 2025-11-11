using System;
public class Pep
{
    public int Id { get; set; }
    public String InstitutionName { get; set; }
    public int InstitutionCountryId { get; set; }
    public String JobTitle { get; set; }
    public int WealthOriginTypeId { get; set; }
    public String WealthDescription { get; set; }


    public Pep()
    {
    }

    public Pep(int id, String institutionName, int institutionCountryId, String jobTitle, int wealthOriginTypeId, String wealthDescription)
    {
        Id = id;
        InstitutionName = institutionName;
        InstitutionCountryId = institutionCountryId;
        JobTitle = jobTitle;
        WealthOriginTypeId = wealthOriginTypeId;
        WealthDescription = wealthDescription;
    }
}
