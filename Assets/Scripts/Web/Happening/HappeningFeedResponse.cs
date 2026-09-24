using System.Collections.Generic;

public class HappeningFeedResponse
{
    public int Chunk { get; set; }
    public int Direction { get; set; }

    public List<HappeningFeed> HappeningFeeds { get; set; }

    public int Total { get; set; }
}
