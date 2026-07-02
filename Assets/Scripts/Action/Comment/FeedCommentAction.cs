using System;
using UnityEngine;
//using URandom = UnityEngine.Random;

using Leap.UI.Elements;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;

public class FeedCommentAction : MonoBehaviour
{
    [Space]
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
    long postId = -1;
    readonly CommentFull emptyCommentFull = new CommentFull();

    private bool resetting = false;

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

    public void CreateLoopFeed()
    {
        int feedCount = chunkCount * 4;

        valueDates = new String[feedCount];

        loopFeed.ClearValues();
        DateTime utcNow = DateTime.UtcNow;
        for (int k = 0; k < feedCount; k++)
        {
            LoopScrollerValue loopValue = new LoopScrollerValue(loopFeed.LoopItems[0].LoopItem, null);
            UpdateValue(emptyCommentFull, loopValue, utcNow);
            loopFeed.AddValue(loopValue);

            valueDates[k] = "--:--:--:---- : -1";
        }
        loopFeed.ApplyValues();
    }

    public void ResetComments()
    {
        UpdateOverlay(0);

        ifdComment.Clear();
        goEmptyComments.SetActive(false);
        loopFeed.gameObject.SetActive(false);

        resetting = true;
        GetComments(0, new FeedCommentUserData(-1, DateTime.UtcNow), 2);
    }

    public void GetComments(int startLoopIdx, object commentUserData, int direction)
    {
        if (commentUserData == null)
            return;

        FeedCommentUserData feedCommentUserData = (FeedCommentUserData)commentUserData;

        CommentFeedRequest request = new CommentFeedRequest
        {
            Chunk = startLoopIdx,

            StartDateTime = feedCommentUserData.PublicationDateTime,
            Direction = direction,
            Count = feedCommentUserData.PostId == -1 ? chunkCount + chunkCount : chunkCount,

            PostId = postId,
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
        if (response.Direction < 3)
        {
            for (int i = 0; i < response.CommentFulls.Count; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(response.CommentFulls[i], loopFeed[k], utcNow);
                UpdateDebug(k, response.CommentFulls[i]);
            }

            if (resetting)
            {
                for (int i = response.CommentFulls.Count; i < loopFeed.ValuesCount; i++)
                {
                    int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                    UpdateValue(emptyCommentFull, loopFeed[k], utcNow);
                    UpdateDebug(k, emptyCommentFull);
                }
                Invoke(nameof(ResetSelectedIndex), 1f);
                resetting = false;
            }
            else
            {
                for (int i = response.CommentFulls.Count; i < chunkCount; i++)
                {
                    int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                    UpdateValue(emptyCommentFull, loopFeed[k], utcNow);
                    UpdateDebug(k, emptyCommentFull);
                }
            }
        }
        else
        {
            int n = chunkCount - response.CommentFulls.Count;
            for (int i = 0; i < n; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(emptyCommentFull, loopFeed[k], utcNow);
                UpdateDebug(k, emptyCommentFull);
            }

            for (int i = 0; i < response.CommentFulls.Count; i++)
            {
                int k = (startLoopIdx + n + i) % loopFeed.ValuesCount;
                UpdateValue(response.CommentFulls[i], loopFeed[k], utcNow);
                UpdateDebug(k, response.CommentFulls[i]);
            }
        }
        loopFeed.RefreshVisibleValues();

        if (txtDebug != null)
            txtDebug.TextValue = String.Join('\n', valueDates);

        if (response.Direction == 3 && response.CommentFulls.Count > 0)
            loopFeed.SelectedIndex = (startLoopIdx + chunkCount - response.CommentFulls.Count) % loopFeed.ValuesCount;
    }

    private void ResetSelectedIndex()
    {
        loopFeed.SelectedIndex = 0;

        ScreenDialog.Instance.Hide();
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
        if (String.IsNullOrWhiteSpace(ifdComment.Text))
        {
            ChoiceDialog.Instance.Error("Tu comentario tiene que incluir caracteres.");
            return;
        }

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

        Invoke(nameof(ResetComments), 1f);
    }

    // Debug

    String[] valueDates;

    public void UpdateDebug(int k, CommentFull commentFull)
    {
        if (commentFull.PublicationDateTime.Year == 1753)
            valueDates[k] = "<color=red>--:--:--:---- : -1</color>";
        else
            valueDates[k] = $"<color=red>{commentFull.PublicationDateTime.ToString("HH:mm:ss:ffff")} : {(commentFull.Message.Length > 12 ? commentFull.Message[0..12] : commentFull.Message)}</color>";

    }

    public void UpdateOverlay(int idx)
    {
        if (trfOverlay == null)
            return;

        trfOverlay.anchoredPosition = new Vector2(trfOverlay.anchoredPosition.x, -.5f - 22.2f * (idx % loopFeed.ValuesCount));
    }
}