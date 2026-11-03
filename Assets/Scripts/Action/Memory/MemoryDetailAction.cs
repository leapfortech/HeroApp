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
    Text txtAlias = null;
    [SerializeField]
    Text txtLocation = null;

    [Title("Reactions")]
    [SerializeField]
    Text txtReactionsCount = null;

    [Space, SerializeField]
    Toggle[] tglReactions = null;

    [Title("Comments")]
    [SerializeField]
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

    [SerializeField]
    Image imgThumbnailComment1 = null;
    [SerializeField]
    Image imgThumbnailComment2 = null;
    [SerializeField]
    Image imgThumbnailComment3 = null;


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
    RectTransform rtfAdmin = null;

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
    float contentInitialHeight = 0.0f, commentTotalInitialHeight = 0.0f, commentItemInitialHeight = 0.0f, commentTextItemInitialHeight = 0.0f;
    bool isRefresh = false;
    int[] reactionCounts;
    long currentReactionPhraseId = -1, previousReactionPhraseId = -1;

    MemoryService memoryService;
    MemoryFull memoryFull = null;

    RectTransform content, rtfTitle, rtfDescription, rtfComment1, rtfComment2, rtfComment3;

    private void Awake()
    {
        memoryService = GetComponent<MemoryService>();

        content = (RectTransform)txtTitle.transform.parent;

        rtfTitle = (RectTransform)txtTitle.transform;
        rtfDescription = (RectTransform)txtDescription.transform;

        rtfComment1 = (RectTransform)txtComment1.transform.parent;
        rtfComment2 = (RectTransform)txtComment2.transform.parent;
        rtfComment3 = (RectTransform)txtComment3.transform.parent;
    }

    private void Start()
    {
        contentInitialHeight = content.sizeDelta.y
                               - txtTitle.TextHeight
                               - txtDescription.TextHeight
                               - rtfComments.sizeDelta.y
                               - rtfAdmin.sizeDelta.y;
        
        commentTextItemInitialHeight = txtComment1.TextHeight;
        commentItemInitialHeight = rtfComment1.sizeDelta.y;
        commentTotalInitialHeight = rtfComments.sizeDelta.y;

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

    public void Refresh()
    {
        isRefresh = true;
        ScreenDialog.Instance.Display();
        memoryService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);
    }

    public void ApplyFull(MemoryFull memoryFull)
    {
        this.memoryFull = memoryFull;

        postId = memoryFull.PostId;

        reactionCounts = (int[])memoryFull.ReactionCounts.Clone();
        currentReactionPhraseId = memoryFull.ReactionPhraseId;

        // Post
        txtTitle.TextValue = $"<line-height=70%>{(String.IsNullOrWhiteSpace(memoryFull.Title) ? "Evento" : memoryFull.Title)}";
        rtfTitle.sizeDelta = new Vector2(rtfTitle.sizeDelta.x, txtTitle.TextHeight);
        float deltaY = txtTitle.TextHeight;

        txtDescription.TextValue = String.IsNullOrWhiteSpace(memoryFull.Description) ? "-" : memoryFull.Description;
        rtfDescription.sizeDelta = new Vector2(rtfDescription.sizeDelta.x, txtDescription.TextHeight);
        deltaY += txtDescription.TextHeight;

        // Memory
        txtMemoryType.TextValue = memoryFull.MemoryTypeId == -1 ? "-" : vllMemoryType.FindRecordCellString(memoryFull.MemoryTypeId, "Name");

        txtAlias.TextValue = $"@{memoryFull.AppUserAlias}";

        String country = memoryFull.PostCountryId == -1 ? "" : vllCountry.FindRecordCellString(memoryFull.PostCountryId, "Name");
        String state = memoryFull.PostStateId == -1 ? "" : vllState.FindRecordCellString(memoryFull.PostStateId, "Name");
        txtLocation.TextValue = country + (!String.IsNullOrWhiteSpace(country) && !String.IsNullOrWhiteSpace(state) ? ", " : "") + state;

        txtDateTime.TextValue = memoryFull.DateTime == null ? "-" : memoryFull.DateTime.Value.ToString("dd/MM/yyyy 'a las' HH:mm");

        DisplayReactionsCount();

        sttCommentsCount.Title = $"Comentarios ({FeedHelper.Instance.FormatCount(memoryFull.CommentCount)})";

        bool hasComments = memoryFull.CommentFulls != null && memoryFull.CommentFulls.Count > 0 && memoryFull.CommentFulls[0] != null;
        txtAliasComment1.TextValue = hasComments ? memoryFull.CommentFulls[0].AppUserAlias : "";
        txtDateComment1.TextValue = hasComments ? FeedHelper.Instance.GetFeedDelay(DateTime.UtcNow - memoryFull.CommentFulls[0].PublicationDateTime) : "";
        txtComment1.TextValue = hasComments ? memoryFull.CommentFulls[0].Message : "";
        imgThumbnailComment1.Sprite = hasComments ? memoryFull.CommentFulls[0].ThumbnailSprite : null;

        hasComments = memoryFull.CommentFulls != null && memoryFull.CommentFulls.Count > 1 && memoryFull.CommentFulls[1] != null;
        txtAliasComment2.TextValue = hasComments ? memoryFull.CommentFulls[1].AppUserAlias : "";
        txtDateComment2.TextValue = hasComments ? FeedHelper.Instance.GetFeedDelay(DateTime.UtcNow - memoryFull.CommentFulls[1].PublicationDateTime) : "";
        txtComment2.TextValue = hasComments ? memoryFull.CommentFulls[1].Message : "";
        imgThumbnailComment2.Sprite = hasComments ? memoryFull.CommentFulls[1].ThumbnailSprite : null;

        hasComments = memoryFull.CommentFulls != null && memoryFull.CommentFulls.Count > 2 && memoryFull.CommentFulls[2] != null;
        txtAliasComment3.TextValue = hasComments ? memoryFull.CommentFulls[2].AppUserAlias : "";
        txtDateComment3.TextValue = hasComments ? FeedHelper.Instance.GetFeedDelay(DateTime.UtcNow - memoryFull.CommentFulls[2].PublicationDateTime) : "";
        txtComment3.TextValue = hasComments ? memoryFull.CommentFulls[2].Message : "";
        imgThumbnailComment3.Sprite = hasComments ? memoryFull.CommentFulls[2].ThumbnailSprite : null;

        onImagesDisplay.Invoke(memoryFull.ImageSprites);

        // Actions
        for (int i = 0; i < tglReactions.Length; i++)
        {
            tglReactions[i].Checked = memoryFull.ReactionPhraseId == (i + 1);
            ThemeManager.Instance.RefreshStylesInHierarchy(tglReactions[i].gameObject);
        }

        // Comments
        DisplayComments(memoryFull.CommentFulls.Count);
        configs[StateManager.Instance.FeedDetailType].onCommentChanged.Invoke(memoryFull.CommentCount);

        // Admin
        if (memoryFull.AppUserId == StateManager.Instance.AppUser.Id)
        {
            rtfAdmin.gameObject.SetActive(true);
            deltaY += rtfAdmin.sizeDelta.y;
        }
        else
            rtfAdmin.gameObject.SetActive(false);

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
            tglReactions[i].Title = reactionCounts[i] > 9999 ? "+9999" : reactionCounts[i].ToString();
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
            memoryService.DeleteReaction(new Reaction(-1, postId, StateManager.Instance.AppUser.Id));

            ChangeReactionCount(previousReactionPhraseId, -1);
            currentReactionPhraseId = -1;

            ignoreReactionEvent = true;
            tglReactions[previousReactionPhraseId - 1].Checked = false;
            ignoreReactionEvent = false;
        }

        if (!check)
        {
            configs[StateManager.Instance.FeedDetailType].onReactionChanged.Invoke(previousReactionPhraseId, reactionPhraseId, false);
            return;
        }

        memoryService.RegisterReaction(new Reaction(reactionPhraseId, postId, StateManager.Instance.AppUser.Id));

        ChangeReactionCount(reactionPhraseId, 1);
        currentReactionPhraseId = reactionPhraseId;

        configs[StateManager.Instance.FeedDetailType].onReactionChanged.Invoke(previousReactionPhraseId, reactionPhraseId, true);
    }

    private void ChangeReactionCount(long reactionPhraseId, int amount)
    {
        long index = reactionPhraseId - 1;

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
        memoryService.RegisterPostPlaint(postPlaint);
    }

    public void PlaintRegistered()
    {
        ChoiceDialog.Instance.Info("Reporte", "Reporte registrado exitosamente.");
    }

    //

    private void DisplayComments(int commentCount)
    {
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

        rtfComments.sizeDelta = new Vector2(rtfComments.sizeDelta.x, commentTotalInitialHeight + totalHeight);
    }

    private void RefreshContents(float deltaY)
    {
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