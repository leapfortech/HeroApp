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

public class TreatmentDetailAction : MonoBehaviour
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
    Text txtPlace = null;
    [SerializeField]
    Text txtSummary = null;
    [SerializeField]
    Text txtDescription = null;
    [SerializeField]
    Text txtIngredients = null;
    [SerializeField]
    Text txtPreparation = null;
    [SerializeField]
    Text txtUsage = null;
    //[SerializeField]
    //Text txtAnnotation = null;

    [Title("Images")]
    [SerializeField]
    GameObject goEmptyImages = null;
    [SerializeField]
    GameObject goImages = null;

    [Title("Contents")]
    [SerializeField]
    float contentPadding = 40f;

    [Title("List")]
    [SerializeField]
    ListScroller lstDisease = null;

    [Title("Values")]
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    //[SerializeField]
    //ValueList vllCity = null;
    [SerializeField]
    ValueList vllDisease = null;

    [Title("Actions")]
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

    [Title("Page")]
    [SerializeField]
    Page pagDetail;

    [Title("Events")]
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

    TreatmentService treatmentService;
    PostService postService;

    long postId = -1;

    private void Awake()
    {
        treatmentService = GetComponent<TreatmentService>();
        postService = GetComponent<PostService>();
    }

    public void Display(long postId)
    {
        ScreenDialog.Instance.Display();
        treatmentService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);
    }

    public void ApplyFull(TreatmentFull treatmentFull)
    {
        postId = treatmentFull.PostId;

        // Post
        imgThumbnail.Sprite = treatmentFull.ThumbnailSprite;

        txtAlias.TextValue = $"@{treatmentFull.AppUserAlias}";
        txtTitle.TextValue = String.IsNullOrWhiteSpace(treatmentFull.Title) ? "Remedio" : treatmentFull.Title;
        txtDateTime.TextValue = treatmentFull.PublicationDateTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        if (txtSummary != null)
            txtSummary.TextValue = String.IsNullOrWhiteSpace(treatmentFull.Summary) ? "-" : treatmentFull.Summary;
        
        txtDescription.TextValue = String.IsNullOrWhiteSpace(treatmentFull.Description) ? "-" : treatmentFull.Description;

        String country = treatmentFull.PostCountryId == -1 ? "" : vllCountry.FindRecordCellString(treatmentFull.PostCountryId, "Name");
        String state = treatmentFull.PostStateId == -1 ? "" : vllState.FindRecordCellString(treatmentFull.PostStateId, "Name");
        txtPlace.TextValue = country + (!String.IsNullOrWhiteSpace(country) && !String.IsNullOrWhiteSpace(state) ? ", " : "") + state;

        // Treatment
        txtIngredients.TextValue = String.IsNullOrWhiteSpace(treatmentFull.Ingredients) ? "-" : treatmentFull.Ingredients;
        txtPreparation.TextValue = String.IsNullOrWhiteSpace(treatmentFull.Preparation) ? "-" : treatmentFull.Preparation;
        txtUsage.TextValue = String.IsNullOrWhiteSpace(treatmentFull.Usage) ? "-" : treatmentFull.Usage;
        //txtAnnotation.TextValue = String.IsNullOrWhiteSpace(treatmentFull.Annotation) ? "-" : treatmentFull.Annotation;

        // Disease
        lstDisease.Clear();
        for (int i = 0; i < treatmentFull.DiseaseFulls.Count; i++)
        {
            ListScrollerValue value = new ListScrollerValue(1, true);
            value.SetText(0, vllDisease.FindRecordCellString(treatmentFull.DiseaseFulls[i].DiseaseTypeId, "Name"));

            lstDisease.AddValue(value);
        }

        lstDisease.ApplyValues();

        // Images
        goEmptyImages.SetActive(treatmentFull.ImageSprites.Count == 0);
        goImages.SetActive(treatmentFull.ImageSprites.Count != 0);

        onImagesDisplay.Invoke(treatmentFull.ImageSprites);

        // Actions
        SetToggle(tglFavorite, treatmentFull.Favorite != 0);
        SetToggle(tglLike, treatmentFull.Like == 5);
        SetToggle(tglDislike, treatmentFull.Like == 1);
        SetToggle(tglReaction, treatmentFull.ReactionPhraseId != -1);

        RefreshContents();

        btnUpdate.gameObject.SetActive(treatmentFull.AppUserId == StateManager.Instance.AppUser.Id);

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
        RectTransform content = txtDescription.transform.parent.GetComponent<RectTransform>();
        content.sizeDelta = new Vector2(content.sizeDelta.x, txtDescription.TextHeight + contentPadding);

        content = txtIngredients.transform.parent.GetComponent<RectTransform>();
        content.sizeDelta = new Vector2(content.sizeDelta.x, txtIngredients.TextHeight + contentPadding);

        content = txtPreparation.transform.parent.GetComponent<RectTransform>();
        content.sizeDelta = new Vector2(content.sizeDelta.x, txtPreparation.TextHeight + contentPadding);

        content = txtUsage.transform.parent.GetComponent<RectTransform>();
        content.sizeDelta = new Vector2(content.sizeDelta.x, txtUsage.TextHeight + contentPadding);
    }
}