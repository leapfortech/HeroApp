using System;
using System.Collections.Generic;

public class HappeningFeedRequest : PostFeedRequest
{
    public List<long> HappeningTypeIds { get; set; } = new List<long> { -1L };

    public HappeningFeedRequest()
    {
        PostTypeId = PostType.Happening;
    }
}
