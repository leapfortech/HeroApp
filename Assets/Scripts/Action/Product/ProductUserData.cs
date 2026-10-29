using System;

public class ProductUserData : FeedUserData
{
    public long PostId0 { get; set; }
    public DateTime PublicationDateTime0 { get; set; }
    public String Link0 { get; set; }

    public long PostId1 { get; set; }
    public DateTime PublicationDateTime1 { get; set; }
    public String Link1 { get; set; }

    public ProductUserData(long postId, DateTime publicationDateTime) : base(postId, publicationDateTime)
    {
    }

    public ProductUserData(long postId0, DateTime publicationDateTime0, String link0, long postId1, DateTime publicationDateTime1, String link1) : base(postId1, publicationDateTime1)
    {
        PostId0 = postId0;
        PublicationDateTime0 = publicationDateTime0;
        Link0 = link0;

        PostId1 = postId1;
        PublicationDateTime1 = publicationDateTime1;
        Link1 = link1;
    }
}