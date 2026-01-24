
using System.Collections.Generic;

public class PostFeedState
{
    public List<PostFull> PostFulls { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public bool IsLoading { get; set; }
    public bool HasMore { get; set; }
    public long PostSubtypeId { get; set; }

    public PostFeedState()
    {
        PostFulls = new List<PostFull>();
        Page = 1;
        PageSize = 10;
        HasMore = true;
        IsLoading = false;
    }
}