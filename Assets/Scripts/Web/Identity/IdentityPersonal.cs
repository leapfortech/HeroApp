using System;

public class IdentityPersonal
{
    public long AppUserId { get; set; } = -1;
    public long IdentityId { get; set; } = -1;
    public String FirstName1 { get; set; } = null;
    public String FirstName2 { get; set; } = null;
    public String LastName1 { get; set; } = null;
    public String LastName2 { get; set; } = null;
    public DateTime BirthDate { get; set; } = new DateTime(1753, 1, 1);
    public long GenderId { get; set; } = -1;

    public IdentityPersonal()
    {
    }

    public IdentityPersonal(long appUserId, long identityId,
                            String firstName1, String firstName2,
                            String lastName1, String lastName2,
                            DateTime birthDate, long genderId)
    {
        AppUserId = appUserId;
        IdentityId = identityId;
        FirstName1 = firstName1;
        FirstName2 = firstName2;
        LastName1 = lastName1;
        LastName2 = lastName2;
        BirthDate = birthDate;
        GenderId = genderId;
    }

    public IdentityPersonal(long appUserId, Identity identity)
    {
        AppUserId = appUserId;
        IdentityId = identity.Id;
        FirstName1 = identity.FirstName1;
        FirstName2 = identity.FirstName2;
        LastName1 = identity.LastName1;
        LastName2 = identity.LastName2;
        BirthDate = identity.BirthDate;
        GenderId = identity.GenderId;
    }
}