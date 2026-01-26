using System;

public class PostFeedRequest
{
    // PARAMS
    public int PageSize { get; set; } = 10;

    // FILTERS
    public long AppUserId { get; set; } = -1;
    public long PostSubtypeId { get; set; } = -1;
    public long CountryId { get; set; } = -1;
    public long StateId { get; set; } = -1;
    public int Status { get; set; } = -1;

    // CURSOR
    public string Cursor { get; set; } = null;

    public int Direction { get; set; } = 0;


    public PostFeedRequest()
    {
    }

    public PostFeedRequest(int pageSize, long appUserId, long postSubtypeId, long countryId, long stateId,
                           int status, String cursor, int direction)
    {
        PageSize = pageSize;
        AppUserId = appUserId;
        PostSubtypeId = postSubtypeId;
        CountryId = countryId;
        StateId = stateId;
        Status = status;
        Cursor = cursor;
        Direction = direction;
    }
}
