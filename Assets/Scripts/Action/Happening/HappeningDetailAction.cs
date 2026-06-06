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

public class HappeningDetailAction : MonoBehaviour
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

    [Space, Title("Comments")]
    [SerializeField]
    GameObject goEmptyComments = null;
    //[SerializeField]
    //GameObject goComments = null;

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

    [Space, Title("Event")]
    [SerializeField]
    ImagesEvent onImagesDisplay = null;
    [SerializeField]
    UnityLongsEvent onDisplayed = null;

    [Space, Title("Page")]
    [SerializeField]
    Page pagDetail;

    HappeningService happeningService;

    private void Awake()
    {
        happeningService = GetComponent<HappeningService>();
    }

    public void Display(long postId)
    {
        ScreenDialog.Instance.Display();
        happeningService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);
    }

    public void ApplyFull(HappeningFull happeningFull)
    {
        // Post
        txtAlias.TextValue = $"@{happeningFull.AppUserAlias}";
        txtTitle.TextValue = String.IsNullOrWhiteSpace(happeningFull.Title) ? "-" : happeningFull.Title;
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
        goEmptyImages.SetActive(happeningFull.Images.Length == 0);
        goImages.SetActive(happeningFull.Images.Length != 0);

        List<Sprite> images = new List<Sprite>();
        for (int i = 0; i < happeningFull.Images.Length; i++)
            images.Add(happeningFull.Images[i].CreateSprite($"HappeningImage_{i}"));
        onImagesDisplay.Invoke(images);
        onDisplayed.Invoke(new long[2] {happeningFull.PostId, happeningFull.Id});

        // Comments
        goEmptyComments.SetActive(true);

        RefreshContents();

        PageManager.Instance.ChangePage(pagDetail);
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