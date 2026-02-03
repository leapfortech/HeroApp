using System;

public class Like
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public long AppUserId { get; set; }
    public int Rank { get; set; }
    public int Status { get; set; }

    public Like() { }

    public Like(long id, long postId, long appUserId, int rank, int status)
    {
        Id = id;
        PostId = postId;
        AppUserId = appUserId;
        Rank = rank;
        Status = status;
    }

    public Like(long postId, long appUserId, int rank)
    {
        Id = -1;
        PostId = postId;
        AppUserId = appUserId;
        Rank = rank;
        Status = -1;
    }
}
