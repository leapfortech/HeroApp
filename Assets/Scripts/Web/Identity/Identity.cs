using System;

using Sirenix.OdinInspector;


public class Identity
{
    public long Id { get; set; }
    [ShowInInspector]
    public String FirstName1 { get; set; }
    [ShowInInspector]
    public String FirstName2 { get; set; }
    [ShowInInspector]
    public String LastName1 { get; set; }
    [ShowInInspector]
    public String LastName2 { get; set; }
    [ShowInInspector]
    public long GenderId { get; set; }
    [ShowInInspector]
    public DateTime BirthDate { get; set; }
    [ShowInInspector]
    public long OriginCountryId { get; set; }
    [ShowInInspector]
    public long OriginStateId { get; set; }
    [ShowInInspector]
    public long PhoneCountryId { get; set; }
    [ShowInInspector]
    public String Phone { get; set; }
    [ShowInInspector]
    public String Email { get; set; }
    public int Status { get; set; }


    public Identity()
    {
    }

    public Identity(long id, String firstName1, String firstName2, String lastName1, String lastName2,
                    long genderId, DateTime birthDate, long originCountryId, long originStateId,
                    long phoneCountryId, String phone, String email, int status)
    {
        Id = id;
        FirstName1 = firstName1;
        FirstName2 = firstName2;
        LastName1 = lastName1;
        LastName2 = lastName2;
        GenderId = genderId;
        BirthDate = birthDate;
        OriginCountryId = originCountryId;
        OriginStateId = originStateId;
        PhoneCountryId = phoneCountryId;
        Phone = phone;
        Email = email;
        Status = status;
    }
}
