using System;

public class HappeningUserData : FeedUserData
{
    public int FavoriteCount { get; set; }

    public HappeningUserData(long postId, DateTime publicationDateTime) : base(postId, publicationDateTime)
    {
    }

    public HappeningUserData(long postId, DateTime publicationDateTime, int favoriteCount) : base(postId, publicationDateTime)
    {
        FavoriteCount = favoriteCount;
    }
}