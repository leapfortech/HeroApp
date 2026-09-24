using System;
using System.Collections.Generic;

public class MemoryFeedResponse
{
    public int Chunk { get; set; }
    public int Direction { get; set; }

    public List<MemoryFeed> MemoryFeeds { get; set; }

    public int Total { get; set; }
}
