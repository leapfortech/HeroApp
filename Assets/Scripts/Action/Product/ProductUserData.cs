using System;

public class ProductUserData : FeedUserData
{
    public String Link { get; set; }

    public ProductUserData(long postId, DateTime publicationDateTime) : base(postId, publicationDateTime)
    {
    }

    public ProductUserData(long postId, DateTime publicationDateTime, String link) : base(postId, publicationDateTime)
    {
        Link = link;
    }
}