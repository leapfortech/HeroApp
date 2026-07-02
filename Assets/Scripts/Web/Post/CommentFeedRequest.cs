
using System;

public class CommentFeedRequest
{
    // PARAMS
    public int Chunk { get; set; } = -1;
    public DateTime StartDateTime { get; set; }
    public int Direction { get; set; }
    public int Count { get; set; }

    // FILTERS
    public long PostId { get; set; } = -1L;
    public long AppUserId { get; set; } = -1L;
    public int Status { get; set; } = 1;
}
