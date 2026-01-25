
using System;
using System.Collections.Generic;

public class FeedState
{
    // CURSORS
    public DateTime? FirstPublicationDateTime;
    public long FirstPostId = -1;

    public DateTime? LastPublicationDateTime;
    public long LastPostId = -1;

    // STATE
    public bool IsLoading;
    public bool HasMore = true;

    // PARAMS
    public int PageSize;
    public long PostSubtypeId;
    public int Status;

    // DATA
    public List<PostFull> PostFulls;

    public FeedState(long postSubtypeId, int pageSize, bool isLoading, bool hasMore,int status)
    {
        PostSubtypeId = postSubtypeId;
        PageSize = pageSize;
        IsLoading = isLoading;
        HasMore = hasMore;
        Status = status;

        FirstPublicationDateTime = null;
        FirstPostId = -1;

        LastPublicationDateTime = null;
        LastPostId = -1;

        PostFulls = new List<PostFull>();
    }
}