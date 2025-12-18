using System;

public class Tale
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public int Status { get; set; }

    public Tale() { }

    public Tale(long id, long postId, int status)
    {
        Id = id;
        PostId = postId;
        Status = status;
    }
}
