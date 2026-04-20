using System;

public class ServiceWish
{
    public long Id { get; set; }
    public long AppUserId { get; set; }
    public long ServiceTypeId { get; set; }
    public String Comment { get; set; }
    public int Status { get; set; }

    public ServiceWish()
    { 
    }

    public ServiceWish(long id, long appUserId, long serviceTypeId, String comment, int status)
    {
        Id = id;
        AppUserId = appUserId;
        ServiceTypeId = serviceTypeId;
        Comment = comment;
        Status = status;
    }
}
