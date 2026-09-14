using System;

public class Memory
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public long MemoryTypeId { get; set; }
    public long CountryId { get; set; }
    public long StateId { get; set; }
    public DateTime? DateTime { get; set; }
    public String Location { get; set; }
    public int Status { get; set; }

    public Memory() { }

    public Memory(long id, long postId, long memoryTypeId, long countryId, long stateId,
                  DateTime? dateTime, String location, int status)
    {
        Id = id;
        PostId = postId;
        MemoryTypeId = memoryTypeId;
        CountryId = countryId;
        StateId = stateId;
        DateTime = dateTime;
        Location = location;
        Status = status;
    }

    public Memory(MemoryFull memoryFull)
    {
        Id = memoryFull.Id;
        PostId = memoryFull.PostId;
        MemoryTypeId = memoryFull.MemoryTypeId;
        CountryId = memoryFull.CountryId;
        StateId = memoryFull.StateId;
        DateTime = memoryFull.DateTime;
        Location = memoryFull.Location;
        Status = memoryFull.Status;
    }

    public void Update(Memory memory)
    {
        MemoryTypeId = memory.MemoryTypeId;
        CountryId = memory.CountryId;
        StateId = memory.StateId;
        Location = memory.Location;
    }
}
