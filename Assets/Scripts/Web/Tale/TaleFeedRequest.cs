using System;

public class TaleFeedRequest : PostFeedRequest
{
    public TaleFeedRequest()
    {
        PostTypeId = PostType.Tale;
    }
}
