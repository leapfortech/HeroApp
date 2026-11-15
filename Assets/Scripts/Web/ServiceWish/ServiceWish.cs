using System;

public class ServiceWish
{
    public long Id { get; set; }
    public long AppUserId { get; set; }
    public long ServiceTypeId { get; set; }
    public long ServiceOptionId { get; set; }
    public int ServiceOptions { get; set; }
    public String Wish { get; set; }
    public int Status { get; set; }

    public ServiceWish()
    { 
    }

    public ServiceWish(long id, long appUserId, long serviceTypeId, long serviceOptionId, int serviceOptions, String wish, int status)
    {
        Id = id;
        AppUserId = appUserId;
        ServiceTypeId = serviceTypeId;
        ServiceOptionId = serviceOptionId;
        ServiceOptions = serviceOptions;
        Wish = wish;
        Status = status;
    }
}
