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

public class HappeningDetailAction : MonoBehaviour
{
    [Serializable]
    public class ImagesEvent : UnityEvent<List<Sprite>> { }

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

    [SerializeField]
    Text txtContactName = null;
    [SerializeField]
    Text txtPhone = null;
    [SerializeField]
    Text txtWhatsApp = null;
    [SerializeField]
    Text txtEmail = null;

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

    [Title("Values")]
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    //[SerializeField]
    //ValueList vllCity = null;
    [SerializeField]
    ValueList vllHappeningType = null;

    [Title("Actions")]
    [SerializeField]
    Button btnUpdate = null;
    [SerializeField]
    Toggle tglFavorite = null;
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

    HappeningService happeningService;
    PostService postService;

    long postId = -1;
    float contentInitialHeight = 0.0f;

    private void Awake()
    {
        happeningService = GetComponent<HappeningService>();
        postService = GetComponent<PostService>();
    }

    private void Start()
    {
        RectTransform content = txtDescription.transform.parent.GetComponent<RectTransform>();
        contentInitialHeight = content.sizeDelta.y - txtDescription.TextHeight
                               - txtLocation.TextHeight - txtPaymentDetails.TextHeight;
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
        //imgThumbnail.Sprite = happeningFull.ThumbnailSprite;

        //txtAlias.TextValue = $"@{happeningFull.AppUserAlias}";
        txtTitle.TextValue = $"<line-height=70%>{(String.IsNullOrWhiteSpace(happeningFull.Title) ? "Evento" : happeningFull.Title)}";
        //txtDateTime.TextValue = happeningFull.PublicationDateTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

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

        txtContactName.TextValue = String.IsNullOrEmpty(happeningFull.ContactFull.Name) ? "-" : happeningFull.ContactFull.Name;

        txtPhone.TextValue = "-";
        txtWhatsApp.TextValue = "-";
        txtEmail.TextValue = "-";

        for (int i = 0; i < happeningFull.LinkFulls.Count; i++)
        {
            String url = happeningFull.LinkFulls[i].Url;

            if (String.IsNullOrWhiteSpace(url))
                continue;

            String[] split = url.Split('|');

            String fullPhone = null;
            if (split.Length > 1)
            {
                long phoneCountryId = Convert.ToInt64(split[0]);
                String phone = split[1];
                String phonePrefix = vllCountry.FindRecordCellString(phoneCountryId, "PhonePrefix");
                fullPhone = phonePrefix + " " + phone;
            }

            if (happeningFull.LinkFulls[i].LinkTypeId == 2)
                txtPhone.TextValue = fullPhone;

            else if (happeningFull.LinkFulls[i].LinkTypeId == 3)
                txtWhatsApp.TextValue = fullPhone;

            else if (happeningFull.LinkFulls[i].LinkTypeId == 4)
                txtEmail.TextValue = happeningFull.LinkFulls[i].Url;
        }

        // Images
        goEmptyImages.SetActive(happeningFull.ImageSprites.Count == 0);
        goImages.SetActive(happeningFull.ImageSprites.Count != 0);

        onImagesDisplay.Invoke(happeningFull.ImageSprites);

        // Actions
        SetToggle(tglFavorite, happeningFull.Favorite != 0);

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

        float txtHeights = txtDescription.TextHeight + txtLocation.TextHeight + txtPaymentDetails.TextHeight;

        content.sizeDelta = new Vector2(content.sizeDelta.x, contentInitialHeight + txtHeights + contentPadding);

        scrollRect.verticalNormalizedPosition = 1f;
    }
}