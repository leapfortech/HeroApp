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

public class TaleDetailAction : MonoBehaviour
{
    [Serializable]
    public class ImagesEvent : UnityEvent<List<Sprite>> { }
    [Serializable]
    public class TaleFullEvent : UnityEvent<TaleFull> { }
    [Serializable]
    public class ReactionEvent : UnityEvent<long, long, bool> { }

    [Space, Title("Details")]
    [SerializeField]
    Image imgThumbnail = null;
    [SerializeField]
    Text txtAlias = null;
    [SerializeField]
    Text txtTitle = null;
    [SerializeField]
    Text txtCurrentLocation = null;
    [SerializeField]
    Text txtInterestLocation = null;
    [SerializeField]
    Text txtDescription = null;
    [SerializeField]
    Text txtDate = null;
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
    [SerializeField]
    ValueList vllCity = null;

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
    Page pagDetail = null;

    [Title("Events")]
    [SerializeField]
    ImagesEvent onImagesDisplay = null;
    [SerializeField]
    TaleFullEvent onApplyUpdate = null;

    [SerializeField]
    UnityEvent onDeleted = null;

    long postId = -1;
    float contentInitialHeight = 0.0f;
    long currentReactionPhraseId = -1, previousReactionPhraseId = -1;
    int[] reactionCounts;
    bool isRefresh = false;
    TaleFull taleFull = null;

    TaleService taleService;
    PostService postService;
    float commentTotalinitialHeight = 0.0f, commentItemInitialHeight = 0.0f, commentTextItemInitialHeight = 0.0f;

    private void Awake()
    {
        taleService = GetComponent<TaleService>();
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
        taleService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);
    }

    public bool Refresh()
    {
        isRefresh = true;
        ScreenDialog.Instance.Display();
        taleService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);

        return false;
    }

    public void ApplyFull(TaleFull taleFull)
    {
        this.taleFull = taleFull;

        postId = taleFull.PostId;

        imgThumbnail.Sprite = taleFull.AppUserInfo.ThumbnailSprite;

        reactionCounts = (int[])taleFull.ReactionCounts.Clone();
        currentReactionPhraseId = taleFull.ReactionPhraseId;

        // Post
        txtAlias.TextValue = $"@{taleFull.AppUserAlias}";
        txtTitle.TextValue = $"<line-height=70%>{(String.IsNullOrWhiteSpace(taleFull.Title) ? "Historia" : taleFull.Title)}";
        txtTitle.GetComponent<RectTransform>().sizeDelta = new Vector2(txtTitle.GetComponent<RectTransform>().sizeDelta.x, txtTitle.TextHeight);
        float deltaY = txtTitle.TextHeight;

        String currCountry = taleFull.AppUserInfo.CurrentLocality.CountryId == -1 ? "" : vllCountry.FindRecordCellString(taleFull.AppUserInfo.CurrentLocality.CountryId, "Name");
        String currState = taleFull.AppUserInfo.CurrentLocality.StateId == -1 ? "" : vllState.FindRecordCellString(taleFull.AppUserInfo.CurrentLocality.StateId, "Name");
        String currCity = taleFull.AppUserInfo.CurrentLocality.CityId == -1 ? "" : vllCity.FindRecordCellString(taleFull.AppUserInfo.CurrentLocality.CityId, "Name");

        String intCountry = taleFull.AppUserInfo.InterestLocality.CountryId == -1 ? "" : vllCountry.FindRecordCellString(taleFull.AppUserInfo.InterestLocality.CountryId, "Name");
        String intState = taleFull.AppUserInfo.InterestLocality.StateId == -1 ? "" : vllState.FindRecordCellString(taleFull.AppUserInfo.InterestLocality.StateId, "Name");
        String intCity = taleFull.AppUserInfo.InterestLocality.CityId == -1 ? "" : vllCity.FindRecordCellString(taleFull.AppUserInfo.InterestLocality.CityId, "Name");

        String currentLocation = currCountry + (String.IsNullOrWhiteSpace(currState) ? "" : ", " + currState) + (String.IsNullOrWhiteSpace(currCity) ? "" : ", " + currCity);
        String interestLocation = intCountry + (String.IsNullOrWhiteSpace(intState) ? "" : ", " + intState) + (String.IsNullOrWhiteSpace(intCity) ? "" : ", " + intCity);

        txtCurrentLocation.TextValue = String.IsNullOrWhiteSpace(currCountry + currState + currCity) ? "" : "Vive en: " + currentLocation;
        txtInterestLocation.TextValue = String.IsNullOrWhiteSpace(intCountry + intState + intCity) ? "" : "Originario de: " + interestLocation;

        txtDescription.TextValue = String.IsNullOrWhiteSpace(taleFull.Description) ? "-" : taleFull.Description;
        txtDescription.GetComponent<RectTransform>().sizeDelta = new Vector2(txtDescription.GetComponent<RectTransform>().sizeDelta.x, txtDescription.TextHeight);
        deltaY += txtDescription.TextHeight;

        txtDate.TextValue = taleFull.PublicationDateTime.ToString("dd/MM/yyyy");

        String country = vllCountry.FindRecordCellString(taleFull.PostCountryId, "Name");
        String state = vllState.FindRecordCellString(taleFull.PostStateId, "Name");

        txtLocation.TextValue = country + (String.IsNullOrWhiteSpace(state) ? "" : ", " + state);

        DisplayReactionsCount();

        sttCommentsCount.Title = $"Comentarios ({FeedHelper.Instance.FormatCount(taleFull.CommentCount)})";

        txtAliasComment1.TextValue = taleFull.CommentFulls != null && taleFull.CommentFulls.Count > 0 && taleFull.CommentFulls[0] != null ? taleFull.CommentFulls[0].AppUserAlias : "";
        txtAliasComment2.TextValue = taleFull.CommentFulls != null && taleFull.CommentFulls.Count > 1 && taleFull.CommentFulls[1] != null ? taleFull.CommentFulls[1].AppUserAlias : "";
        txtAliasComment3.TextValue = taleFull.CommentFulls != null && taleFull.CommentFulls.Count > 2 && taleFull.CommentFulls[2] != null ? taleFull.CommentFulls[2].AppUserAlias : "";

        txtDateComment1.TextValue = taleFull.CommentFulls != null && taleFull.CommentFulls.Count > 0 && taleFull.CommentFulls[0] != null ? FeedHelper.Instance.GetFeedDelay(DateTime.Now - taleFull.CommentFulls[0].PublicationDateTime) : "";
        txtDateComment2.TextValue = taleFull.CommentFulls != null && taleFull.CommentFulls.Count > 1 && taleFull.CommentFulls[1] != null ? FeedHelper.Instance.GetFeedDelay(DateTime.Now - taleFull.CommentFulls[0].PublicationDateTime) : "";
        txtDateComment3.TextValue = taleFull.CommentFulls != null && taleFull.CommentFulls.Count > 2 && taleFull.CommentFulls[2] != null ? FeedHelper.Instance.GetFeedDelay(DateTime.Now - taleFull.CommentFulls[0].PublicationDateTime) : "";

        txtComment1.TextValue = taleFull.CommentFulls != null && taleFull.CommentFulls.Count > 0 && taleFull.CommentFulls[0] != null ? taleFull.CommentFulls[0].Message : "";
        txtComment2.TextValue = taleFull.CommentFulls != null && taleFull.CommentFulls.Count > 1 && taleFull.CommentFulls[1] != null ? taleFull.CommentFulls[1].Message : "";
        txtComment3.TextValue = taleFull.CommentFulls != null && taleFull.CommentFulls.Count > 2 && taleFull.CommentFulls[2] != null ? taleFull.CommentFulls[2].Message : "";

        onImagesDisplay.Invoke(taleFull.ImageSprites);

        // Actions
        SetToggle(tglReaction1, taleFull.ReactionPhraseId == 1);
        SetToggle(tglReaction2, taleFull.ReactionPhraseId == 2);
        SetToggle(tglReaction3, taleFull.ReactionPhraseId == 3);
        SetToggle(tglReaction4, taleFull.ReactionPhraseId == 4);

        // Comments
        DisplayComments(taleFull.CommentFulls.Count);

        // Admin
        gobAdmin.SetActive(taleFull.AppUserId == StateManager.Instance.AppUser.Id);

        if (taleFull.AppUserId == StateManager.Instance.AppUser.Id)
            deltaY += gobAdmin.GetComponent<RectTransform>().sizeDelta.y;

        RefreshContents(deltaY);

        PageManager.Instance.ChangePage(pagDetail);
    }

    public void ApplyUpdate()
    {
        onApplyUpdate.Invoke(taleFull);
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

        taleService.Delete(new PostModerationRequest(taleFull.PostId, taleFull.Id));
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