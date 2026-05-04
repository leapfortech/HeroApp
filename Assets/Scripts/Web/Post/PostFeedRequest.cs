using System;

public class PostFeedRequest
{
    // PARAMS
    public int Chunk { get; set; } = -1;

    public DateTime StartDateTime { get; set; }
    public int Direction { get; set; } = -1;
    public int Count { get; set; } = 20;

    // FILTERS
    public long PostTypeId { get; set; } = -1;
    public long AppUserId { get; set; } = -1;
    public long CountryId { get; set; } = -1;
    public long StateId { get; set; } = -1;
    public int Status { get; set; } = -1;


    public PostFeedRequest()
    {
    }

    public PostFeedRequest(int chunk, DateTime startDateTime, int direction, int count, long postTypeId, long appUserId, long countryId, long stateId, int status)
    {
        Chunk = chunk;
        StartDateTime = startDateTime;
        Direction = direction;
        Count = count;

        PostTypeId = postTypeId;
        AppUserId = appUserId;
        CountryId = countryId;
        StateId = stateId;
        Status = status;
    }
}
