using System;

public class IdentityPlace
{
    public long AppUserId { get; set; } = -1;
    public long IdentityId { get; set; } = -1;
    public long BirthCountryId { get; set; } = -1;
    public long BirthStateId { get; set; } = -1;
    public long BirthCityId { get; set; } = -1;

    public IdentityPlace()
    {
    }

    public IdentityPlace(long appUserId, long identityId, long birthCountryId, long birthStateId, long birthCityId)
    {
        AppUserId = appUserId;
        IdentityId = identityId;
        BirthCountryId = birthCountryId;
        BirthStateId = birthStateId;
        BirthCityId = birthCityId;
    }

    public IdentityPlace(long appUserId, Identity identity)
    {
        AppUserId = appUserId;
        IdentityId = identity.Id;
        BirthCountryId = identity.BirthCountryId;
        BirthStateId = identity.BirthStateId;
        BirthCityId = identity.BirthCityId;
    }
}