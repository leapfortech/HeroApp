using System;

public class Reaction
{
    public long Id { get; set; }
    public long ReactionPhraseId { get; set; }
    public long PostId { get; set; }
    public long AppUserId { get; set; }
    public int Status { get; set; }

    public Reaction() { }

    public Reaction(long id, long reactionPhraseId, long postId, long appUserId, int status)
    {
        Id = id;
        ReactionPhraseId = reactionPhraseId;
        PostId = postId;
        AppUserId = appUserId;
        Status = status;
    }

    public Reaction(long reactionPhraseId, long postId, long appUserId)
    {
        Id = -1;
        ReactionPhraseId = reactionPhraseId;
        PostId = postId;
        AppUserId = appUserId;
        Status = 1;
    }
}
