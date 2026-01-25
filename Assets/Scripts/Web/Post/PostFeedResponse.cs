using System;
using System.Collections.Generic;

public class PostFeedResponse
{
    public List<PostFull> PostFulls { get; set; } = new();

    public int Total { get; set; }

    // CURSORS
    public DateTime? FirstPublicationDateTime { get; set; }
    public long FirstPostId { get; set; }

    public DateTime? LastPublicationDateTime { get; set; }
    public long LastPostId { get; set; }

    public PostFeedResponse()
    {
    }
}
