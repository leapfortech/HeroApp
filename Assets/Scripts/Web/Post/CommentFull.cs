using System;

public class CommentFull
{
    public long Id { get; set; } = -1L;
    public long PostId { get; set; } = -1L;
    public long AppUserId { get; set; } = -1L;
    public String AppUserAlias { get; set; }
    public String Message { get; set; }
    public DateTime PublicationDateTime { get; set; } = new DateTime(1753, 1, 1);
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public int Status { get; set; } = -1;

    public CommentFull()
    {
    }

    public CommentFull(long id, long postId, long appUserId, String appUserAlias,
                             String message, DateTime publicationDateTime, DateTime createDateTime, DateTime updateDateTime, int status)
    {
        Id = id;
        PostId = postId;
        AppUserId = appUserId;
        AppUserAlias = appUserAlias;
        Message = message;
        PublicationDateTime = publicationDateTime;
        CreateDateTime = createDateTime;
        UpdateDateTime = updateDateTime;
        Status = status;
    }
}
