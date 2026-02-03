using System;

public class PostRead
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public long AppUserId { get; set; }

    public PostRead() 
    {
    }

    public PostRead(long id, long postId, long appUserId)
    {
        Id = id;
        PostId = postId;
        AppUserId = appUserId;
    }

    public PostRead(long postId, long appUserId)
    {
        Id = -1;
        PostId = postId;
        AppUserId = appUserId;
    }
}
