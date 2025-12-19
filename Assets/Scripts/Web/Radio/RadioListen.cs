using System;

public class RadioListen
{
    public long Id { get; set; }
    public long RadioId { get; set; }
    public long AppUserId { get; set; }

    public RadioListen()
    {
    }

    public RadioListen(long id, long radioId, long appUserId)
    {
        Id = id;
        RadioId = radioId;
        AppUserId = appUserId;
    }
}
