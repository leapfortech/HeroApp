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
    GameObject txtEmpty;

    PostService postService;
    FeedState feedState;

    //long countryId = -1;
    //long stateId = -1;

    private void Awake()
    {
        postService = GetComponent<PostService>();
    }

    public long GetPostId(int idx)
    {
        return feedState.PostFulls[idx].PostId;
    }

    //public void SetFilters(long countryId = -1, long stateId = -1)
    //{
    //    this.countryId = countryId;
    //    this.stateId = stateId;
    //}

    public void CreateFeeds(bool force)
    {
        if (feedState != null && !force)
            return;

        feedState = StateManager.Instance.GetFeedState(feedConfig.FeedKey);
        feedState.PostFulls = new List<PostFull>(feedState.Count * 4);

        loopFeed.ClearValues();
        DateTime now = DateTime.Now;
        PostFull emptyPostFull = new PostFull();
        for (int i = 0; i < feedState.Count * 4; i++)
        {
            feedState.PostFulls.Add(emptyPostFull);
            loopFeed.AddValue(CreateValue(emptyPostFull, now));
        }
        loopFeed.ApplyValues();

        GetPosts(0, new FeedUserData(-1, DateTime.Now), 2);
    }

    public LoopScrollerValue CreateValue(PostFull postFull, DateTime now)
    {
        LoopScrollerValue loopValue = new LoopScrollerValue(loopFeed.LoopItem.ElementCount, null);
        UpdateValue(postFull, loopValue, now);
        return loopValue;
    }

    public void GetPosts(int startLoopIdx, object userData, int direction)
    {
        ScreenDialog.Instance.Display();

        FeedUserData feedUserData = (FeedUserData)userData;
        PostFeedRequest request = new PostFeedRequest
        {
            Chunk = startLoopIdx,

            StartDateTime = feedUserData.PublicationDateTime,
            Direction = direction,
            Count = feedUserData.PostId == -1 ? feedState.Count + feedState.Count : feedState.Count,

            PostTypeId = feedState.PostTypeId,
            AppUserId = filterAppUser ? StateManager.Instance.AppUser.Id : -1,
            //CountryId = countryId,
            //StateId = stateId,
            Status = feedState.Status,
        };

        postService.GetPostFeed(request);
    }

    public void ApplyPosts(PostFeedResponse response)
    {
        if (response.PostFulls.Count == 0)
        {
            ScreenDialog.Instance.Hide();
            return;
        }

        //response.Chunk = response.Chunk == -1 ? 0 : (response.Chunk + feedState.Count);
        int startLoopIdx = response.Chunk % loopFeed.ValuesCount;
        int endLoopIdx = startLoopIdx + loopFeed.PreloadCount;

        //Debug.Log($"ApplyPosts : {startLoopIdx.ToString()} > {endLoopIdx.ToString()}, {response.PostFulls[0].PublicationDateTime.ToString("dd/MM/yyyy")} > {response.PostFulls[^1].PublicationDateTime.ToString("dd/MM/yyyy")}");

        DateTime now = DateTime.Now;
        for (int i = 0; i < response.PostFulls.Count; i++)
        {
            int k = (startLoopIdx + i) % loopFeed.ValuesCount;
            feedState.PostFulls[k] = response.PostFulls[i];
            UpdateValue(response.PostFulls[i], loopFeed[k], now);
        }

        loopFeed.RefreshVisibleValues();

        txtEmpty.SetActive(feedState.PostFulls.Count == 0);
        ScreenDialog.Instance.Hide();
    }

    public void UpdateValue(PostFull postFull, LoopScrollerValue loopValue, DateTime now)
    {
        loopValue.UserData = postFull.PublicationDateTime.Year == 1753 ? null : new FeedUserData(postFull.PostId, postFull.PublicationDateTime);
        loopValue.SetSprite(0, null);
        loopValue.SetText(1, postFull.AppUserAlias);
        loopValue.SetText(2, postFull.PublicationDateTime.Year == 1753 ? null : $"@{postFull.AppUserAlias} - hace {((int)(now - postFull.PublicationDateTime).TotalHours).ToString()} horas");
        loopValue.SetText(3, postFull.Summary);
    }
}