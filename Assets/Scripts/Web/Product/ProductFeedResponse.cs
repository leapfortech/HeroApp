using System.Collections.Generic;

public class ProductFeedResponse
{
    public int Chunk { get; set; }
    public int Direction { get; set; }

    public List<ProductFeed> ProductFeeds { get; set; }

    public int Total { get; set; }
}
