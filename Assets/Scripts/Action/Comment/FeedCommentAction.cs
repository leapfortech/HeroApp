using System;
using UnityEngine;
//using URandom = UnityEngine.Random;

using Leap.UI.Elements;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;

public class FeedCommentAction : MonoBehaviour
{
    [Space]
    //[Title("Feed")]
    //[SerializeField]
    //FeedState feedConfig = null;
    //[SerializeField]
    //bool filterAppUser = false;
    [Title("Config")]
    [SerializeField]
    public int chunkCount = 10;
    [SerializeField]
    public int chunkStatus = 1;

    [Title("Loop")]
    [SerializeField]
    LoopScroller loopFeed = null;

    [Space, Title("Elements")]
    [SerializeField]
    GameObject goEmptyComments = null;
    [SerializeField]
    InputField ifdComment = null;

    [Title("Action")]
    [SerializeField]
    Button btnRegister = null;

    [Title("Debug")]
    [SerializeField]
    Text txtDebug = null;
    [SerializeField]
    RectTransform trfOverlay = null;

    PostService postService;
    int feedCount = 0;
    long postId = -1;
    CommentFull emptyCommentFull = new CommentFull();

    private void Awake()
    {
        postService = GetComponent<PostService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(RegisterComment);
    }

    public void SelectValue(long postId)
    {
        this.postId = postId;
    }


    public void CreateFeeds(bool force)
    {
        if (feedCount != 0 && !force)
            return;

        feedCount = chunkCount * 4;

        valueDates = new String[feedCount];

        loopFeed.ClearValues();
        DateTime utcNow = DateTime.UtcNow;
        for (int k = 0; k < feedCount; k++)
        {
            loopFeed.AddValue(CreateEmptyValue(emptyCommentFull, utcNow));
            valueDates[k] = "--:--:--:---- : -1";
        }
        loopFeed.ApplyValues();

        UpdateOverlay(0);

        goEmptyComments.SetActive(true);
        loopFeed.gameObject.SetActive(false);

        GetComments(0, new FeedCommentUserData(-1, utcNow), 2);
    }

    public LoopScrollerValue CreateEmptyValue(CommentFull commentFull, DateTime utcNow)
    {
        LoopScrollerValue loopValue = new LoopScrollerValue(loopFeed.LoopItems[0].LoopItem, null);
        UpdateValue(commentFull, loopValue, utcNow);
        return loopValue;
    }

    public void GetComments(int startLoopIdx, object commentUserData, int direction)
    {
        FeedCommentUserData feedCommentUserData = (FeedCommentUserData)commentUserData;

        //if (feedCommentUserData.PostId == -1)
        //    ScreenDialog.Instance.Display();

        CommentFeedRequest request = new CommentFeedRequest
        {
            Chunk = startLoopIdx,

            StartDateTime = feedCommentUserData.PublicationDateTime,
            Direction = direction,
            Count = feedCommentUserData.PostId == -1 ? chunkCount + chunkCount : chunkCount,

            PostId = postId,
            AppUserId = -1, //filterAppUser ? StateManager.Instance.AppUser.Id : -1,
            Status = chunkStatus
        };

        postService.GetCommentFeed(request);
    }

    public void ApplyComments(CommentFeedResponse response)
    {
        if (txtDebug != null)
            for (int i = 0; i < valueDates.Length; i++)
                valueDates[i] = valueDates[i].Replace("<color=red>", "").Replace("</color>", "");

        // Comments
        goEmptyComments.SetActive(response.Total == 0);
        loopFeed.gameObject.SetActive(response.Total != 0);

        int startLoopIdx = response.Chunk % loopFeed.ValuesCount;

        DateTime utcNow = DateTime.UtcNow;
        if (response.Direction == 1)
        {
            for (int i = 0; i < response.CommentFulls.Count; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(response.CommentFulls[i], loopFeed[k], utcNow);
                UpdateDebug(k, response.CommentFulls[i]);
            }

            for (int i = response.CommentFulls.Count; i < chunkCount; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(emptyCommentFull, loopFeed[k], utcNow);
                UpdateDebug(k, emptyCommentFull);
            }
        }
        else
        {
            for (int i = 0; i < response.CommentFulls.Count; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(response.CommentFulls[i], loopFeed[k], utcNow);
                UpdateDebug(k, response.CommentFulls[i]);
            }

            for (int i = response.CommentFulls.Count; i < chunkCount; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(emptyCommentFull, loopFeed[k], utcNow);
                UpdateDebug(k, emptyCommentFull);
            }
        }
        loopFeed.RefreshVisibleValues();

        if (txtDebug != null)
            txtDebug.TextValue = String.Join('\n', valueDates);
    }

    public void UpdateValue(CommentFull commentFull, LoopScrollerValue loopValue, DateTime utcNow)
    {
        bool empty = commentFull.PublicationDateTime.Year == 1753;
        loopValue.ItemIdx = empty ? 0 : 1;
        loopValue.ItemSize = empty ? 2000 : 430;
        loopValue.Reset(loopFeed.LoopItems[loopValue.ItemIdx].LoopItem, empty ? null : new FeedCommentUserData(commentFull.PostId, commentFull.PublicationDateTime));

        if (empty)
            return;

        loopValue.SetText(0, commentFull.AppUserAlias);
        loopValue.SetText(1, $"{PostHelper.GetFeedDelay(utcNow - commentFull.PublicationDateTime)}");
        loopValue.SetText(2, commentFull.Message);
    }

    // Register
    public void RegisterComment()
    {
        if (!ifdComment.IsValid())
        {
            ChoiceDialog.Instance.Error(ifdComment.GetValidError());
            return;
        }

        ScreenDialog.Instance.Display();
        Comment comment = new Comment(postId, StateManager.Instance.AppUser.Id, ifdComment.Text);
        postService.RegisterComment(comment);
    }

    public void ApplyComment(long commentId)
    {
        ifdComment.Clear();
        ScreenDialog.Instance.Hide();
        // RM REFRESH LOOP
    }

    // Debug

    String[] valueDates;

    public void UpdateDebug(int k, CommentFull commentFull)
    {
        if (commentFull.PublicationDateTime.Year == 1753)
            valueDates[k] = "<color=red>--:--:--:---- : -1</color>";
        else
            valueDates[k] = $"<color=red>{commentFull.PublicationDateTime.ToString("HH:mm:ss:ffff")} : {commentFull.Message}</color>";

    }

    public void UpdateOverlay(int idx)
    {
        if (trfOverlay == null)
            return;

        trfOverlay.anchoredPosition = new Vector2(trfOverlay.anchoredPosition.x, -.5f - 22.2f * (idx % loopFeed.ValuesCount));
    }
}