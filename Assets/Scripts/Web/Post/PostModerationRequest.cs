using UnityEngine;

public class PostModerationRequest
{
    public long PostId { get; set; }
    public long Id { get; set; }

    public PostModerationRequest()
    {
    }

    public PostModerationRequest(long postId, long id)
    {
        PostId = postId;
        Id = id;
    }
}
