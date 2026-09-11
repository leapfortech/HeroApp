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

public class TaleDetailAction : MonoBehaviour
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
    Text txtCurrentLocation = null;
    [SerializeField]
    Text txtInterestLocation = null;
    [SerializeField]
    Text txtSummary = null;
    [SerializeField]
    Text txtDescription = null;

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
    [SerializeField]
    ValueList vllCity = null;

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

    TaleService taleService;
    PostService postService;

    long postId = -1;
    float contentInitialHeight = 0.0f;
    long reactionPhraseId = -1, currentReactionPhraseId = -1;
    int[] reactionCounts;
    bool isRefresh = false;

    private void Awake()
    {
        taleService = GetComponent<TaleService>();
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
        postId = taleFull.PostId;

        reactionCounts = (int[])taleFull.ReactionCounts.Clone();
        currentReactionPhraseId = taleFull.ReactionPhraseId;

        // Post
        imgThumbnail.Sprite = taleFull.ThumbnailSprite;

        txtAlias.TextValue = $"@{taleFull.AppUserAlias}";
        txtTitle.TextValue = $"<line-height=70%>{(String.IsNullOrWhiteSpace(taleFull.Title) ? "Historia" : taleFull.Title)}";

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
        
        if (txtSummary != null)
            txtSummary.TextValue = String.IsNullOrWhiteSpace(taleFull.Summary) ? "-" : taleFull.Summary;

        txtDescription.TextValue = String.IsNullOrWhiteSpace(taleFull.Description) ? "-" : taleFull.Description;

        for(int i = 0; i < reactionCounts.Length; i++)
            txtReactionCounts[i].TextValue = reactionCounts[i] > 9999 ? "+9999" : reactionCounts[i].ToString();

        txtCommentCount.TextValue = $"({taleFull.CommentCount.ToString()})";
        txtComment1.TextValue = taleFull.CommentFulls != null && taleFull.CommentFulls.Count > 0 && taleFull.CommentFulls[0] != null ? taleFull.CommentFulls[0].Message : "";
        txtComment2.TextValue = taleFull.CommentFulls != null && taleFull.CommentFulls.Count > 1 && taleFull.CommentFulls[1] != null ? taleFull.CommentFulls[1].Message : "";
        txtComment3.TextValue = taleFull.CommentFulls != null && taleFull.CommentFulls.Count > 2 && taleFull.CommentFulls[2] != null ? taleFull.CommentFulls[2].Message : "";

        // Images
        goEmptyImages.SetActive(taleFull.ImageSprites.Count == 0);
        goImages.SetActive(taleFull.ImageSprites.Count != 0);

        onImagesDisplay.Invoke(taleFull.ImageSprites);

        // Actions
        SetToggle(tglReaction1, taleFull.ReactionPhraseId == 1);
        SetToggle(tglReaction2, taleFull.ReactionPhraseId == 2);
        SetToggle(tglReaction3, taleFull.ReactionPhraseId == 3);
        SetToggle(tglReaction4, taleFull.ReactionPhraseId == 4);

        RefreshContents(taleFull.CommentFulls.Count);

        btnUpdate.gameObject.SetActive(taleFull.AppUserId == StateManager.Instance.AppUser.Id);

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
        RectTransform content = txtDescription.transform.parent.GetComponent<RectTransform>();
        RectTransform rtfDescription = txtDescription.transform.GetComponent<RectTransform>();

        rtfDescription.sizeDelta = new Vector2(rtfDescription.sizeDelta.x, txtDescription.TextHeight);

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