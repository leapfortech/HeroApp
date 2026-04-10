using System;

public class Locality
{
    public long Id { get; set; } = -1;
    public long AppUserId { get; set; } = -1;
    public int LocalityType { get; set; } = -1;
    public long CountryId { get; set; } = -1;
    public long StateId { get; set; } = -1;
    public long CityId { get; set; } = -1;
    public int Status { get; set; } = -1;

    public Locality() { }

    public Locality(long id, long appUserId, int localityType, long countryId,
                    long stateId, long cityId, int status)
    {
        Id = id;
        AppUserId = appUserId;
        LocalityType = localityType;
        CountryId = countryId;
        StateId = stateId;
        CityId = cityId;
        Status = status;
    }
}
