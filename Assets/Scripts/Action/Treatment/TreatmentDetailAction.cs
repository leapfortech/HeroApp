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

using Sirenix.OdinInspector;

public class TreatmentDetailAction : MonoBehaviour
{
    [Serializable]
    public class ImagesEvent : UnityEvent<List<Sprite>> { }

    [Space, Title("Details")]
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

    [Space, Title("List")]
    [SerializeField]
    ListScroller lstDisease = null;

    [Space, Space]
    [Title("Values")]
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    //[SerializeField]
    //ValueList vllCity = null;
    [SerializeField]
    ValueList vllDisease = null;

    [Space, Title("Actions")]
    [SerializeField]
    Toggle tglFavorite = null;
    [SerializeField]
    Toggle tglLike = null;
    [SerializeField]
    Toggle tglDislike = null;

    [Space, Title("Event")]
    [SerializeField]
    ImagesEvent onImagesDisplay = null;
    [SerializeField]
    UnityLongsEvent onDisplayed = null;

    [Space, Title("Page")]
    [SerializeField]
    Page pagDetail;

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
        for (int i = 0; i < treatmentFull.DiseaseFulls.Count; i++)
        {
            ListScrollerValue value = new ListScrollerValue(1, true);
            value.SetText(0, vllDisease.FindRecordCellString(treatmentFull.DiseaseFulls[i].DiseaseTypeId, "Name"));

            lstDisease.AddValue(value);
        }

        lstDisease.ApplyValues();

        // Images
        goEmptyImages.SetActive(treatmentFull.Images.Length == 0);
        goImages.SetActive(treatmentFull.Images.Length != 0);

        List<Sprite> images = new List<Sprite>();
        for (int i = 0; i < treatmentFull.Images.Length; i++)
            images.Add(treatmentFull.Images[i].CreateSprite($"TreatmentImage_{i}"));
        onImagesDisplay.Invoke(images);
        onDisplayed.Invoke(new long[2] {treatmentFull.PostId, treatmentFull.Id});

        // Actions
        SetToggle(tglFavorite, treatmentFull.Favorite != 0);
        SetToggle(tglLike, treatmentFull.Like == 5);
        SetToggle(tglDislike, treatmentFull.Like == 1);

        RefreshContents();

        PageManager.Instance.ChangePage(pagDetail);
    }

    public void ApplyFavorite(bool check)
    {
        Favorite favorite = new Favorite(postId, StateManager.Instance.AppUser.Id);
        if (check)
            postService.RegisterFavorite(favorite);
        else
            postService.DeleteFavorite(favorite);
    }

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