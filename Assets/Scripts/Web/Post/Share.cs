using System;

public class Share
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public long AppUserId { get; set; }

    public Share() { }

    public Share(long id, long postId, long appUserId)
    {
        Id = id;
        PostId = postId;
        AppUserId = appUserId;
    }

    public Share(long postId, long appUserId)
    {
        Id = -1;
        PostId = postId;
        AppUserId = appUserId;
    }
}
