using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.Core.Tools;
using Leap.Data.Collections;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.UI.Extensions;

using Sirenix.OdinInspector;

public class NewsDetailAction : MonoBehaviour
{
    [Serializable]
    public class ImagesEvent : UnityEvent<List<Sprite>> { }

    [Space, Title("Details")]
    [SerializeField]
    Image imgThumbnail = null;
    [SerializeField]
    Text txtAlias = null;
    [SerializeField]
    Text txtTitle = null;
    [SerializeField]
    Text txtSummary = null;
    [SerializeField]
    Text txtDescription = null;

    [SerializeField]
    Text txtNewsType = null;
    [SerializeField]
    Text txtSource = null;
    [SerializeField]
    Text txtNewsDate = null;

    [Space, SerializeField]
    Text[] txtReactionCounts = null;

    [Space, SerializeField]
    Text txtCommentCount = null;

    [SerializeField]
    Text txtComment1 = null;
    [SerializeField]
    Text txtComment2 = null;
    [SerializeField]
    Text txtComment3 = null;

    [Title("Images")]
    [SerializeField]
    GameObject goEmptyImages = null;
    [SerializeField]
    GameObject goImages = null;

    [Title("ScrollView")]
    [SerializeField]
    UnityEngine.UI.ScrollRect scrollRect;
    [SerializeField]
    float contentPadding = 160f;

    [Title("Comments")]
    [SerializeField]
    float commentItemPadding = 80f;
    [SerializeField]
    float commentPadding = 220;
    [SerializeField]
    RectTransform rtfImgComments = null;

    [Title("Actions")]
    [SerializeField]
    Button btnUpdate = null;
    [SerializeField]
    Toggle tglReaction1 = null;
    [SerializeField]
    Toggle tglReaction2 = null;
    [SerializeField]
    Toggle tglReaction3 = null;
    [SerializeField]
    Toggle tglReaction4 = null;
    [SerializeField]
    ComboAdapter cmbPlaintType = null;

    [Title("Values")]
    [SerializeField]
    ValueList vllNewsType = null;

    [Title("Page")]
    [SerializeField]
    Page pagDetail;

    [Title("Events")]
    [SerializeField]
    ImagesEvent onImagesDisplay = null;
    [SerializeField]
    UnityBoolEvent onReaction1Changed = null;
    [SerializeField]
    UnityBoolEvent onReaction2Changed = null;
    [SerializeField]
    UnityBoolEvent onReaction3Changed = null;
    [SerializeField]
    UnityBoolEvent onReaction4Changed = null;

    NewsService newsService;
    PostService postService;

    long postId = -1;
    String url = null;
    float contentInitialHeight = 0.0f;
    long reactionPhraseId = -1, currentReactionPhraseId = -1;
    int[] reactionCounts;
    bool isRefresh = false;

    private void Awake()
    {
        newsService = GetComponent<NewsService>();
        postService = GetComponent<PostService>();
    }

    private void Start()
    {
        RectTransform content = txtDescription.transform.parent.GetComponent<RectTransform>();

        contentInitialHeight = content.sizeDelta.y - txtDescription.TextHeight - rtfImgComments.sizeDelta.y;
    }

    public void Display(long postId)
    {
        isRefresh = false;
        ScreenDialog.Instance.Display();
        newsService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);
    }

    public bool Refresh()
    {
        isRefresh = true;
        ScreenDialog.Instance.Display();
        newsService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);

        return false;
    }

    public void ApplyFull(NewsFull newsFull)
    {
        postId = newsFull.PostId;

        reactionCounts = (int[])newsFull.ReactionCounts.Clone();
        currentReactionPhraseId = newsFull.ReactionPhraseId;

        if (newsFull.LinkFulls != null && newsFull.LinkFulls.Count > 0)
            url = newsFull.LinkFulls[0].Url;

        // Post
        imgThumbnail.Sprite = newsFull.ThumbnailSprite;

        txtAlias.TextValue = $"@{newsFull.AppUserAlias}";
        txtTitle.TextValue = $"<line-height=70%>{(String.IsNullOrWhiteSpace(newsFull.Title) ? "Noticia" : newsFull.Title)}";

        if (txtSummary != null)
            txtSummary.TextValue = String.IsNullOrWhiteSpace(newsFull.Summary) ? "-" : newsFull.Summary;

        txtDescription.TextValue = String.IsNullOrWhiteSpace(newsFull.Description) ? "-" : newsFull.Description;

        txtNewsType.TextValue = newsFull.NewsTypeId == -1 ? "-" : vllNewsType.FindRecordCellString(newsFull.NewsTypeId, "Name");
        txtSource.TextValue = String.IsNullOrWhiteSpace(newsFull.Source) ? "-" : newsFull.Source;
        txtNewsDate.TextValue = newsFull.DateTime == null ? "-" : newsFull.DateTime.Value.ToLocalTime().ToString("d 'de' MMMM, yyyy", new System.Globalization.CultureInfo("es-ES"));

        for (int i = 0; i < reactionCounts.Length; i++)
            txtReactionCounts[i].TextValue = reactionCounts[i] > 9999 ? "+9999" : reactionCounts[i].ToString();

        txtCommentCount.TextValue = $"({newsFull.CommentCount.ToString()})";
        txtComment1.TextValue = newsFull.CommentFulls != null && newsFull.CommentFulls.Count > 0 && newsFull.CommentFulls[0] != null ? newsFull.CommentFulls[0].Message : "";
        txtComment2.TextValue = newsFull.CommentFulls != null && newsFull.CommentFulls.Count > 1 && newsFull.CommentFulls[1] != null ? newsFull.CommentFulls[1].Message : "";
        txtComment3.TextValue = newsFull.CommentFulls != null && newsFull.CommentFulls.Count > 2 && newsFull.CommentFulls[2] != null ? newsFull.CommentFulls[2].Message : "";

        // Images
        goEmptyImages.SetActive(newsFull.ImageSprites.Count == 0);
        goImages.SetActive(newsFull.ImageSprites.Count != 0);

        onImagesDisplay.Invoke(newsFull.ImageSprites);

        // Actions
        SetToggle(tglReaction1, newsFull.ReactionPhraseId == 1);
        SetToggle(tglReaction2, newsFull.ReactionPhraseId == 2);
        SetToggle(tglReaction3, newsFull.ReactionPhraseId == 3);
        SetToggle(tglReaction4, newsFull.ReactionPhraseId == 4);

        RefreshContents(newsFull.CommentFulls.Count);

        btnUpdate.gameObject.SetActive(newsFull.AppUserId == StateManager.Instance.AppUser.Id);

        PageManager.Instance.ChangePage(pagDetail);
    }

    // Reaction
    public void ApplyReaction1(bool check)
    {
        reactionPhraseId = 1;
        ApplyReaction(check, reactionPhraseId);
    }

    public void ApplyReaction2(bool check)
    {
        reactionPhraseId = 2;
        ApplyReaction(check, reactionPhraseId);
    }

    public void ApplyReaction3(bool check)
    {
        reactionPhraseId = 3;
        ApplyReaction(check, reactionPhraseId);
    }

    public void ApplyReaction4(bool check)
    {
        reactionPhraseId = 4;
        ApplyReaction(check, reactionPhraseId);
    }

    public void ApplyReaction(bool check, long reactionPhraseId)
    {
        long previousReactionPhraseId = currentReactionPhraseId;

        if (!check)
        {
            postService.DeleteReaction(new Reaction(reactionPhraseId, postId, StateManager.Instance.AppUser.Id));

            ChangeReactionCount(reactionPhraseId, -1);
            currentReactionPhraseId = -1;

            return;
        }

        if (previousReactionPhraseId != -1 && previousReactionPhraseId != reactionPhraseId)
        {
            postService.DeleteReaction(new Reaction(previousReactionPhraseId, postId, StateManager.Instance.AppUser.Id));

            ChangeReactionCount(previousReactionPhraseId, -1);
        }

        postService.RegisterReaction(new Reaction(reactionPhraseId, postId, StateManager.Instance.AppUser.Id));

        ChangeReactionCount(reactionPhraseId, 1);
        currentReactionPhraseId = reactionPhraseId;

        UncheckOtherReactions(reactionPhraseId);
    }

    public void ApplyDetailReaction()
    {
        switch (reactionPhraseId)
        {
            case 1:
                onReaction1Changed.Invoke(tglReaction1.Checked);
                break;

            case 2:
                onReaction2Changed.Invoke(tglReaction2);
                break;

            case 3:
                onReaction3Changed.Invoke(tglReaction3);
                break;

            case 4:
                onReaction4Changed.Invoke(tglReaction4);
                break;
        }
    }

    private void UncheckOtherReactions(long reactionPhraseId)
    {
        if (reactionPhraseId != 1)
            tglReaction1.Uncheck();

        if (reactionPhraseId != 2)
            tglReaction2.Uncheck();

        if (reactionPhraseId != 3)
            tglReaction3.Uncheck();

        if (reactionPhraseId != 4)
            tglReaction4.Uncheck();
    }

    private void ChangeReactionCount(long reactionPhraseId, int amount)
    {
        int index = (int)reactionPhraseId - 1;

        reactionCounts[index] += amount;

        if (reactionCounts[index] < 0)
            reactionCounts[index] = 0;

        txtReactionCounts[index].TextValue = reactionCounts[index] > 9999 ? "+9999" : reactionCounts[index].ToString();
    }

    // Plaint

    public void DisplayPlaintTypes()
    {
        cmbPlaintType.Combo.Click();
    }

    public void ApplyPlaint()
    {
        ScreenDialog.Instance.Display();

        long plaintTypeId = cmbPlaintType.GetSelectedId();

        PostPlaint postPlaint = new PostPlaint(plaintTypeId, postId, StateManager.Instance.AppUser.Id);
        postService.RegisterPostPlaint(postPlaint);
    }

    public void PlaintRegistered()
    {
        ChoiceDialog.Instance.Info("Reporte", "Reporte registrado exitosamente.");
    }

    private void SetToggle(Toggle toggle, bool value)
    {
        if (value)
            toggle.Check();
        else
            toggle.Uncheck();
    }

    //private void RefreshContents()
    //{
    //    RectTransform content = txtDescription.transform.parent.GetComponent<RectTransform>();

    //    content.sizeDelta = new Vector2(content.sizeDelta.x, contentInitialHeight + txtDescription.TextHeight + contentPadding);

    //    if (!isRefresh)
    //        scrollRect.verticalNormalizedPosition = 1f;
    //}

    private void RefreshContents(int commentCount)
    {
        RectTransform content = txtDescription.transform.parent.GetComponent<RectTransform>();

        RectTransform rtfDescription = txtDescription.transform.GetComponent<RectTransform>();
        RectTransform rtfComment1 = txtComment1.transform.parent.GetComponent<RectTransform>();
        RectTransform rtfComment2 = txtComment2.transform.parent.GetComponent<RectTransform>();
        RectTransform rtfComment3 = txtComment3.transform.parent.GetComponent<RectTransform>();

        rtfDescription.sizeDelta = new Vector2(rtfDescription.sizeDelta.x,txtDescription.TextHeight);

        float commentsHeight = 0f;

        if (commentCount > 0)
        {
            rtfComment1.gameObject.SetActive(true);
            rtfComment1.sizeDelta = new Vector2(rtfComment1.sizeDelta.x, txtComment1.TextHeight + commentItemPadding);
            commentsHeight += rtfComment1.sizeDelta.y + 40f;
        }
        else
        {
            rtfComment1.gameObject.SetActive(false);
        }

        if (commentCount > 1)
        {
            rtfComment2.gameObject.SetActive(true);
            rtfComment2.sizeDelta = new Vector2(rtfComment2.sizeDelta.x, txtComment2.TextHeight + commentItemPadding);
            commentsHeight += rtfComment2.sizeDelta.y + 40f;
        }
        else
        {
            rtfComment2.gameObject.SetActive(false);
        }

        if (commentCount > 2)
        {
            rtfComment3.gameObject.SetActive(true);
            rtfComment3.sizeDelta = new Vector2(rtfComment3.sizeDelta.x, txtComment3.TextHeight + commentItemPadding);
            commentsHeight += rtfComment3.sizeDelta.y + 40f;
        }
        else
        {
            rtfComment3.gameObject.SetActive(false);
        }

        if (commentCount > 0)
        {
            rtfImgComments.sizeDelta = new Vector2(rtfImgComments.sizeDelta.x, commentsHeight + commentPadding);
        }
        else
        {
            rtfImgComments.sizeDelta = new Vector2(rtfImgComments.sizeDelta.x, commentPadding);
        }

        content.sizeDelta = new Vector2(content.sizeDelta.x,
                                        contentInitialHeight +
                                        txtDescription.TextHeight +
                                        rtfImgComments.sizeDelta.y +
                                        contentPadding);

        if (!isRefresh)
            scrollRect.verticalNormalizedPosition = 1f;
    }
}