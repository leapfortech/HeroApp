using System;

public class Link
{
    public long Id { get; set; }
    public long LinkTypeId { get; set; }
    public long PostId { get; set; }
    public String Url { get; set; }
    public int Status { get; set; }

    public Link() 
    {
    }

    public Link(long id, long linkTypeId, long postId, String url, int status)
    {
        Id = id;
        LinkTypeId = linkTypeId;
        PostId = postId;
        Url = url;
        Status = status;
    }

    public Link(LinkFull linkFull)
    {
        Id = linkFull.Id;
        LinkTypeId = linkFull.LinkTypeId;
        PostId = linkFull.PostId;
        Url = linkFull.Url;
        Status = linkFull.Status;
    }
}
