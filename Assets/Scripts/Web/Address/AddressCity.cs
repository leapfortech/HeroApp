using System;

public class AddressCity
{
    public long AppUserId { get; set; } = -1;
    public long AddressId { get; set; } = -1;
    public long CountryId { get; set; } = -1;
    public long StateId { get; set; } = -1;
    public long CityId { get; set; } = -1;

    public AddressCity()
    {
    }

    public AddressCity(long appUserId, long addressId,
                       long countryId, long stateId, long cityId)
    {
        AppUserId = appUserId;
        AddressId = addressId;
        CountryId = countryId;
        StateId = stateId;
        CityId = cityId;
    }

    public AddressCity(long appUserId, Address address)
    {
        AppUserId = appUserId;
        AddressId = address.Id;
        CountryId = address.CountryId;
        StateId = address.StateId;
        CityId = address.CityId;
    }
}