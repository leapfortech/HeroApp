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

public class MemoryDetailAction : MonoBehaviour
{
    [Serializable]
    public class ImagesEvent : UnityEvent<List<Sprite>> { }
    [Serializable]
    public class MemoryFullEvent : UnityEvent<MemoryFull> { }

    [Space, Title("Details")]
    //[SerializeField]
    //Image imgThumbnail = null;
    //[SerializeField]
    //Text txtAlias = null;
    //[SerializeField]
    //Text txtDateTime = null;
    [SerializeField]
    Text txtTitle = null;
    [SerializeField]
    Text txtSummary = null;
    [SerializeField]
    Text txtDescription = null;

    [SerializeField]
    Text txtMemoryType = null;
    [SerializeField]
    Text txtPlace = null;
    [SerializeField]
    Text txtDateTime = null;
    [SerializeField]
    Text txtLocation = null;

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

    [SerializeField]
    MemoryFullEvent onApplyUpdate = null;

    MemoryService memoryService;
    PostService postService;

    long postId = -1;
    float contentInitialHeight = 0.0f;
    bool isRefresh = false;
    int[] reactionCounts;
    long reactionPhraseId = -1, currentReactionPhraseId = -1;
    MemoryFull memoryFull = null;

    private void Awake()
    {
        memoryService = GetComponent<MemoryService>();
        postService = GetComponent<PostService>();
    }

    private void Start()
    {
        RectTransform content = txtTitle.transform.parent.GetComponent<RectTransform>();
        contentInitialHeight = content.sizeDelta.y - txtLocation.TextHeight - txtDescription.TextHeight - rtfImgComments.sizeDelta.y;
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
        //imgThumbnail.Sprite = memoryFull.ThumbnailSprite;

        //txtAlias.TextValue = $"@{memoryFull.AppUserAlias}";
        txtTitle.TextValue = $"<line-height=70%>{(String.IsNullOrWhiteSpace(memoryFull.Title) ? "Evento" : memoryFull.Title)}";
        //txtDateTime.TextValue = memoryFull.PublicationDateTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        if (txtSummary != null)
            txtSummary.TextValue = String.IsNullOrWhiteSpace(memoryFull.Summary) ? "-" : memoryFull.Summary;

        txtDescription.TextValue = String.IsNullOrWhiteSpace(memoryFull.Description) ? "-" : memoryFull.Description;

        // Memory
        txtMemoryType.TextValue = memoryFull.MemoryTypeId == -1 ? "-" : vllMemoryType.FindRecordCellString(memoryFull.MemoryTypeId, "Name");

        String country = memoryFull.PostCountryId == -1 ? "" : vllCountry.FindRecordCellString(memoryFull.PostCountryId, "Name");
        String state = memoryFull.PostStateId == -1 ? "" : vllState.FindRecordCellString(memoryFull.PostStateId, "Name");
        txtPlace.TextValue = country + (!String.IsNullOrWhiteSpace(country) && !String.IsNullOrWhiteSpace(state) ? ", " : "") + state;

        txtDateTime.TextValue = memoryFull.DateTime == null ? "-" : memoryFull.DateTime.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
        txtLocation.TextValue = String.IsNullOrWhiteSpace(memoryFull.Location) ? "-" : memoryFull.Location;

        for (int i = 0; i < reactionCounts.Length; i++)
            txtReactionCounts[i].TextValue = reactionCounts[i] > 9999 ? "+9999" : reactionCounts[i].ToString();

        txtCommentCount.TextValue = $"({memoryFull.CommentCount.ToString()})";
        txtComment1.TextValue = memoryFull.CommentFulls != null && memoryFull.CommentFulls.Count > 0 && memoryFull.CommentFulls[0] != null ? memoryFull.CommentFulls[0].Message : "";
        txtComment2.TextValue = memoryFull.CommentFulls != null && memoryFull.CommentFulls.Count > 1 && memoryFull.CommentFulls[1] != null ? memoryFull.CommentFulls[1].Message : "";
        txtComment3.TextValue = memoryFull.CommentFulls != null && memoryFull.CommentFulls.Count > 2 && memoryFull.CommentFulls[2] != null ? memoryFull.CommentFulls[2].Message : "";

        // Images
        goEmptyImages.SetActive(memoryFull.ImageSprites.Count == 0);
        goImages.SetActive(memoryFull.ImageSprites.Count != 0);

        onImagesDisplay.Invoke(memoryFull.ImageSprites);

        // Actions
        SetToggle(tglReaction1, memoryFull.ReactionPhraseId == 1);
        SetToggle(tglReaction2, memoryFull.ReactionPhraseId == 2);
        SetToggle(tglReaction3, memoryFull.ReactionPhraseId == 3);
        SetToggle(tglReaction4, memoryFull.ReactionPhraseId == 4);

        RefreshContents(memoryFull.CommentFulls.Count);

        btnUpdate.gameObject.SetActive(memoryFull.AppUserId == StateManager.Instance.AppUser.Id);

        PageManager.Instance.ChangePage(pagDetail);
    }

    public void ApplyUpdate()
    {
        onApplyUpdate.Invoke(memoryFull);
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

    //

    private void SetToggle(Toggle toggle, bool value)
    {
        if (value)
            toggle.Check();
        else
            toggle.Uncheck();
    }

    private void RefreshContents(int commentCount)
    {
        RectTransform content = txtTitle.transform.parent.GetComponent<RectTransform>();
        RectTransform rtfLocation = txtLocation.transform.GetComponent<RectTransform>();
        RectTransform rtfDescription = txtDescription.transform.GetComponent<RectTransform>();

        rtfLocation.sizeDelta = new Vector2(rtfLocation.sizeDelta.x, txtLocation.TextHeight);
        rtfDescription.sizeDelta = new Vector2(rtfDescription.sizeDelta.x, txtDescription.TextHeight);

        DisplayComments(commentCount);

        content.sizeDelta = new Vector2(content.sizeDelta.x, contentInitialHeight + txtLocation.TextHeight + txtDescription.TextHeight + rtfImgComments.sizeDelta.y + contentPadding);

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