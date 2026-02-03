using System;

public class Favorite
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public long AppUserId { get; set; }
    public int Status { get; set; }

    public Favorite()
    {
    }

    public Favorite(long id, long postId, long appUserId, int status)
    {
        Id = id;
        PostId = postId;
        AppUserId = appUserId;
        Status = status;
    }

    public Favorite(long postId, long appUserId)
    {
        Id = -1;
        PostId = postId;
        AppUserId = appUserId;
        Status = -1;
    }
}
