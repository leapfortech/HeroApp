using System;

public class Radio
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public int Status { get; set; }

    public Radio() { }

    public Radio(long id, long postId, int status)
    {
        Id = id;
        PostId = postId;
        Status = status;
    }

    public Radio(RadioFull radioFull)
    {
        Id = radioFull.Id;
        PostId = radioFull.PostId;
        Status = radioFull.Status;
    }
}
