using System.Collections.Generic;

public class NewsFeedRequest : PostFeedRequest
{
    public List<long> NewsTypeIds { get; set; } = new List<long> { -1L };

    public NewsFeedRequest()
    {
        PostTypeId = PostType.News;
    }
}
