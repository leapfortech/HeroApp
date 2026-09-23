using System;

public class RadioUserData : FeedUserData
{
    public String Url { get; set; }

    public RadioUserData(long postId, DateTime publicationDateTime) : base(postId, publicationDateTime)
    {
    }

    public RadioUserData(long postId, DateTime publicationDateTime, String url) : base(postId, publicationDateTime)
    {
        Url = url;
    }
}