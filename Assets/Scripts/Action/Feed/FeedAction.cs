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
    [Title("Loop")]
    [SerializeField]
    LoopScroller loopFeed = null;
    [SerializeField]
    Text txtEmpty;

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
    }

    public void SelectPost(int idx)
    {
        if (indexes.TryGetValue(idx, out long postId))
            onSelected.Invoke(postId);
    }

    public long GetPostId(int idx)
    {
        if (!indexes.TryGetValue(idx, out long postId))
            return -1;
        return postId;
    }

    public void SetFilters(long countryId = -1, long stateId = -1)
    {
        this.countryId = countryId;
        this.stateId = stateId;
    }

    public void GetFirstFeeds()
    {
        if (state.PostFulls.Count > 0)
            return;

        ScreenDialog.Instance.Display();
        lastDirection = 0;

        PostFeedRequest request = new PostFeedRequest
        {
            Direction = 0,
            Count = state.Count,
            PostTypeId = state.PostTypeId,
            Status = state.Status,

            AppUserId = filterAppUser ? StateManager.Instance.AppUser.Id : -1,
            CountryId = countryId,
            StateId = stateId,
        };

        postService.GetPostFeed(request);
    }

    public void GetNextFeeds()
    {
        ScreenDialog.Instance.Display();

        PostFeedRequest request = new PostFeedRequest
        {
            Direction = 1,
            Count = state.Count,
            PostTypeId = state.PostTypeId,
            Status = state.Status,

            AppUserId = filterAppUser ? StateManager.Instance.AppUser.Id : -1,
            CountryId = countryId,
            StateId = stateId,
        };

        postService.GetPostFeed(request);
    }

    public void ApplyFeeds(PostFeedResponse response)
    {
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
            //>>lstFeed.Clear();
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
            //>>lstFeed.Clear();

            for (int i = 0; i < response.PostFulls.Count; i++)
            {
                PostFull post = response.PostFulls[i];

                if (state.PostIds.Add(post.PostId))
                    state.PostFulls.Add(post);
            }

            DisplayFrom(0);
        }

        txtEmpty.gameObject.SetActive(state.PostFulls.Count == 0);
        ScreenDialog.Instance.Hide();
    }

    private void DisplayFrom(int startIdx)
    {
        for (int i = 0; i < state.PostFulls.Count; i++)
        {
            indexes[idx++] = state.PostFulls[i].PostId;

            String summary = state.PostFulls[i].Summary;

            if (!String.IsNullOrEmpty(summary) && summary.Length > 100)
                summary = summary[..100] + "...";

            LoopScrollerValue scrollerValue = new LoopScrollerValue(3, state.PostFulls[i].PublicationDateTime);
            scrollerValue.SetText(0, state.PostFulls[i].Description);
            scrollerValue.SetSprite(1, state.PostFulls[i].TitleSprite);
            scrollerValue.SetText(2, summary);

            loopFeed.AddValue(scrollerValue);
        }

        //>>lstFeed.ApplyValues();
        txtEmpty.gameObject.SetActive(state.PostFulls.Count == 0);
    }
}