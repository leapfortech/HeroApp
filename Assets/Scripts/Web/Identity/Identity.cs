using System;

using Sirenix.OdinInspector;


public class Identity
{
    public int Id { get; set; }
    public int AppUserId { get; set; }
    [ShowInInspector]
    public String FirstName1 { get; set; }
    [ShowInInspector]
    public String FirstName2 { get; set; }
    [ShowInInspector]
    public String FirstName3 { get; set; }
    [ShowInInspector]
    public String LastName1 { get; set; }
    [ShowInInspector]
    public String LastName2 { get; set; }
    [ShowInInspector]
    public String LastNameMarried { get; set; }
    [ShowInInspector]
    public int GenderId { get; set; }
    [ShowInInspector]
    public DateTime BirthDate { get; set; }
    [ShowInInspector]
    public int BirthCountryId { get; set; }
    [ShowInInspector]
    public int BirthStateId { get; set; }
    [ShowInInspector]
    public int BirthCityId { get; set; }
    [ShowInInspector]
    public String NationalityIds { get; set; }
    [ShowInInspector]
    public int MaritalStatusId { get; set; }
    [ShowInInspector]
    public String DpiCui { get; set; }
    [ShowInInspector]
    public DateTime DpiIssueDate { get; set; }
    [ShowInInspector]
    public DateTime DpiDueDate { get; set; }
    [ShowInInspector]
    public String DpiMrz { get; set; }
    [ShowInInspector]
    public String Occupation { get; set; }
    [ShowInInspector]
    public String Nit { get; set; }
    [ShowInInspector]
    public int PhoneCountryId { get; set; }
    [ShowInInspector]
    public String Phone { get; set; }
    [ShowInInspector]
    public String Email { get; set; }
    public int IsPep { get; set; }
    public int HasPepIdentity { get; set; }
    public int IsCpe { get; set; }
    public int Status { get; set; }


    public Identity()
    {
    }

    public Identity(int id, int appUserId, String firstName1, String firstName2, String firstName3, String lastName1, String lastName2, String lastNameMarried, 
                    int genderId, DateTime birthDate, int birthCountryId, int birthStateId, int birthCityId, String nationalityIds, int maritalStatusId,
                    String dpiCui, DateTime dpiIssueDate, DateTime dpiDueDate, String dpiMrz, String nit, String occupation,
                    int phoneCountryId, String phone, String email, int isPep, int hasPepIdentity, int isCpe, int status)
    {
        Id = id;
        AppUserId = appUserId;
        FirstName1 = firstName1;
        FirstName2 = firstName2;
        FirstName3 = firstName3;
        LastName1 = lastName1;
        LastName2 = lastName2;
        LastNameMarried = lastNameMarried;
        GenderId = genderId;
        BirthDate = birthDate;
        BirthCountryId = birthCountryId;
        BirthStateId = birthStateId;
        BirthCityId = birthCityId;
        NationalityIds = nationalityIds;
        MaritalStatusId = maritalStatusId;
        DpiCui = dpiCui;
        DpiIssueDate = dpiIssueDate;
        DpiDueDate = dpiDueDate;
        DpiMrz = dpiMrz;
        DpiMrz = dpiMrz;
        Nit = nit;
        Occupation = occupation;
        PhoneCountryId = phoneCountryId;
        Phone = phone;
        Email = email;
        IsPep = isPep;
        HasPepIdentity = hasPepIdentity;
        IsCpe = isCpe;
        Status = status;
    }
}
