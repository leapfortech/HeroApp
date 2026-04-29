using System;

public class PostFeedRequest
{
    // PARAMS
    public DateTime StartDateTime { get; set; }
    public int Direction { get; set; } = -1;
    public int Count { get; set; } = 20;

    // FILTERS
    public long AppUserId { get; set; } = -1;
    public long PostTypeId { get; set; } = -1;
    public long CountryId { get; set; } = -1;
    public long StateId { get; set; } = -1;
    public int Status { get; set; } = -1;


    public PostFeedRequest()
    {
    }

    public PostFeedRequest(DateTime startDateTime, int direction, int count, long appUserId, long postTypeId, long countryId, long stateId, int status)
    {
        StartDateTime = startDateTime;
        Direction = direction;
        Count = count;
        AppUserId = appUserId;
        PostTypeId = postTypeId;
        CountryId = countryId;
        StateId = stateId;
        Status = status;
    }
}
