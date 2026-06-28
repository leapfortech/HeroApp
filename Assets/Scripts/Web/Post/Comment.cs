using System;

public class Comment
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public long AppUserId { get; set; }
    public String Message { get; set; }
    public DateTime PublicationDateTime { get; set; }
    public int Status { get; set; }

    public Comment() { }

    public Comment(long id, long postId, long appUserId, String message, DateTime publicationDateTime, int status)
    {
        Id = id;
        PostId = postId;
        AppUserId = appUserId;
        Message = message;
        PublicationDateTime = publicationDateTime;
        Status = status;
    }

    public Comment(long postId, long appUserId, String message)
    {
        Id = -1;
        PostId = postId;
        AppUserId = appUserId;
        Message = message;
        PublicationDateTime = DateTime.UtcNow;
        Status = -1;
    }
}
