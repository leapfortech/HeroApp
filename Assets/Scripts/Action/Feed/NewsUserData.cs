using System;

public class NewsUserData : FeedUserData
{
    public String Url { get; set; }

    public NewsUserData(long postId, DateTime publicationDateTime) : base(postId, publicationDateTime)
    {
    }

    public NewsUserData(long postId, DateTime publicationDateTime, String url) : base(postId, publicationDateTime)
    {
        Url = url;
    }
}