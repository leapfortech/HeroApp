using System;

public class Selected
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public long AppUserId { get; set; }
    public int Status { get; set; }

    public Selected()
    {
    }

    public Selected(long id, long postId, long appUserId, int status)
    {
        Id = id;
        PostId = postId;
        AppUserId = appUserId;
        Status = status;
    }

    public Selected(long postId, long appUserId)
    {
        Id = -1;
        PostId = postId;
        AppUserId = appUserId;
        Status = -1;
    }
}
