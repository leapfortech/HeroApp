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
    Text txtTitle = null;
    [SerializeField]
    Text txtNewsDate = null;
    [SerializeField]
    Text txtDescription = null;
    [SerializeField]
    Text txtNewsType = null;
    [SerializeField]
    Text txtSource = null;
    [SerializeField]
    Text txtAlias = null;
    [SerializeField]
    Text txtLocation = null;
    [SerializeField]
    Text txtReactionsCount = null;

    [Space, SerializeField]
    Toggle[] tglReactionCounts = null;

    [Space, SerializeField]
    SectionTitle sttCommentsCount = null;

    [SerializeField]
    Text txtAliasComment1 = null;
    [SerializeField]
    Text txtAliasComment2 = null;
    [SerializeField]
    Text txtAliasComment3 = null;

    [SerializeField]
    Text txtDateComment1 = null;
    [SerializeField]
    Text txtDateComment2 = null;
    [SerializeField]
    Text txtDateComment3 = null;

    [SerializeField]
    Text txtComment1 = null;
    [SerializeField]
    Text txtComment2 = null;
    [SerializeField]
    Text txtComment3 = null;

    [Title("ScrollView")]
    [SerializeField]
    UnityEngine.UI.ScrollRect scrollRect;
    [SerializeField]
    float contentPadding = 160f;

    [Title("Comments")]
    [SerializeField]
    RectTransform rtfComments = null;

    [Title("Admin")]
    [SerializeField]
    GameObject gobAdmin = null;

    [Title("Actions")]
    [SerializeField]
    Button btnDelete = null;
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
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;

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

    [SerializeField]
    UnityEvent onDeleted = null;

    NewsService newsService;
    PostService postService;

    long postId = -1;
    String url = null;
    float contentInitialHeight = 0.0f, commentTotalinitialHeight = 0.0f, commentItemInitialHeight = 0.0f, commentTextItemInitialHeight = 0.0f;
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
        RectTransform content = txtTitle.transform.parent.GetComponent<RectTransform>();
        RectTransform rtfAdmin = gobAdmin.GetComponent<RectTransform>();
        contentInitialHeight = content.sizeDelta.y
                               - txtTitle.TextHeight
                               - txtDescription.TextHeight
                               - rtfComments.sizeDelta.y
                               - rtfAdmin.sizeDelta.y;

        RectTransform rtfComment1 = txtComment1.transform.parent.GetComponent<RectTransform>();
        commentTextItemInitialHeight = txtComment1.TextHeight;
        commentItemInitialHeight = rtfComment1.sizeDelta.y;
        commentTotalinitialHeight = rtfComments.sizeDelta.y;

        btnDelete?.AddAction(Delete);
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
        txtTitle.TextValue = $"<line-height=70%>{(String.IsNullOrWhiteSpace(newsFull.Title) ? "Noticia" : newsFull.Title)}";
        txtTitle.GetComponent<RectTransform>().sizeDelta = new Vector2(txtTitle.GetComponent<RectTransform>().sizeDelta.x, txtTitle.TextHeight);
        float deltaY = txtTitle.TextHeight;

        txtNewsDate.TextValue = newsFull.DateTime == null ? "-" : newsFull.DateTime.Value.ToString("dd/MM/yyyy");
        txtNewsType.TextValue = newsFull.NewsTypeId == -1 ? "-" : vllNewsType.FindRecordCellString(newsFull.NewsTypeId, "Name");
        
        txtDescription.TextValue = String.IsNullOrWhiteSpace(newsFull.Description) ? "-" : newsFull.Description;
        txtDescription.GetComponent<RectTransform>().sizeDelta = new Vector2(txtDescription.GetComponent<RectTransform>().sizeDelta.x, txtDescription.TextHeight);
        deltaY += txtDescription.TextHeight;

        txtSource.TextValue = String.IsNullOrWhiteSpace(newsFull.Source) ? "-" : newsFull.Source;
        txtAlias.TextValue = $"@{newsFull.AppUserAlias}";

        String country = vllCountry.FindRecordCellString(newsFull.PostCountryId, "Name");
        String state = vllState.FindRecordCellString(newsFull.PostStateId, "Name");

        txtLocation.TextValue = country + (String.IsNullOrWhiteSpace(state) ? "" : ", " + state);

        DisplayReactionsCount();

        sttCommentsCount.Title = $"Comentarios ({FeedHelper.Instance.FormatCount(newsFull.CommentCount)})";

        txtAliasComment1.TextValue = newsFull.CommentFulls != null && newsFull.CommentFulls.Count > 0 && newsFull.CommentFulls[0] != null ? newsFull.CommentFulls[0].AppUserAlias : "";
        txtAliasComment2.TextValue = newsFull.CommentFulls != null && newsFull.CommentFulls.Count > 1 && newsFull.CommentFulls[1] != null ? newsFull.CommentFulls[1].AppUserAlias : "";
        txtAliasComment3.TextValue = newsFull.CommentFulls != null && newsFull.CommentFulls.Count > 2 && newsFull.CommentFulls[2] != null ? newsFull.CommentFulls[2].AppUserAlias : "";

        txtDateComment1.TextValue = newsFull.CommentFulls != null && newsFull.CommentFulls.Count > 0 && newsFull.CommentFulls[0] != null ? FeedHelper.Instance.GetFeedDelay(DateTime.UtcNow - newsFull.CommentFulls[0].PublicationDateTime) : "";
        txtDateComment2.TextValue = newsFull.CommentFulls != null && newsFull.CommentFulls.Count > 1 && newsFull.CommentFulls[1] != null ? FeedHelper.Instance.GetFeedDelay(DateTime.UtcNow - newsFull.CommentFulls[1].PublicationDateTime) : "";
        txtDateComment3.TextValue = newsFull.CommentFulls != null && newsFull.CommentFulls.Count > 2 && newsFull.CommentFulls[2] != null ? FeedHelper.Instance.GetFeedDelay(DateTime.UtcNow - newsFull.CommentFulls[2].PublicationDateTime) : "";

        txtComment1.TextValue = newsFull.CommentFulls != null && newsFull.CommentFulls.Count > 0 && newsFull.CommentFulls[0] != null ? newsFull.CommentFulls[0].Message : "";
        txtComment2.TextValue = newsFull.CommentFulls != null && newsFull.CommentFulls.Count > 1 && newsFull.CommentFulls[1] != null ? newsFull.CommentFulls[1].Message : "";
        txtComment3.TextValue = newsFull.CommentFulls != null && newsFull.CommentFulls.Count > 2 && newsFull.CommentFulls[2] != null ? newsFull.CommentFulls[2].Message : "";

        onImagesDisplay.Invoke(newsFull.ImageSprites);

        // Actions
        SetToggle(tglReaction1, newsFull.ReactionPhraseId == 1);
        SetToggle(tglReaction2, newsFull.ReactionPhraseId == 2);
        SetToggle(tglReaction3, newsFull.ReactionPhraseId == 3);
        SetToggle(tglReaction4, newsFull.ReactionPhraseId == 4);

        // Comments
        DisplayComments(newsFull.CommentFulls.Count);

        // Admin
        gobAdmin.SetActive(newsFull.AppUserId == StateManager.Instance.AppUser.Id);

        if (newsFull.AppUserId == StateManager.Instance.AppUser.Id)
            deltaY += gobAdmin.GetComponent<RectTransform>().sizeDelta.y;

        RefreshContents(deltaY);

        PageManager.Instance.ChangePage(pagDetail);
    }

    public void ApplyUpdate()
    {
        onApplyUpdate.Invoke(newsFull);
    }

    // Reaction
    private void DisplayReactionsCount()
    {
        long totalReactions = 0;
        for (int i = 0; i < reactionCounts.Length; i++)
        {
            totalReactions += reactionCounts[i];
            tglReactionCounts[i].Title = reactionCounts[i] > 9999 ? "+9999" : reactionCounts[i].ToString();
        }

        txtReactionsCount.TextValue = $"{totalReactions} reacciones";
    }

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

        DisplayReactionsCount();
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
            float height = commentItemInitialHeight - commentTextItemInitialHeight + txtComment1.TextHeight;
            rtfComment1.sizeDelta = new Vector2(rtfComment1.sizeDelta.x, height);
            totalHeight += height;
        }

        if (commentCount > 1)
        {
            float height = commentItemInitialHeight - commentTextItemInitialHeight + txtComment2.TextHeight;
            rtfComment2.sizeDelta = new Vector2(rtfComment2.sizeDelta.x, height);
            totalHeight += height;
        }

        if (commentCount > 2)
        {
            float height = commentItemInitialHeight - commentTextItemInitialHeight + txtComment3.TextHeight;
            rtfComment3.sizeDelta = new Vector2(rtfComment3.sizeDelta.x, height);
            totalHeight += height;
        }

        rtfComments.sizeDelta = new Vector2(rtfComments.sizeDelta.x, commentTotalinitialHeight + totalHeight);
    }

    private void RefreshContents(float deltaY)
    {
        RectTransform content = txtTitle.transform.parent.GetComponent<RectTransform>();

        content.sizeDelta = new Vector2(content.sizeDelta.x, contentInitialHeight + deltaY + rtfComments.sizeDelta.y + contentPadding);

        if (!isRefresh)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    // Delete
    private void Delete()
    {
        ChoiceDialog.Instance.Error("Eliminar publicación", "¿Estas seguro de eliminar la publicación?", DoDelete, null, "De acuerdo", "Regresar");
    }

    private void DoDelete()
    {
        ScreenDialog.Instance.Display();

        newsService.Delete(new PostModerationRequest(newsFull.PostId, newsFull.Id));
    }

    public void ApplyDelete(bool updated)
    {
        if (!updated)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        onDeleted.Invoke();
    }
}