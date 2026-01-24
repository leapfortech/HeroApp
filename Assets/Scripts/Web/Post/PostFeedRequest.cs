
public class PostFeedRequest
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public long PostSubtypeId { get; set; }
    public int Status { get; set; }
    public PostFeedRequest()
    {
    }

    public PostFeedRequest(int page, int pageSize, long postSubtypeId, int status)
    {
        Page = page;
        PageSize = pageSize;
        PostSubtypeId = postSubtypeId;
        Status = status;
    }
}
