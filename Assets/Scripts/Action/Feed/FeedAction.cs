using System;
using UnityEngine;
using URandom = UnityEngine.Random;

using Leap.UI.Elements;
using Leap.UI.Dialog;

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

    [Title("Debug")]
    [SerializeField]
    Text txtDebug = null;
    [SerializeField]
    RectTransform trfOverlay = null;

    PostService postService;
    FeedState feedState;
    int feedCount = 0;

    //long countryId = -1;
    //long stateId = -1;

    String[] valueDates;

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

        valueDates = new String[feedCount];

        loopFeed.ClearValues();
        DateTime now = DateTime.Now;
        PostFull emptyPostFull = new PostFull();
        for (int k = 0; k < feedCount; k++)
        {
            //feedState.PostFulls.Add(emptyPostFull);
            loopFeed.AddValue(CreateValue(emptyPostFull, now));
            valueDates[k] = "--:--:--:---- : -1";
        }
        loopFeed.ApplyValues();

        UpdateOverlay(0);

        GetPosts(0, new FeedUserData(-1, DateTime.Now), 2);
    }

    public LoopScrollerValue CreateValue(PostFull postFull, DateTime now)
    {
        LoopScrollerValue loopValue = new LoopScrollerValue(loopFeed.LoopItem, null);
        UpdateValue(postFull, loopValue, now);
        return loopValue;
    }

    public void GetPosts(int startLoopIdx, object userData, int direction)
    {
        FeedUserData feedUserData = (FeedUserData)userData;

        if (feedUserData.PostId == -1)
            ScreenDialog.Instance.Display();

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
        for (int i = 0; i < valueDates.Length; i++)
            valueDates[i] = valueDates[i].Replace("<color=red>", "").Replace("</color>", "");

        if (response.PostFulls.Count == 0)
        {
            ScreenDialog.Instance.Hide();
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
                UpdateDebug(k, response.PostFulls[i]);
            }
        }
        else
        {
            for (int i = 0; i < response.PostFulls.Count; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                //feedState.PostFulls[k] = response.PostFulls[i];
                UpdateValue(response.PostFulls[i], loopFeed[k], now);
                UpdateDebug(k, response.PostFulls[i]);
            }
        }

        if (txtDebug != null)
            txtDebug.TextValue = String.Join('\n', valueDates);
        loopFeed.RefreshVisibleValues();

        txtEmpty.SetActive(response.Total == 0);
        ScreenDialog.Instance.Hide();
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

        int r = URandom.Range(0, 10);
        loopValue.SetCheck(0, r > 7);
        loopValue.SetCheck(1, r < 2);
        loopValue.SetCheck(2, r > 3 && r < 6);
    }

    public void UpdateDebug(int k, PostFull postFull)
    {
        if (postFull.PublicationDateTime.Year == 1753)
            valueDates[k] = "<color=red>--:--:--:---- : -1</color>";
        else
            valueDates[k] = $"<color=red>{postFull.PublicationDateTime.ToString("HH:mm:ss:ffff")} : {postFull.Title}</color>";

    }

    public void UpdateOverlay(int idx)
    {
        if (trfOverlay == null)
            return;

        trfOverlay.anchoredPosition = new Vector2(trfOverlay.anchoredPosition.x, -.5f - 22.2f * (idx % loopFeed.ValuesCount));
    }

    public void FavoriteApply(int dataIndex, bool check)
    {
        Debug.Log($"Favorite {dataIndex} is {(check ? "" : "UN")}CHECKED");
    }

    public void LikeApply(int dataIndex, bool check)
    {
        Debug.Log($"Like {dataIndex} is {(check ? "" : "UN")}CHECKED");
    }

    public void DislikeApply(int dataIndex, bool check)
    {
        Debug.Log($"Dislike {dataIndex} is {(check ? "" : "UN")}CHECKED");
    }
}