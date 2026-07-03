using System;

public class ServiceWish
{
    public long Id { get; set; }
    public long AppUserId { get; set; }
    public long ServiceTypeId { get; set; }
    public String Wish { get; set; }
    public int Status { get; set; }

    public ServiceWish()
    { 
    }

    public ServiceWish(long id, long appUserId, long serviceTypeId, String wish, int status)
    {
        Id = id;
        AppUserId = appUserId;
        ServiceTypeId = serviceTypeId;
        Wish = wish;
        Status = status;
    }
}
