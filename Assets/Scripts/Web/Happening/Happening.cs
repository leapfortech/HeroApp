using System;

public class Happening
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public long HappeningTypeId { get; set; }
    public long CountryId { get; set; }
    public long StateId { get; set; }
    public int IsPublic { get; set; }
    public int HasSignup { get; set; }
    public int HasPayment { get; set; }
    public String PaymentDetails { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public String Location { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int Status { get; set; }

    public Happening() { }

    public Happening(long id, long postId, long happeningTypeId, long countryId, long stateId,
                        int isPublic, int hasSignup, int hasPayment, String paymentDetails,
                        DateTime? startDateTime, DateTime? endDateTime, String location, double? latitude,
                        double? longitude, int status)
    {
        Id = id;
        PostId = postId;
        HappeningTypeId = happeningTypeId;
        CountryId = countryId;
        StateId = stateId;
        IsPublic = isPublic;
        HasSignup = hasSignup;
        HasPayment = hasPayment;
        PaymentDetails = paymentDetails;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        Location = location;
        Latitude = latitude;
        Longitude = longitude;
        Status = status;
    }

    public Happening(HappeningFull happeningFull)
    {
        Id = happeningFull.Id;
        PostId = happeningFull.PostId;
        HappeningTypeId = happeningFull.HappeningTypeId;
        CountryId = happeningFull.CountryId;
        StateId = happeningFull.StateId;
        IsPublic = happeningFull.IsPublic;
        HasSignup = happeningFull.HasSignup;
        HasPayment = happeningFull.HasPayment;
        PaymentDetails = happeningFull.PaymentDetails;
        StartDateTime = happeningFull.StartDateTime;
        EndDateTime = happeningFull.EndDateTime;
        Location = happeningFull.Location;
        Latitude = happeningFull.Latitude;
        Longitude = happeningFull.Longitude;
        Status = happeningFull.Status;
    }

    public void Update(Happening happening)
    {
        HappeningTypeId = happening.HappeningTypeId;
        CountryId = happening.CountryId;
        StateId = happening.StateId;
        IsPublic = happening.IsPublic;
        HasSignup = happening.HasSignup;
        HasPayment = happening.HasPayment;
        PaymentDetails = happening.PaymentDetails;
        Location = happening.Location;
        Latitude = happening.Latitude;
        Longitude = happening.Longitude;
    }
}
