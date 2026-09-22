using System.Collections.Generic;

public class RadioFeedResponse
{
    public int Chunk { get; set; }
    public int Direction { get; set; }

    public List<RadioFeed> RadioFeeds { get; set; }

    public int Total { get; set; }
}