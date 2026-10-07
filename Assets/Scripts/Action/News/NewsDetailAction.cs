using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    [Serializable]
    public class NewsFullEvent : UnityEvent<NewsFull> { }
    [Serializable]
    public class ReactionEvent : UnityEvent<long, long, bool> { }

    [Space, Title("Details")]
    [SerializeField]
    Text txtAlias = null;
    [SerializeField]
    Text txtTitle = null;
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
    float commentItemSpacing = 40f;
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

    [Title("Configs")]
    [SerializeField]
    FeedDetailConfig[] configs = null;


    [Title("Page")]
    [SerializeField]
    Page pagDetail;

    [Title("Events")]
    [SerializeField]
    ImagesEvent onImagesDisplay = null;

    [SerializeField]
    NewsFullEvent onApplyUpdate = null;

    NewsService newsService;
    PostService postService;

    long postId = -1;
    String url = null;
    float contentInitialHeight = 0.0f;
    long currentReactionPhraseId = -1, previousReactionPhraseId = -1;
    int[] reactionCounts;
    bool isRefresh = false;
    NewsFull newsFull = null;

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

    public bool ChangePagBack()
    {
        PageManager.Instance.ChangePage(configs[StateManager.Instance.FeedDetailType].pagBack);
        return false;
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
        this.newsFull = newsFull;

        postId = newsFull.PostId;

        reactionCounts = (int[])newsFull.ReactionCounts.Clone();
        currentReactionPhraseId = newsFull.ReactionPhraseId;

        if (newsFull.LinkFulls != null && newsFull.LinkFulls.Count > 0)
            url = newsFull.LinkFulls[0].Url;

        // Post
        txtAlias.TextValue = $"@{newsFull.AppUserAlias}";
        txtTitle.TextValue = $"<line-height=70%>{(String.IsNullOrWhiteSpace(newsFull.Title) ? "Noticia" : newsFull.Title)}";

        txtDescription.TextValue = String.IsNullOrWhiteSpace(newsFull.Description) ? "-" : newsFull.Description;

        txtNewsType.TextValue = newsFull.NewsTypeId == -1 ? "-" : vllNewsType.FindRecordCellString(newsFull.NewsTypeId, "Name");
        txtSource.TextValue = String.IsNullOrWhiteSpace(newsFull.Source) ? "-" : newsFull.Source;
        //txtNewsDate.TextValue = newsFull.DateTime == null ? "-" : newsFull.DateTime.Value.ToLocalTime().ToString("d 'de' MMMM, yyyy", new System.Globalization.CultureInfo("es-ES"));
        txtNewsDate.TextValue = newsFull.DateTime == null ? "-" : newsFull.DateTime.Value.ToString("d 'de' MMMM, yyyy", new System.Globalization.CultureInfo("es-ES"));

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

    public void ApplyUpdate()
    {
        onApplyUpdate.Invoke(newsFull);
    }

    // Reaction
    public void ApplyReaction1(bool check)
    {
        if (ignoreReactionEvent)
            return;

        ApplyReaction(check, 1);
    }

    public void ApplyReaction2(bool check)
    {
        if (ignoreReactionEvent)
            return;

        ApplyReaction(check, 2);
    }

    public void ApplyReaction3(bool check)
    {
        if (ignoreReactionEvent)
            return;

        ApplyReaction(check, 3);
    }

    public void ApplyReaction4(bool check)
    {
        if (ignoreReactionEvent)
            return;

        ApplyReaction(check, 4);
    }

    bool ignoreReactionEvent = false;
    public void ApplyReaction(bool check, long reactionPhraseId)
    {
        previousReactionPhraseId = currentReactionPhraseId;

        if (previousReactionPhraseId != -1)
        {
            postService.DeleteReaction(new Reaction(-1, postId, StateManager.Instance.AppUser.Id));

            ChangeReactionCount(previousReactionPhraseId, -1);
            currentReactionPhraseId = -1;

            ignoreReactionEvent = true;
            SetReactionToggle(previousReactionPhraseId, false);
            ignoreReactionEvent = false;
        }

        if (!check)
        {
            configs[StateManager.Instance.FeedDetailType].onReactionChanged.Invoke(previousReactionPhraseId, reactionPhraseId, false);
            return;
        }

        postService.RegisterReaction(new Reaction(reactionPhraseId, postId, StateManager.Instance.AppUser.Id));

        ChangeReactionCount(reactionPhraseId, 1);
        currentReactionPhraseId = reactionPhraseId;

        configs[StateManager.Instance.FeedDetailType].onReactionChanged.Invoke(previousReactionPhraseId, reactionPhraseId, true);
    }

    private void SetReactionToggle(long reactionPhraseId, bool value)
    {
        if (reactionPhraseId == 1)
        {
            SetToggle(tglReaction1, value);
            return;
        }

        if (reactionPhraseId == 2)
        {
            SetToggle(tglReaction2, value);
            return;
        }

        if (reactionPhraseId == 3)
        {
            SetToggle(tglReaction3, value);
            return;
        }

        if (reactionPhraseId == 4)
        {
            SetToggle(tglReaction4, value);
        }
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

    private void RefreshContents(int commentCount)
    {
        RectTransform content = txtDescription.transform.parent.GetComponent<RectTransform>();
        RectTransform rtfDescription = txtDescription.transform.GetComponent<RectTransform>();

        rtfDescription.sizeDelta = new Vector2(rtfDescription.sizeDelta.x,txtDescription.TextHeight);

        DisplayComments(commentCount);

        content.sizeDelta = new Vector2(content.sizeDelta.x, contentInitialHeight + txtDescription.TextHeight + rtfImgComments.sizeDelta.y + contentPadding);

        if (!isRefresh)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    private void DisplayComments(int commentCount)
    {
        RectTransform rtfComment1 = txtComment1.transform.parent.GetComponent<RectTransform>();
        RectTransform rtfComment2 = txtComment2.transform.parent.GetComponent<RectTransform>();
        RectTransform rtfComment3 = txtComment3.transform.parent.GetComponent<RectTransform>();

        rtfComment1.gameObject.SetActive(commentCount > 0);
        rtfComment2.gameObject.SetActive(commentCount > 1);
        rtfComment3.gameObject.SetActive(commentCount > 2);

        float totalHeight = 0f;

        if (commentCount > 0)
        {
            float height = txtComment1.TextHeight + commentItemPadding;
            rtfComment1.sizeDelta = new Vector2(rtfComment1.sizeDelta.x, height);
            totalHeight += height + commentItemSpacing;
        }

        if (commentCount > 1)
        {
            float height = txtComment2.TextHeight + commentItemPadding;
            rtfComment2.sizeDelta = new Vector2(rtfComment2.sizeDelta.x, height);
            totalHeight += height + commentItemSpacing;
        }

        if (commentCount > 2)
        {
            float height = txtComment3.TextHeight + commentItemPadding;
            rtfComment3.sizeDelta = new Vector2(rtfComment3.sizeDelta.x, height);
            totalHeight += height + commentItemSpacing;
        }

        rtfImgComments.sizeDelta = new Vector2(rtfImgComments.sizeDelta.x, totalHeight + commentPadding);
    }
}