using System;

using Sirenix.OdinInspector;


public class Identity
{
    public long Id { get; set; } = -1;
    [ShowInInspector]
    public String FirstName1 { get; set; } = null;
    [ShowInInspector]
    public String FirstName2 { get; set; } = null;
    [ShowInInspector]
    public String LastName1 { get; set; } = null;
    [ShowInInspector]
    public String LastName2 { get; set; } = null;
    [ShowInInspector]
    public long GenderId { get; set; } = -1;
    [ShowInInspector]
    public DateTime BirthDate { get; set; }
    [ShowInInspector]
    public long BirthCountryId { get; set; } = -1;
    [ShowInInspector]
    public long BirthStateId { get; set; } = -1;
    [ShowInInspector]
    public long BirthCityId { get; set; } = -1;
    [ShowInInspector]
    public long PhoneCountryId { get; set; } = -1;
    [ShowInInspector]
    public String Phone { get; set; }
    [ShowInInspector]
    public String Email { get; set; }
    public int Status { get; set; }


    public Identity()
    {
    }

    public Identity(long id, String firstName1, String firstName2, String lastName1, String lastName2,
                    long genderId, DateTime birthDate, long birthCountryId, long birthStateId, long birthCityId,
                    long phoneCountryId, String phone, String email, int status)
    {
        Id = id;
        FirstName1 = firstName1;
        FirstName2 = firstName2;
        LastName1 = lastName1;
        LastName2 = lastName2;
        GenderId = genderId;
        BirthDate = birthDate;
        BirthCountryId = birthCountryId;
        BirthStateId = birthStateId;
        BirthCityId = birthCityId;
        PhoneCountryId = phoneCountryId;
        Phone = phone;
        Email = email;
        Status = status;
    }
}
