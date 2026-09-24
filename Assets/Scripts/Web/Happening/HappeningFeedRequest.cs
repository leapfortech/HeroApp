using System;

public class HappeningFeedRequest : PostFeedRequest
{
    public long HappeningTypeId { get; set; } = -1L;

    public HappeningFeedRequest()
    {
        PostTypeId = PostType.Happening;
    }
}
