
using System.Collections.Generic;

public class FeedState
{
    public List<PostFull> PostFulls { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public bool IsLoading { get; set; }
    public bool HasMore { get; set; }
    public long PostSubtypeId { get; set; }

    public FeedState(int page, int pageSize, bool isLoading, bool hasMore, long postSubtipeId)
    {
        PostFulls = new List<PostFull>();
        Page = page;
        PageSize = pageSize;
        IsLoading = isLoading;
        HasMore = hasMore;
        PostSubtypeId = postSubtipeId;
    }
}