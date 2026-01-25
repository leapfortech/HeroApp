using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;

public class FeedAction : MonoBehaviour
{
    [Space]
    [Title("List")]
    [SerializeField]
    ListScroller lstFeed = null;
    [SerializeField]
    Text txtEmpty;

    [Title("Action")]
    [SerializeField]
    Button btnOlder = null;
    [SerializeField]
    Button btnRefresh = null;

    PostService postService;
    FeedState state;

    public int SelIdx { get; set; } = 0;

    Dictionary<int, long> indexes = new();
    int idx = 0;
    
    private int lastDirection = 0;
    private HashSet<long> postIds = new HashSet<long>();

    private void Awake()
    {
        postService = GetComponent<PostService>();
    }


    private void Start()
    {
        btnOlder?.AddAction(LoadOlder);
        btnRefresh?.AddAction(Refresh);
    }

    public long GetSelectedPostId()
    {
        if (indexes.TryGetValue(SelIdx, out long postId))
            return postId;

        return -1;
    }

    public void FirstLoad()
    {
        if (state != null)
            return;

        state = new FeedState(postSubtypeId: 1, pageSize: 3, isLoading: false, hasMore: true, status: 1);
        StateManager.Instance.FeedTale = state;

        ScreenDialog.Instance.Display();
        state.IsLoading = true;

        lastDirection = 0;

        PostFeedRequest request = new PostFeedRequest
        {
            Direction = 0,
            PageSize = state.PageSize,
            PostSubtypeId = state.PostSubtypeId,
            Status = state.Status
        };

        postService.GetPostFeed(request);
    }

    // OLDER
    public void LoadOlder()
    {
        if (state.IsLoading)
            return;

        if (!state.HasMore)
            return;

        ScreenDialog.Instance.Display();
        state.IsLoading = true;

        lastDirection = 2;

        PostFeedRequest request = new PostFeedRequest
        {
            Direction = 2,
            PageSize = state.PageSize,
            PostSubtypeId = state.PostSubtypeId,
            Status = state.Status,
            LastPublicationDateTime = state.LastPublicationDateTime,
            LastPostId = state.LastPostId
        };

        postService.GetPostFeed(request);
    }

    // REFRESH
    public void Refresh()
    {
        if (state.IsLoading)
            return;

        if (!state.FirstPublicationDateTime.HasValue)
            return;

        ScreenDialog.Instance.Display();
        state.IsLoading = true;

        lastDirection = 1;

        PostFeedRequest request = new PostFeedRequest
        {
            Direction = 1,
            PageSize = state.PageSize,
            PostSubtypeId = state.PostSubtypeId,
            Status = state.Status,
            FirstPublicationDateTime = state.FirstPublicationDateTime,
            FirstPostId = state.FirstPostId
        };

        postService.GetPostFeed(request);
    }

    public void ApplyFeed(PostFeedResponse response)
    {
        state.IsLoading = false;

        if (response.PostFulls.Count == 0)
        {
            ScreenDialog.Instance.Hide();
            return;
        }

        if (lastDirection == 1) // REFRESH
        {
            for (int i = 0; i < response.PostFulls.Count; i++)
            {
                PostFull post = response.PostFulls[i];

                if (postIds.Add(post.PostId))
                    state.PostFulls.Insert(0, post);
            }

            idx = 0;
            indexes.Clear();
            lstFeed.Clear();
            DisplayFrom(0);
        }
        else if (lastDirection == 2) // OLDER
        {
            int startIndex = state.PostFulls.Count;

            for (int i = 0; i < response.PostFulls.Count; i++)
            {
                PostFull post = response.PostFulls[i];

                if (postIds.Add(post.PostId))
                    state.PostFulls.Add(post);
            }

            DisplayFrom(startIndex);
        }
        else // INITIAL
        {
            postIds.Clear();
            state.PostFulls.Clear();
            indexes.Clear();
            idx = 0;
            lstFeed.Clear();

            for (int i = 0; i < response.PostFulls.Count; i++)
            {
                PostFull post = response.PostFulls[i];

                if (postIds.Add(post.PostId))
                    state.PostFulls.Add(post);
            }

            DisplayFrom(0);
        }

        // SET CURSORS
        state.FirstPublicationDateTime = response.FirstPublicationDateTime;
        state.FirstPostId = response.FirstPostId;

        state.LastPublicationDateTime = response.LastPublicationDateTime;
        state.LastPostId = response.LastPostId;

        if (lastDirection == 0 || lastDirection == 2)
            state.HasMore = response.PostFulls.Count == state.PageSize;

        txtEmpty.gameObject.SetActive(state.PostFulls.Count == 0);
        ScreenDialog.Instance.Hide();
    }

    private void DisplayFrom(int startIdx)
    {
        for (int i = startIdx; i < state.PostFulls.Count; i++)
        {
            indexes[idx] = state.PostFulls[i].PostId;
            idx++;

            ListScrollerValue scrollerValue = new ListScrollerValue(2, true);
            scrollerValue.SetText(0, state.PostFulls[i].Description);
            scrollerValue.SetSprite(1, state.PostFulls[i].TitleSprite);

            lstFeed.AddValue(scrollerValue);
        }

        lstFeed.ApplyValues();
        txtEmpty.gameObject.SetActive(state.PostFulls.Count == 0);
    }
}