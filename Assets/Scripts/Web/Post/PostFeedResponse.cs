using System;
using System.Collections.Generic;

public class PostFeedResponse
{
    public int Chunk { get; set; }

    public List<PostFull> PostFulls { get; set; } = new();
    public int Total { get; set; }

    public PostFeedResponse()
    {
    }
}
