using System;

public class FeedCommentUserData
{
    public long PostId { get; set; }
    public DateTime CreateDateTime { get; set; }

    public FeedCommentUserData(long postId, DateTime createDateTime)
    {
        PostId = postId;
        CreateDateTime = createDateTime;
    }
}