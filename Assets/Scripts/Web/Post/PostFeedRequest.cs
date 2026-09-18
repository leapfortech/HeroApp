using System;

public class PostFeedRequest
{
    // PARAMS
    public int Chunk { get; set; } = -1;

    public DateTime StartDateTime { get; set; }
    public int Direction { get; set; } = -1;
    public int Count { get; set; } = 20;

    // LIKE
    public long ReactionAppUserId { get; set; } = -1L;

    // FILTERS
    public long PostTypeId { get; set; } = -1L;
    public long AppUserId { get; set; } = -1L;
    public long CountryId { get; set; } = -1L;
    public long StateId { get; set; } = -1L;
    public int Status { get; set; } = -1;

    public long FavoriteAppUserId { get; set; } = -1L;
    public long SelectedAppUserId { get; set; } = -1L;

    public PostFeedRequest()
    {
    }

    public PostFeedRequest(int chunk, DateTime startDateTime, int direction, int count, long reactionAppUserId, long postTypeId, long appUserId, long countryId, long stateId, int status, long favoriteAppUserId, long selectedAppUserId)
    {
        Chunk = chunk;
        StartDateTime = startDateTime;
        Direction = direction;
        Count = count;
        ReactionAppUserId = reactionAppUserId;

        PostTypeId = postTypeId;
        AppUserId = appUserId;
        CountryId = countryId;
        StateId = stateId;
        Status = status;

        FavoriteAppUserId = favoriteAppUserId;
        SelectedAppUserId = selectedAppUserId;
    }
}
