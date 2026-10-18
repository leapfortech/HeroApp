using System.Collections.Generic;

public class MemoryFeedRequest : PostFeedRequest
{
    public List<long> MemoryTypeIds { get; set; } = new List<long> { -1L };
    public MemoryFeedRequest()
    {
        PostTypeId = PostType.Memory;
    }
}
