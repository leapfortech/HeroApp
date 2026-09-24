using System;
using System.Collections.Generic;

public class TaleFeedResponse
{
    public int Chunk { get; set; }
    public int Direction { get; set; }

    public List<TaleFeed> TaleFeeds { get; set; }

    public int Total { get; set; }
}
