using System.Collections.Generic;

public class NewsFeedResponse
{
    public int Chunk { get; set; }
    public int Direction { get; set; }

    public List<NewsFeed> NewsFeeds { get; set; }

    public int Total { get; set; }
}