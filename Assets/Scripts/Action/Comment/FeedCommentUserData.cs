using System;

public class FeedCommentUserData
{
    public long PostId { get; set; }
    public DateTime PublicationDateTime { get; set; }

    public FeedCommentUserData(long postId, DateTime publicationDateTime)
    {
        PostId = postId;
        PublicationDateTime = publicationDateTime;
    }
}