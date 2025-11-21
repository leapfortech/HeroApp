using System;

using Sirenix.OdinInspector;

public class Address
{
    public long Id { get; set; } = -1;
    
    [ShowInInspector]
    public long CountryId { get; set; } = -1;
    [ShowInInspector]
    public long StateId { get; set; } = -1;
    [ShowInInspector]
    public long CityId { get; set; } = -1;
    [ShowInInspector]
    public String Address1 { get; set; }
    [ShowInInspector]
    public String Address2 { get; set; }
    [ShowInInspector]
    public String Zone { get; set; }
    public String ZipCode { get; set; }
    public float? Latitude { get; set; }
    public float? Longitude { get; set; }
    public int Status { get; set; } = -1;


    public Address()
    {
    }

    public Address(long id, long countryId, long stateId, long cityId, String address1, String address2,
                   String zone, String zipCode, float? latitude, float? longitude, int status)
    {
        Id = id;
        CountryId = countryId;
        StateId = stateId;
        CityId = cityId;
        Address1 = address1;
        Address2 = address2;
        Zone = zone;
        ZipCode = zipCode;
        Latitude = latitude;
        Longitude = longitude;
        Status = status;
    }

    public Address(Address address)
    {
        Id = address.Id;
        CountryId = address.CountryId;
        StateId = address.StateId;
        CityId = address.CityId;
        Address1 = address.Address1;
        Address2 = address.Address2;
        Zone = address.Zone;
        ZipCode = address.ZipCode;
        Latitude = address.Latitude;
        Longitude = address.Longitude;
        Status = address.Status;
    }
}
