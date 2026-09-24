using System;

public class MemoryUserData : FeedUserData
{
    public int[] ReactionCounts { get; set; }

    public MemoryUserData(long postId, DateTime publicationDateTime) : base(postId, publicationDateTime)
    {
        ReactionCounts = new int[0];
    }

    public MemoryUserData(long postId, DateTime publicationDateTime, int[] reactionCounts) : base(postId, publicationDateTime)
    {
        ReactionCounts = reactionCounts;
    }
}