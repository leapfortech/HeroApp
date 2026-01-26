using System;
using System.Collections.Generic;

public class PostFeedResponse
{
    public List<PostFull> PostFulls { get; set; } = new();

    public int Total { get; set; }

    // CURSORS
    public string PrevCursor { get; set; }
    public string NextCursor { get; set; }

    public PostFeedResponse()
    {
    }
}
