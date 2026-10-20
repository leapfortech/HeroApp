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

public class MemoryDetailAction : MonoBehaviour
{
    [Serializable]
    public class ImagesEvent : UnityEvent<List<Sprite>> { }
    [Serializable]
    public class MemoryFullEvent : UnityEvent<MemoryFull> { }
    [Serializable]
    public class ReactionEvent : UnityEvent<long, long, bool> { }

    [Space, Title("Details")]
    [SerializeField]
    Text txtTitle = null;
    [SerializeField]
    Text txtDateTime = null;
    [SerializeField]
    Text txtMemoryType = null;
    [SerializeField]
    Text txtDescription = null;
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

    [Title("Values")]
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    //[SerializeField]
    //ValueList vllCity = null;
    [SerializeField]
    ValueList vllMemoryType = null;

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
    UnityEvent onDeleted = null;

    [SerializeField]
    MemoryFullEvent onApplyUpdate = null;

    long postId = -1;
    float contentInitialHeight = 0.0f, commentTotalinitialHeight = 0.0f, commentItemInitialHeight = 0.0f, commentTextItemInitialHeight = 0.0f;
    bool isRefresh = false;
    int[] reactionCounts;
    long currentReactionPhraseId = -1, previousReactionPhraseId = -1;

    MemoryService memoryService;
    PostService postService;
    MemoryFull memoryFull = null;

    private void Awake()
    {
        memoryService = GetComponent<MemoryService>();
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
        memoryService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);
    }

    public bool Refresh()
    {
        isRefresh = true;
        ScreenDialog.Instance.Display();
        memoryService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);

        return false;
    }

    public void ApplyFull(MemoryFull memoryFull)
    {
        this.memoryFull = memoryFull;

        postId = memoryFull.PostId;

        reactionCounts = (int[])memoryFull.ReactionCounts.Clone();
        currentReactionPhraseId = memoryFull.ReactionPhraseId;

        // Post
        txtTitle.TextValue = $"<line-height=70%>{(String.IsNullOrWhiteSpace(memoryFull.Title) ? "Evento" : memoryFull.Title)}";
        txtTitle.GetComponent<RectTransform>().sizeDelta = new Vector2(txtTitle.GetComponent<RectTransform>().sizeDelta.x, txtTitle.TextHeight);
        float deltaY = txtTitle.TextHeight;

        txtDescription.TextValue = String.IsNullOrWhiteSpace(memoryFull.Description) ? "-" : memoryFull.Description;
        txtDescription.GetComponent<RectTransform>().sizeDelta = new Vector2(txtDescription.GetComponent<RectTransform>().sizeDelta.x, txtDescription.TextHeight);
        deltaY += txtDescription.TextHeight;

        // Memory
        txtMemoryType.TextValue = memoryFull.MemoryTypeId == -1 ? "-" : vllMemoryType.FindRecordCellString(memoryFull.MemoryTypeId, "Name");

        String country = memoryFull.PostCountryId == -1 ? "" : vllCountry.FindRecordCellString(memoryFull.PostCountryId, "Name");
        String state = memoryFull.PostStateId == -1 ? "" : vllState.FindRecordCellString(memoryFull.PostStateId, "Name");
        txtLocation.TextValue = country + (!String.IsNullOrWhiteSpace(country) && !String.IsNullOrWhiteSpace(state) ? ", " : "") + state;

        //txtDateTime.TextValue = memoryFull.DateTime == null ? "-" : memoryFull.DateTime.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
        txtDateTime.TextValue = memoryFull.DateTime == null ? "-" : memoryFull.DateTime.Value.ToString("dd/MM/yyyy HH:mm");

        DisplayReactionsCount();

        sttCommentsCount.Title = $"Comentarios ({FeedHelper.Instance.FormatCount(memoryFull.CommentCount)})";

        txtAliasComment1.TextValue = memoryFull.CommentFulls != null && memoryFull.CommentFulls.Count > 0 && memoryFull.CommentFulls[0] != null ? memoryFull.CommentFulls[0].AppUserAlias : "";
        txtAliasComment2.TextValue = memoryFull.CommentFulls != null && memoryFull.CommentFulls.Count > 1 && memoryFull.CommentFulls[1] != null ? memoryFull.CommentFulls[1].AppUserAlias : "";
        txtAliasComment3.TextValue = memoryFull.CommentFulls != null && memoryFull.CommentFulls.Count > 2 && memoryFull.CommentFulls[2] != null ? memoryFull.CommentFulls[2].AppUserAlias : "";

        txtDateComment1.TextValue = memoryFull.CommentFulls != null && memoryFull.CommentFulls.Count > 0 && memoryFull.CommentFulls[0] != null ? FeedHelper.Instance.GetFeedDelay(DateTime.Now - memoryFull.CommentFulls[0].PublicationDateTime) : "";
        txtDateComment2.TextValue = memoryFull.CommentFulls != null && memoryFull.CommentFulls.Count > 1 && memoryFull.CommentFulls[1] != null ? FeedHelper.Instance.GetFeedDelay(DateTime.Now - memoryFull.CommentFulls[0].PublicationDateTime) : "";
        txtDateComment3.TextValue = memoryFull.CommentFulls != null && memoryFull.CommentFulls.Count > 2 && memoryFull.CommentFulls[2] != null ? FeedHelper.Instance.GetFeedDelay(DateTime.Now - memoryFull.CommentFulls[0].PublicationDateTime) : "";

        txtComment1.TextValue = memoryFull.CommentFulls != null && memoryFull.CommentFulls.Count > 0 && memoryFull.CommentFulls[0] != null ? memoryFull.CommentFulls[0].Message : "";
        txtComment2.TextValue = memoryFull.CommentFulls != null && memoryFull.CommentFulls.Count > 1 && memoryFull.CommentFulls[1] != null ? memoryFull.CommentFulls[1].Message : "";
        txtComment3.TextValue = memoryFull.CommentFulls != null && memoryFull.CommentFulls.Count > 2 && memoryFull.CommentFulls[2] != null ? memoryFull.CommentFulls[2].Message : "";

        onImagesDisplay.Invoke(memoryFull.ImageSprites);

        // Actions
        SetToggle(tglReaction1, memoryFull.ReactionPhraseId == 1);
        SetToggle(tglReaction2, memoryFull.ReactionPhraseId == 2);
        SetToggle(tglReaction3, memoryFull.ReactionPhraseId == 3);
        SetToggle(tglReaction4, memoryFull.ReactionPhraseId == 4);

        // Comments
        DisplayComments(memoryFull.CommentFulls.Count);

        // Admin
        gobAdmin.SetActive(memoryFull.AppUserId == StateManager.Instance.AppUser.Id);

        if (memoryFull.AppUserId == StateManager.Instance.AppUser.Id)
            deltaY += gobAdmin.GetComponent<RectTransform>().sizeDelta.y;

        // Contents
        RefreshContents(deltaY);

        PageManager.Instance.ChangePage(pagDetail);
    }

    public void ApplyUpdate()
    {
        onApplyUpdate.Invoke(memoryFull);
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

    //

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

        memoryService.Delete(new PostModerationRequest(memoryFull.PostId, memoryFull.Id));
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