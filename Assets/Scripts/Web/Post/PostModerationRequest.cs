using UnityEngine;

public class PostModerationRequest
{
    public long PostId { get; set; }
    public long SubtypeId { get; set; }

    public PostModerationRequest()
    {
    }

    public PostModerationRequest(long postId, long subtypeId)
    {
        PostId = postId;
        SubtypeId = subtypeId;
    }
}
