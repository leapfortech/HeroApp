using System;

public class CommentPlaint
{
    public long Id { get; set; }
    public long PlaintTypeId { get; set; }
    public long CommentId { get; set; }
    public long AppUserId { get; set; }
    public int Status { get; set; }

    public CommentPlaint() { }

    public CommentPlaint(long id, long plaintTypeId, long commentId, long appUserId, int status)
    {
        Id = id;
        PlaintTypeId = plaintTypeId;
        CommentId = commentId;
        AppUserId = appUserId;
        Status = status;
    }

    public CommentPlaint(long plaintTypeId, long commentId, long appUserId)
    {
        Id = -1;
        PlaintTypeId = plaintTypeId;
        CommentId = commentId;
        AppUserId = appUserId;
        Status = -1;
    }
}
