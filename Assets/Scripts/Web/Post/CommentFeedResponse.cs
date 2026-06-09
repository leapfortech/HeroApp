using System;
using System.Collections.Generic;

public class CommentFeedResponse
{
    public int Chunk { get; set; }
    public int Direction { get; set; }

    public List<CommentFull> CommentFulls { get; set; } = new();

    // Stats
    public int Total { get; set; } = 0;

    public long FirstCommentId { get; set; } = -1;
    public DateTime FirstDateTime { get; set; }
    public long LastCommentId { get; set; } = -1;
    public DateTime LastDateTime { get; set; }

    public CommentFeedResponse()
    {
    }
}
