using System;

public class RadioType
{
    public long Id { get; set; }
    public long RadioId { get; set; }
    public long RadioTypeId { get; set; }
    public int Status { get; set; }

    public RadioType() 
    {
    }

    public RadioType(long id, long radioId, long radioTypeId, int status)
    {
        Id = id;
        RadioId = radioId;
        RadioTypeId = radioTypeId;
        Status = status;
    }
}
