using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Dialog;
using Leap.Core.Tools;

using Sirenix.OdinInspector;

public class FeedAction : MonoBehaviour
{
    [Space]
    [Title("Feed")]
    [SerializeField]
    FeedState feedConfig = null;
    [SerializeField]
    bool filterAppUser = false;

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

    [Title("Event")]
    [SerializeField]
    UnityLongEvent onSelected = null;

    PostService postService;
    FeedState state;

    Dictionary<int, long> indexes = new();
    int idx = 0;
    
    private int lastDirection = 0;
    long countryId = -1, stateId = -1;

    private void Awake()
    {
        postService = GetComponent<PostService>();
        state = StateManager.Instance.GetFeed(feedConfig.FeedKey);
    }

    private void Start()
    {
        btnOlder?.AddAction(LoadOlder);
        btnRefresh?.AddAction(Refresh);
    }

    public void SelectPost(int idx)
    {
        if (indexes.TryGetValue(idx, out long postId))
            onSelected.Invoke(postId);
    }

    public long GetPostId(int idx)
    {
        if (indexes.TryGetValue(idx, out long postId))
            return postId;
        return -1;
    }

    public void SetFilters(long countryId = -1, long stateId = -1)
    {
        this.countryId = countryId;
        this.stateId = stateId;
    }

    public void FirstLoad()
    {
        if (state.IsLoading)
            return;

        if (state.PostFulls.Count > 0)
            return;

        ScreenDialog.Instance.Display();
        state.IsLoading = true;
        lastDirection = 0;

        PostFeedRequest request = new PostFeedRequest
        {
            Direction = 0,
            PageSize = state.PageSize,
            PostSubtypeId = state.PostSubtypeId,
            Status = state.Status,

            AppUserId = filterAppUser ? StateManager.Instance.AppUser.Id : -1,
            CountryId = countryId,
            StateId = stateId,

            Cursor = null
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

            AppUserId = filterAppUser ? StateManager.Instance.AppUser.Id : -1,
            CountryId = countryId,
            StateId = stateId,

            Cursor = state.NextCursor
        };

        postService.GetPostFeed(request);
    }

    // REFRESH
    public void Refresh()
    {
        if (state.IsLoading)
            return;

        if (string.IsNullOrEmpty(state.PrevCursor))
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

            AppUserId = filterAppUser ? StateManager.Instance.AppUser.Id : -1,
            CountryId = countryId,
            StateId = stateId,

            Cursor = state.PrevCursor
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

                if (state.PostIds.Add(post.PostId))
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

                if (state.PostIds.Add(post.PostId))
                    state.PostFulls.Add(post);
            }

            DisplayFrom(startIndex);
        }
        else // INITIAL
        {
            state.PostIds.Clear();
            state.PostFulls.Clear();

            indexes.Clear();
            idx = 0;
            lstFeed.Clear();

            for (int i = 0; i < response.PostFulls.Count; i++)
            {
                PostFull post = response.PostFulls[i];

                if (state.PostIds.Add(post.PostId))
                    state.PostFulls.Add(post);
            }

            DisplayFrom(0);
        }

        // SET CURSORS
        state.PrevCursor = response.PrevCursor;
        state.NextCursor = response.NextCursor;

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