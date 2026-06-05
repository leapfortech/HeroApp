using System;

public class FeedUserData
{
    public long PostId { get; set; }
    public DateTime PublicationDateTime { get; set; }

    public FeedUserData(long postId, DateTime publicationDateTime)
    {
        PostId = postId;
        PublicationDateTime = publicationDateTime;
    }
}