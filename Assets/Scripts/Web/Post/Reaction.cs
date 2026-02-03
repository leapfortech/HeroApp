using System;

public class Reaction
{
    public long Id { get; set; }
    public long ReactionTypeId { get; set; }
    public long PostId { get; set; }
    public long AppUserId { get; set; }
    public int Status { get; set; }

    public Reaction() { }

    public Reaction(long id, long reactionTypeId, long postId, long appUserId, int status)
    {
        Id = id;
        ReactionTypeId = reactionTypeId;
        PostId = postId;
        AppUserId = appUserId;
        Status = status;
    }

    public Reaction(long reactionTypeId, long postId, long appUserId)
    {
        Id = -1;
        ReactionTypeId = reactionTypeId;
        PostId = postId;
        AppUserId = appUserId;
        Status = -1;
    }
}
