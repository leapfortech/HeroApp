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

    public RadioType(long radioId, RadioTypeFull radioTypeFull)
    {
        Id = radioTypeFull.Id;
        RadioId = radioId;
        RadioTypeId = radioTypeFull.RadioTypeId;
        Status = radioTypeFull.Status;
    }
}
