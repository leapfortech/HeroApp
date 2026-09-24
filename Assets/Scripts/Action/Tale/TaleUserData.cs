using System;

public class TaleUserData : FeedUserData
{
    public int[] ReactionCounts { get; set; }

    public TaleUserData(long postId, DateTime publicationDateTime) : base(postId, publicationDateTime)
    {
        ReactionCounts = new int[0];
    }

    public TaleUserData(long postId, DateTime publicationDateTime, int[] reactionCounts) : base(postId, publicationDateTime)
    {
        ReactionCounts = reactionCounts;
    }
}