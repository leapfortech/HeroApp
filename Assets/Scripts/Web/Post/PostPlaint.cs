using System;

public class PostPlaint
{
    public long Id { get; set; }
    public long PlaintTypeId { get; set; }
    public long PostId { get; set; }
    public long AppUserId { get; set; }
    public int Status { get; set; }

    public PostPlaint() { }

    public PostPlaint(long id, long plaintTypeId, long postId, long appUserId, int status)
    {
        Id = id;
        PlaintTypeId = plaintTypeId;
        PostId = postId;
        AppUserId = appUserId;
        Status = status;
    }

    public PostPlaint(long plaintTypeId, long postId, long appUserId)
    {
        Id = -1;
        PlaintTypeId = plaintTypeId;
        PostId = postId;
        AppUserId = appUserId;
        Status = -1;
    }
}
