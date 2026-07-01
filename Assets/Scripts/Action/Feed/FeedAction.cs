using System;
using UnityEngine;
//using URandom = UnityEngine.Random;

using Leap.Core.Tools;
using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Dialog;
using Leap.UI.Extensions;

using Sirenix.OdinInspector;

public class FeedAction : MonoBehaviour
{
    [Space]
    [Title("Feed")]
    [SerializeField]
    FeedState feedConfig = null;
    //[SerializeField]
    //bool filterAppUser = false;

    [Title("Loop")]
    [SerializeField]
    LoopScroller loopFeed = null;
    [SerializeField]
    GameObject txtEmpty;

    [Title("Reaction")]
    [SerializeField]
    ComboAdapter cmbReaction = null;

    [Title("Plaint")]
    [SerializeField]
    ComboAdapter cmbPlaintType = null;
    [PropertySpace(6f)]
    [SerializeField]
    ChoiceOption[] options = null;

    [Title("Debug")]
    [SerializeField]
    Text txtDebug = null;
    [SerializeField]
    RectTransform trfOverlay = null;

    [Title("Event")]
    [SerializeField]
    UnityLongEvent onValueSelected = null;

    PostService postService;
    FeedState feedState;
    int feedCount = 0;
    int selectedIdx = -1;
    PostFull emptyPostFull = new PostFull();

    private void Awake()
    {
        postService = GetComponent<PostService>();
    }

    public void CreateFeeds(bool force)
    {
        if (feedState != null && !force)
            return;

        feedState = StateManager.Instance.GetFeedState(feedConfig.FeedKey);
        feedCount = feedState.Count * 4;

        valueDates = new String[feedCount];

        loopFeed.ClearValues();
        DateTime utcNow = DateTime.UtcNow;
        for (int k = 0; k < feedCount; k++)
        {
            loopFeed.AddValue(CreateEmptyValue(emptyPostFull, utcNow));
            valueDates[k] = "--:--:--:---- : -1";
        }
        loopFeed.ApplyValues();

        UpdateOverlay(0);

        txtEmpty.SetActive(false);

        GetPosts(0, new FeedUserData(-1, utcNow), 2);
    }

    public LoopScrollerValue CreateEmptyValue(PostFull postFull, DateTime utcNow)
    {
        LoopScrollerValue loopValue = new LoopScrollerValue(loopFeed.LoopItems[0].LoopItem, null);
        UpdateValue(postFull, loopValue, utcNow);
        return loopValue;
    }

    public void GetPosts(int startLoopIdx, object userData, int direction)
    {
        if (userData == null)
            return;

        FeedUserData feedUserData = (FeedUserData)userData;

        if (feedUserData.PostId == -1 || direction == 3)
            ScreenDialog.Instance.Display();

        PostFeedRequest request = new PostFeedRequest
        {
            Chunk = startLoopIdx,

            StartDateTime = feedUserData.PublicationDateTime,
            Direction = direction,
            Count = feedUserData.PostId == -1 ? feedState.Count + feedState.Count : feedState.Count,

            LikeAppUserId = StateManager.Instance.AppUser.Id,

            PostTypeId = feedState.PostTypeId,
            AppUserId = -1, //filterAppUser ? StateManager.Instance.AppUser.Id : -1,
            //CountryId = countryId,
            //StateId = stateId,
            Status = feedState.Status
        };

        //Debug.Log($"Request : {request.StartDateTime:yyyy/MM/dd HH:mm:ss.fff} [{request.Direction}:{request.Count}]");

        postService.GetPostFeed(request);
    }

    public void ApplyPosts(PostFeedResponse response)
    {
        if (txtDebug != null)
            for (int i = 0; i < valueDates.Length; i++)
                valueDates[i] = valueDates[i].Replace("<color=red>", "").Replace("</color>", "");

        txtEmpty.SetActive(response.Total == 0);

        int startLoopIdx = response.Chunk % loopFeed.ValuesCount;

        //int endLoopIdx = startLoopIdx + loopFeed.PreloadCount;
        //Debug.Log($"ApplyPosts : {startLoopIdx.ToString()} > {endLoopIdx.ToString()}, {response.PostFulls[0].PublicationDateTime.ToString("dd/MM/yyyy")} > {response.PostFulls[^1].PublicationDateTime.ToString("dd/MM/yyyy")}");

        DateTime utcNow = DateTime.UtcNow;
        if (response.Direction < 3)
        {
            for (int i = 0; i < response.PostFulls.Count; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(response.PostFulls[i], loopFeed[k], utcNow);
                UpdateDebug(k, response.PostFulls[i]);
            }

            for (int i = response.PostFulls.Count; i < feedState.Count; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(emptyPostFull, loopFeed[k], utcNow);
                UpdateDebug(k, emptyPostFull);
            }
        }
        else
        {
            int n = feedState.Count - response.PostFulls.Count;
            for (int i = 0; i < n; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(emptyPostFull, loopFeed[k], utcNow);
                UpdateDebug(k, emptyPostFull);
            }

            for (int i = 0; i < response.PostFulls.Count; i++)
            {
                int k = (startLoopIdx + n + i) % loopFeed.ValuesCount;
                UpdateValue(response.PostFulls[i], loopFeed[k], utcNow);
                UpdateDebug(k, response.PostFulls[i]);
            }
        }
        loopFeed.RefreshVisibleValues();

        if (txtDebug != null)
            txtDebug.TextValue = String.Join('\n', valueDates);

        ScreenDialog.Instance.Hide();

        if (response.Direction == 3 && response.PostFulls.Count > 0)
        {
            loopFeed.SelectedIndex = (startLoopIdx + feedState.Count) % loopFeed.ValuesCount;
            int dataIndex = (startLoopIdx + feedState.Count - response.PostFulls.Count) % loopFeed.ValuesCount;
            //Debug.Log($"{loopFeed.SelectedIndex} | {startLoopIdx} | {feedState.Count} | {response.PostFulls.Count} | {dataIndex}");
            loopFeed.SelectSmooth(dataIndex);
        }
    }

    public void UpdateValue(PostFull postFull, LoopScrollerValue loopValue, DateTime utcNow)
    {
        bool empty = postFull.PublicationDateTime.Year == 1753;
        loopValue.ItemIdx = empty ? 0 : postFull.ImageCount == 0 ? 1 : 2;
        loopValue.ItemSize = empty ? 2000 : postFull.ImageCount == 0 ? 460 : 1058;
        loopValue.Reset(loopFeed.LoopItems[loopValue.ItemIdx].LoopItem, empty ? null : new FeedUserData(postFull.PostId, postFull.PublicationDateTime));

        if (empty)
            return;

        loopValue.GetSprite(0)?.Destroy();
        loopValue.SetSprite(0, postFull.ThumbnailSprite);
        loopValue.SetText(1, postFull.Title);

        loopValue.SetText(2, empty ? null : $"@{postFull.AppUserAlias} - {PostHelper.GetFeedDelay(utcNow - postFull.PublicationDateTime)}");
        loopValue.SetText(3, (postFull.Description != null && postFull.Description.Length > 84) ? postFull.Description[0..83] + "..." : postFull.Description);

        if (loopValue.ItemIdx == 2)
        {
            loopValue.GetSprite(4)?.Destroy();
            loopValue.SetSprite(4, postFull.TitleSprite);
            loopValue.SetText(5, postFull.ImageCount < 2 ? null : $"+{(postFull.ImageCount - 1).ToString()}");
        }

        loopValue.SetCheck(0, postFull.Favorite != 0);
        loopValue.SetCheck(1, postFull.Like == 5);
        loopValue.SetCheck(2, postFull.Like == 1);
        loopValue.SetCheck(3, postFull.ReactionPhraseId != -1);
    }

    public void SelectValue(int idx)
    {
        selectedIdx = idx % loopFeed.ValuesCount;
        onValueSelected.Invoke(((FeedUserData)loopFeed[selectedIdx].UserData).PostId);
    }

    public void ApplyDetailPost(Post post, Sprite titleSprite)
    {
        LoopScrollerValue loopValue = loopFeed[selectedIdx];

        Sprite thumbnailSprite = loopValue.GetSprite(0);
        String alias = loopValue.GetText(2);
        bool[] toggles = { loopValue.GetCheck(0), loopValue.GetCheck(0), loopValue.GetCheck(0), loopValue.GetCheck(0) };

        int itemIdx = loopValue.ItemIdx;
        loopValue.ItemIdx = post.ImageCount == 0 ? 1 : 2;
        loopValue.ItemSize = post.ImageCount == 0 ? 460 : 1058;
        if (itemIdx != loopValue.ItemIdx)
            loopValue.Reset(loopFeed.LoopItems[loopValue.ItemIdx].LoopItem, new FeedUserData(post.Id, post.PublicationDateTime));

        loopValue.SetSprite(0, thumbnailSprite);
        loopValue.SetText(1, post.Title);
        loopValue.SetText(2, alias);
        loopValue.SetText(3, (post.Description != null && post.Description.Length > 84) ? post.Description[0..83] + "..." : post.Description);

        if (loopValue.ItemIdx == 2)
        {
            //loopValue.GetSprite(4)?.Destroy();
            loopValue.SetSprite(4, titleSprite.Clone("CPY_" + titleSprite.name, true));
            loopValue.SetText(5, post.ImageCount < 2 ? null : $"+{(post.ImageCount - 1).ToString()}");
        }

        loopValue.SetCheck(0, toggles[0]);
        loopValue.SetCheck(1, toggles[1]);
        loopValue.SetCheck(2, toggles[2]);
        loopValue.SetCheck(3, toggles[3]);

        loopFeed.RefreshVisibleValues();
    }

    // Favorite

    public void ApplyFavorite(int dataIndex, bool check)
    {
        int k = dataIndex % loopFeed.ValuesCount;
        loopFeed[k].SetCheck(0, check);

        Favorite favorite = new Favorite(((FeedUserData)loopFeed[k].UserData).PostId, StateManager.Instance.AppUser.Id);
        if (check)
            postService.RegisterFavorite(favorite);
        else
            postService.DeleteFavorite(favorite);
    }

    public void ApplyDetailFavorite(bool check)
    {
        loopFeed[selectedIdx].SetCheck(0, check);
        loopFeed.RefreshVisibleValues();
    }

    // Like

    public void ApplyLike(int dataIndex, bool check)
    {
        int k = dataIndex % loopFeed.ValuesCount;
        loopFeed[k].SetCheck(1, check);

        Like like = new Like(((FeedUserData)loopFeed[k].UserData).PostId, StateManager.Instance.AppUser.Id, 5);
        if (check)
        {
            loopFeed[k].SetCheck(2, false);
            loopFeed.RefreshVisibleValues();
            postService.UpdateLike(like);
        }
        else
            postService.DeleteLike(like);
    }

    public void ApplyDetailLike(bool check)
    {
        loopFeed[selectedIdx].SetCheck(1, check);
        loopFeed.RefreshVisibleValues();
    }

    // Dislike

    public void ApplyDislike(int dataIndex, bool check)
    {
        int k = dataIndex % loopFeed.ValuesCount;
        loopFeed[k].SetCheck(2, check);

        Like like = new Like(((FeedUserData)loopFeed[k].UserData).PostId, StateManager.Instance.AppUser.Id, 1);
        if (check)
        {
            loopFeed[k].SetCheck(1, false);
            loopFeed.RefreshVisibleValues();
            postService.UpdateLike(like);
        }
        else
        {
            like.Rank = -1;
            postService.DeleteLike(like);
        }
    }

    public void ApplyDetailDislike(bool check)
    {
        loopFeed[selectedIdx].SetCheck(2, check);
        loopFeed.RefreshVisibleValues();
    }

    // Reaction

    public void ApplyReaction(int dataIndex, bool check)
    {
        selectedIdx = dataIndex % loopFeed.ValuesCount;

        loopFeed[selectedIdx].SetCheck(3, false);
        loopFeed.RefreshVisibleValues();

        if (!check)
        {
            postService.DeleteReaction(new Reaction(-1, ((FeedUserData)loopFeed[selectedIdx].UserData).PostId, StateManager.Instance.AppUser.Id));
            return;
        }

        cmbReaction.Combo.Click();
    }

    public void RegisterReaction()
    {
        long reactionPhraseId = cmbReaction.GetSelectedId();

        Reaction reaction = new Reaction(reactionPhraseId, ((FeedUserData)loopFeed[selectedIdx].UserData).PostId, StateManager.Instance.AppUser.Id);
        postService.RegisterReaction(reaction);

        loopFeed[selectedIdx].SetCheck(3, true);
        loopFeed.RefreshVisibleValues();
    }

    public void ApplyDetailReaction(bool check)
    {
        loopFeed[selectedIdx].SetCheck(3, check);
        loopFeed.RefreshVisibleValues();
    }

    // Plaint

    public void DisplayMenu(int dataIndex)
    {
        selectedIdx = dataIndex % loopFeed.ValuesCount;
        ChoiceDialog.Instance.Menu(0, "Opciones", options);
    }

    public void SelectValue()
    {
        SelectValue(selectedIdx);
    }

    public void DisplayPlaintTypes()
    {
        cmbPlaintType.Combo.Click();
    }

    public void ApplyPlaint()
    {
        ScreenDialog.Instance.Display();
        
        long plaintTypeId = cmbPlaintType.GetSelectedId();

        PostPlaint postPlaint = new PostPlaint(plaintTypeId, ((FeedUserData)loopFeed[selectedIdx].UserData).PostId, StateManager.Instance.AppUser.Id);
        postService.RegisterPostPlaint(postPlaint);
    }

    public void PlaintRegistered()
    {
        ChoiceDialog.Instance.Info("Reporte", "Reporte registrado exitosamente.");
    }

    // Debug

    String[] valueDates;

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
}