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
    public int count = 10;
    [SerializeField]
    public int status = 1;

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
    //FeedState feedState;
    int feedCount = 0;
    long postId = -1;

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

        feedCount = count * 4;

        valueDates = new String[feedCount];

        loopFeed.ClearValues();
        DateTime now = DateTime.UtcNow;
        CommentFull emptyCommentFull = new CommentFull();
        for (int k = 0; k < feedCount; k++)
        {
            loopFeed.AddValue(CreateValue(emptyCommentFull, now));
            valueDates[k] = "--:--:--:---- : -1";
        }
        loopFeed.ApplyValues();

        UpdateOverlay(0);

        goEmptyComments.SetActive(false);
        loopFeed.gameObject.SetActive(false);

        GetComments(0, new FeedCommentUserData(-1, now), 2);
    }

    public LoopScrollerValue CreateValue(CommentFull commentFull, DateTime now)
    {
        LoopScrollerValue loopValue = new LoopScrollerValue(loopFeed.LoopItems[0].LoopItem, null);
        UpdateValue(commentFull, loopValue, now);
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
            Count = feedCommentUserData.PostId == -1 ? count + count : count,

            PostId = postId,
            AppUserId = -1, //filterAppUser ? StateManager.Instance.AppUser.Id : -1,
            Status = status
        };

        //Debug.Log($"Request : {request.StartDateTime:yyyy/MM/dd HH:mm:ss.fff} [{request.Direction}:{request.Count}]");

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

        if (response.CommentFulls.Count == 0)
        {
            if (txtDebug != null)
                txtDebug.TextValue = String.Join('\n', valueDates);

            //ScreenDialog.Instance.Hide();
            return;
        }

        //response.Chunk = response.Chunk == -1 ? 0 : (response.Chunk + feedState.Count);
        int startLoopIdx = response.Chunk % loopFeed.ValuesCount;
        //int endLoopIdx = startLoopIdx + loopFeed.PreloadCount;
        int direction = response.Direction;

        //Debug.Log($"ApplyPosts : {startLoopIdx.ToString()} > {endLoopIdx.ToString()}, {response.PostFulls[0].PublicationDateTime.ToString("dd/MM/yyyy")} > {response.PostFulls[^1].PublicationDateTime.ToString("dd/MM/yyyy")}");

        DateTime now = DateTime.Now;
        if (direction == 1)
        {
            for (int i = 0; i < response.CommentFulls.Count; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;  // - i - 1)
                //if (k < 0)
                //    break;
                //feedState.PostFulls[k] = response.PostFulls[i];
                UpdateValue(response.CommentFulls[i], loopFeed[k], now);
                UpdateDebug(k, response.CommentFulls[i]);
            }
        }
        else
        {
            for (int i = 0; i < response.CommentFulls.Count; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                //feedState.PostFulls[k] = response.PostFulls[i];
                UpdateValue(response.CommentFulls[i], loopFeed[k], now);
                UpdateDebug(k, response.CommentFulls[i]);
            }
        }

        if (txtDebug != null)
            txtDebug.TextValue = String.Join('\n', valueDates);
        loopFeed.RefreshVisibleValues();

        //ScreenDialog.Instance.Hide();
    }

    public void UpdateValue(CommentFull commentFull, LoopScrollerValue loopValue, DateTime utcNow)
    {
        bool empty = commentFull.PublicationDateTime.Year == 1753;
        //loopValue.ItemType = empty ? 0 : commentFull.ImageCount == 0 ? 1 : 2;
        loopValue.ItemSize = 430;
        loopValue.UserData = empty ? null : new FeedUserData(commentFull.PostId, commentFull.PublicationDateTime);

        loopValue.SetText(0, commentFull.AppUserAlias);
        loopValue.SetText(1, empty ? null : $"{PostHelper.GetFeedDelay(utcNow - commentFull.PublicationDateTime)}");
        loopValue.SetText(2, commentFull.Message);

        //int r = URandom.Range(0, 10);
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