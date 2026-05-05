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

    [SerializeField, Space(5f), ListDrawerSettings(ShowPaging = false)]
    Sprite[] portraits = new Sprite[0];

    [Space]
    [Title("Loop")]
    [SerializeField]
    LoopScroller loopFeed = null;
    [SerializeField]
    GameObject txtEmpty;

    PostService postService;
    FeedState feedState;
    int feedCount = 0;

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
        feedCount = feedState.Count * 4;
        //feedState.PostFulls = new List<PostFull>(feedCount);

        loopFeed.ClearValues();
        DateTime now = DateTime.Now;
        PostFull emptyPostFull = new PostFull();
        for (int i = 0; i < feedCount; i++)
        {
            //feedState.PostFulls.Add(emptyPostFull);
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
        //ScreenDialog.Instance.Display();

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
            //ScreenDialog.Instance.Hide();
            return;
        }

        //response.Chunk = response.Chunk == -1 ? 0 : (response.Chunk + feedState.Count);
        int startLoopIdx = response.Chunk % loopFeed.ValuesCount;
        int endLoopIdx = startLoopIdx + loopFeed.PreloadCount;
        int direction = response.Direction;

        //Debug.Log($"ApplyPosts : {startLoopIdx.ToString()} > {endLoopIdx.ToString()}, {response.PostFulls[0].PublicationDateTime.ToString("dd/MM/yyyy")} > {response.PostFulls[^1].PublicationDateTime.ToString("dd/MM/yyyy")}");

        DateTime now = DateTime.Now;
        if (direction == 1)
        {
            for (int i = 0; i < response.PostFulls.Count; i++)
            {
                int k = (startLoopIdx - i - 1) % loopFeed.ValuesCount;
                if (k < 0)
                    break;
                //feedState.PostFulls[k] = response.PostFulls[i];
                UpdateValue(response.PostFulls[i], loopFeed[k], now);
            }
        }
        else
        {
            for (int i = 0; i < response.PostFulls.Count; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                //feedState.PostFulls[k] = response.PostFulls[i];
                UpdateValue(response.PostFulls[i], loopFeed[k], now);
            }
        }

        loopFeed.RefreshVisibleValues();

        txtEmpty.SetActive(response.Total == 0);
        //ScreenDialog.Instance.Hide();
    }

    public void UpdateValue(PostFull postFull, LoopScrollerValue loopValue, DateTime now)
    {
        bool empty = postFull.PublicationDateTime.Year == 1753;
        loopValue.ItemType = empty ? 0 : postFull.ImageCount == 0 ? 1 : 2;
        loopValue.ItemSize = postFull.ImageCount == 0 ? 430 : 1058;
        loopValue.UserData = empty ? null : new FeedUserData(postFull.PostId, postFull.PublicationDateTime);

        loopValue.SetSprite(0, empty ? null : portraits.Length > 0 ? portraits[postFull.PostId % portraits.Length] : null);
        loopValue.SetText(1, postFull.AppUserAlias);
        loopValue.SetText(2, empty ? null : $"@{postFull.AppUserAlias} - hace {((int)(now - postFull.PublicationDateTime).TotalHours).ToString()} horas");
        loopValue.SetText(3, postFull.Summary);
        loopValue.SetSprite(4, postFull.ImageCount == 0 ? null : postFull.TitleSprite);
        loopValue.SetText(5, postFull.ImageCount < 2 ? null : $"+{(postFull.ImageCount - 1).ToString()}");
    }
}