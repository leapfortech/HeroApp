using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.Core.Tools;
using Leap.Graphics.Tools;
using Leap.Data.Collections;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.UI.Extensions;

using Sirenix.OdinInspector;

public class HappeningDetailAction : MonoBehaviour
{
    [Serializable]
    public class ImagesEvent : UnityEvent<List<Sprite>> { }

    [Space, Title("Details")]
    [SerializeField]
    Image imgThumbnail = null;
    [SerializeField]
    Text txtAlias = null;
    [SerializeField]
    Text txtDateTime = null;
    [SerializeField]
    Text txtTitle = null;
    [SerializeField]
    Text txtSummary = null;
    [SerializeField]
    Text txtDescription = null;

    [SerializeField]
    Text txtHappeningType = null;
    [SerializeField]
    Text txtPlace = null;
    [SerializeField]
    Text txtIsPublic = null;
    [SerializeField]
    Text txtHasSignup = null;
    [SerializeField]
    Text txtHasPayment = null;
    [SerializeField]
    Text txtPaymentDetails = null;
    [SerializeField]
    Text txtStartDateTime = null;
    [SerializeField]
    Text txtEndDateTime = null;
    [SerializeField]
    Text txtLocation = null;

    [Space, Title("Images")]
    [SerializeField]
    GameObject goEmptyImages = null;
    [SerializeField]
    GameObject goImages = null;

    [Space, Title("Contents")]
    [SerializeField]
    int charsPerLine = 40;
    [SerializeField]
    int lineHeight = 15;
    [SerializeField]
    float contentPadding = 40f;
    [Space, SerializeField]
    RectTransform[] contents = null;

    [Space, Title("Values")]
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    //[SerializeField]
    //ValueList vllCity = null;
    [SerializeField]
    ValueList vllHappeningType = null;

    [Space, Title("Actions")]
    [SerializeField]
    Button btnUpdate = null;
    [SerializeField]
    Toggle tglFavorite = null;
    [SerializeField]
    Toggle tglLike = null;
    [SerializeField]
    Toggle tglDislike = null;
    [SerializeField]
    Toggle tglReaction = null;
    [SerializeField]
    ComboAdapter cmbReaction = null;
    [SerializeField]
    ComboAdapter cmbPlaintType = null;

    [Space, Title("Page")]
    [SerializeField]
    Page pagDetail;

    [Space, Title("Events")]
    [SerializeField]
    ImagesEvent onImagesDisplay = null;
    [SerializeField]
    UnityBoolEvent onFavoriteChanged = null;
    [SerializeField]
    UnityBoolEvent onLikeChanged = null;
    [SerializeField]
    UnityBoolEvent onDislikeChanged = null;
    [SerializeField]
    UnityBoolEvent onReactionChanged = null;

    HappeningService happeningService;
    PostService postService;

    long postId = -1;

    private void Awake()
    {
        happeningService = GetComponent<HappeningService>();
        postService = GetComponent<PostService>();
    }

    public void Display(long postId)
    {
        ScreenDialog.Instance.Display();
        happeningService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);
    }

    public void ApplyFull(HappeningFull happeningFull)
    {
        postId = happeningFull.PostId;

        // Post
        imgThumbnail.Sprite = happeningFull.ThumbnailSprite;

        txtAlias.TextValue = $"@{happeningFull.AppUserAlias}";
        txtTitle.TextValue = String.IsNullOrWhiteSpace(happeningFull.Title) ? "Evento" : happeningFull.Title;
        txtDateTime.TextValue = happeningFull.PublicationDateTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        if (txtSummary != null)
            txtSummary.TextValue = String.IsNullOrWhiteSpace(happeningFull.Summary) ? "-" : happeningFull.Summary;

        txtDescription.TextValue = String.IsNullOrWhiteSpace(happeningFull.Description) ? "-" : happeningFull.Description;

        // Happening
        txtHappeningType.TextValue = happeningFull.HappeningTypeId == -1 ? "-" : vllHappeningType.FindRecordCellString(happeningFull.HappeningTypeId, "Name");

        String country = happeningFull.PostCountryId == -1 ? "" : vllCountry.FindRecordCellString(happeningFull.PostCountryId, "Name");
        String state = happeningFull.PostStateId == -1 ? "" : vllState.FindRecordCellString(happeningFull.PostStateId, "Name");
        txtPlace.TextValue = country + (!String.IsNullOrWhiteSpace(country) && !String.IsNullOrWhiteSpace(state) ? ", " : "") + state;

        txtIsPublic.TextValue = happeningFull.IsPublic == -1 ? "-" : happeningFull.IsPublic == 0 ? "No" : "Sí";
        txtHasSignup.TextValue = happeningFull.HasSignup == -1 ? "-" : happeningFull.HasSignup == 0 ? "No" : "Sí";
        txtHasPayment.TextValue = happeningFull.HasPayment == -1 ? "-" : happeningFull.HasPayment == 0 ? "No" : "Sí";
        txtPaymentDetails.TextValue = String.IsNullOrWhiteSpace(happeningFull.PaymentDetails) ? "-" : happeningFull.PaymentDetails;
        txtStartDateTime.TextValue = happeningFull.StartDateTime == null ? "-" : happeningFull.StartDateTime.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
        txtEndDateTime.TextValue = happeningFull.EndDateTime == null ? "-" : happeningFull.EndDateTime.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
        txtLocation.TextValue = String.IsNullOrWhiteSpace(happeningFull.Location) ? "-" : happeningFull.Location;

        // Images
        goEmptyImages.SetActive(happeningFull.ImageSprites.Count == 0);
        goImages.SetActive(happeningFull.ImageSprites.Count != 0);

        onImagesDisplay.Invoke(happeningFull.ImageSprites);

        // Actions
        SetToggle(tglFavorite, happeningFull.Favorite != 0);
        SetToggle(tglLike, happeningFull.Like == 5);
        SetToggle(tglDislike, happeningFull.Like == 1);
        SetToggle(tglReaction, happeningFull.ReactionPhraseId != -1);

        RefreshContents();

        btnUpdate.gameObject.SetActive(happeningFull.AppUserId == StateManager.Instance.AppUser.Id);

        PageManager.Instance.ChangePage(pagDetail);
    }

    // Favorite

    public void ApplyFavorite(bool check)
    {
        Favorite favorite = new Favorite(postId, StateManager.Instance.AppUser.Id);
        if (check)
            postService.RegisterFavorite(favorite);
        else
            postService.DeleteFavorite(favorite);
    }

    public void ApplyDetailFavorite()
    {
        onFavoriteChanged.Invoke(tglFavorite.Checked);
    }

    // Like

    public void ApplyLike(bool check)
    {
        Like like = new Like(postId, StateManager.Instance.AppUser.Id, 5);
        if (check)
        {
            tglDislike.Uncheck();
            postService.UpdateLike(like);
        }
        else
            postService.DeleteLike(like);
    }

    public void ApplyDislike(bool check)
    {
        Like like = new Like(postId, StateManager.Instance.AppUser.Id, 1);
        if (check)
        {
            tglLike.Uncheck();
            postService.UpdateLike(like);
        }
        else
        {
            like.Rank = -1;
            postService.DeleteLike(like);
        }
    }

    public void ApplyDetailLike()
    {
        onLikeChanged.Invoke(tglLike.Checked);
        onDislikeChanged.Invoke(tglDislike.Checked);
    }

    // Reaction

    public void ApplyReaction(bool check)
    {
        tglReaction.Uncheck();

        if (!check)
        {
            postService.DeleteReaction(new Reaction(-1, postId, StateManager.Instance.AppUser.Id));
            return;
        }

        cmbReaction.Combo.Click();
    }

    public void RegisterReaction()
    {
        long reactionPhraseId = cmbReaction.GetSelectedId();

        Reaction reaction = new Reaction(reactionPhraseId, postId, StateManager.Instance.AppUser.Id);
        postService.RegisterReaction(reaction);
        tglReaction.Check();
    }

    public void ApplyDetailReaction()
    {
        onReactionChanged.Invoke(tglReaction.Checked);
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

    private void RefreshContents()
    {
        for (int i = 0; i < contents.Length; i++)
        {
            Text txtScroll = contents[i].GetComponentInChildren<Text>();
            int lineCount = Mathf.CeilToInt((float)txtScroll.TextValue.Length / charsPerLine);
            float height = lineCount * lineHeight;

            contents[i].sizeDelta = new Vector2(contents[i].sizeDelta.x, height + contentPadding);
        }
    }
}