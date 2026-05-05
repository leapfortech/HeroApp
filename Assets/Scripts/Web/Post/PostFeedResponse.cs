using System;
using System.Collections.Generic;

public class PostFeedResponse
{
    public int Chunk { get; set; }
    public int Direction { get; set; }

    public List<PostFull> PostFulls { get; set; } = new();

    // Stats
    public int Total { get; set; } = 0;

    public long FirstPostId { get; set; } = -1;
    public DateTime FirstDateTime { get; set; }
    public long LastPostId { get; set; } = -1;
    public DateTime LastDateTime { get; set; }

    public PostFeedResponse()
    {
    }
}
