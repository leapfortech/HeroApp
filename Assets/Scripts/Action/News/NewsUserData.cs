using System;

public class NewsUserData : FeedUserData
{
    public int[] ReactionCounts { get; set; }

    public NewsUserData(long postId, DateTime publicationDateTime) : base(postId, publicationDateTime)
    {
        ReactionCounts = new int[0];
    }

    public NewsUserData(long postId, DateTime publicationDateTime, int[] reactionCounts) : base(postId, publicationDateTime)
    {
        ReactionCounts = reactionCounts;
    }
}