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

public class HappeningDetailAction : MonoBehaviour
{
    [Serializable]
    public class ImagesEvent : UnityEvent<List<Sprite>> { }
    [Serializable]
    public class HappeningFullEvent : UnityEvent<HappeningFull> { }

    [Space, Title("Details")]
    [SerializeField]
    Text txtFavoriteCount = null;
    [SerializeField]
    Text txtTitle = null;
    [SerializeField]
    Text txtDescription = null;
    [SerializeField]
    Text txtHappeningType = null;
    [SerializeField]
    Text txtPlace = null;
    [SerializeField]
    Text txtIsPublic = null;
    [SerializeField]
    Text txtHasSignupPayment = null;
    [SerializeField]
    Text txtPaymentDetails = null;
    [SerializeField]
    Text txtStartDate = null;
    [SerializeField]
    Text txtStartTime = null;
    [SerializeField]
    Text txtEndDate = null;
    [SerializeField]
    Text txtEndTime = null;
    [SerializeField]
    Text txtLocation = null;

    [SerializeField]
    RectTransform rtfPaymentDetails = null;
    [SerializeField]
    GameObject gobPaymentDetails = null;

    [SerializeField]
    RectTransform rtfContact = null;
    [SerializeField]
    GameObject gobContact = null;

    [SerializeField]
    Text txtContactName = null;

    [Title("ScrollView")]
    [SerializeField]
    UnityEngine.UI.ScrollRect scrollRect;
    [SerializeField]
    float contentPadding = 160f;

    [Title("Admin")]
    [SerializeField]
    GameObject gobAdmin = null;

    [Title("Values")]
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    [SerializeField]
    ValueList vllHappeningType = null;

    [Title("Actions")]
    [SerializeField]
    Button btnDelete = null;
    [SerializeField]
    Button btnPhone = null;
    [SerializeField]
    Button btnWhatsApp = null;
    [SerializeField]
    Button btnEmail = null;
    [SerializeField]
    Toggle tglFavorite = null;
    [SerializeField]
    Toggle tglSelected = null;
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
    HappeningFullEvent onApplyUpdate = null;

    [SerializeField]
    UnityEvent onDeleted = null;

    HappeningService happeningService;
    PostService postService;

    long postId = -1;
    float contentInitialHeight = 0.0f, paymentDetailsInitialHeight = 0.0f, paymentDetailsTextInitialHeight = 0.0f,
          contactInitialHeight = 0.0f, contactTextInitialHeight = 0.0f;
    String phone = "", whatsapp = "", email = "";
    bool isRefresh = false;
    HappeningFull happeningFull = null;

    private void Awake()
    {
        happeningService = GetComponent<HappeningService>();
        postService = GetComponent<PostService>();
    }

    private void Start()
    {
        RectTransform content = txtTitle.transform.parent.GetComponent<RectTransform>();
        RectTransform rtfAdmin = gobAdmin.GetComponent<RectTransform>();

        paymentDetailsInitialHeight = rtfPaymentDetails.sizeDelta.y;
        paymentDetailsTextInitialHeight = txtPaymentDetails.TextHeight;

        contactInitialHeight = rtfContact.sizeDelta.y;
        contactTextInitialHeight = txtContactName.TextHeight;

        contentInitialHeight = content.sizeDelta.y
                               - txtTitle.TextHeight
                               - txtDescription.TextHeight
                               - txtPlace.TextHeight
                               - txtLocation.TextHeight
                               - paymentDetailsInitialHeight
                               - contactInitialHeight
                               - rtfAdmin.sizeDelta.y;

        btnPhone?.AddAction(OpenPhone);
        btnWhatsApp?.AddAction(OpenWhatsApp);
        btnEmail?.AddAction(OpenEmail);
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
        happeningService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);
    }

    public bool Refresh()
    {
        isRefresh = true;
        ScreenDialog.Instance.Display();
        happeningService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);

        return false;
    }

    public void ApplyFull(HappeningFull happeningFull)
    {
        this.happeningFull = happeningFull;

        postId = happeningFull.PostId;

        // Post
        txtTitle.TextValue = $"<line-height=70%>{(String.IsNullOrWhiteSpace(happeningFull.Title) ? "Evento" : happeningFull.Title)}";
        txtTitle.GetComponent<RectTransform>().sizeDelta = new Vector2(txtTitle.GetComponent<RectTransform>().sizeDelta.x, txtTitle.TextHeight);
        float deltaY = txtTitle.TextHeight;

        txtDescription.TextValue = String.IsNullOrWhiteSpace(happeningFull.Description) ? "-" : happeningFull.Description;
        txtDescription.GetComponent<RectTransform>().sizeDelta = new Vector2(txtDescription.GetComponent<RectTransform>().sizeDelta.x, txtDescription.TextHeight);
        deltaY += txtDescription.TextHeight;

        txtFavoriteCount.TextValue = FeedHelper.Instance.FormatCount(happeningFull.FavoriteCount);
        // Happening
        txtHappeningType.TextValue = happeningFull.HappeningTypeId == -1 ? "-" : vllHappeningType.FindRecordCellString(happeningFull.HappeningTypeId, "Name");

        String country = happeningFull.PostCountryId == -1 ? "" : vllCountry.FindRecordCellString(happeningFull.PostCountryId, "Name");
        String state = happeningFull.PostStateId == -1 ? "" : vllState.FindRecordCellString(happeningFull.PostStateId, "Name");
        txtPlace.TextValue = country + (!String.IsNullOrWhiteSpace(country) && !String.IsNullOrWhiteSpace(state) ? ", " : "") + state;
        txtPlace.GetComponent<RectTransform>().sizeDelta = new Vector2(txtPlace.GetComponent<RectTransform>().sizeDelta.x, txtPlace.TextHeight);
        deltaY += txtPlace.TextHeight;

        txtIsPublic.TextValue = happeningFull.IsPublic == -1 ? "-" : happeningFull.IsPublic == 0 ? "Evento privado" : "Evento público";

        if (happeningFull.HasSignup == 0 && happeningFull.HasPayment == 0)
            txtHasSignupPayment.TextValue = "<color=#34A60E>Sin registro ni pago</color>";
        else if (happeningFull.HasSignup == 0 && happeningFull.HasPayment == 1)
            txtHasSignupPayment.TextValue = "<color=#D88510>Sin registro, con pago</color>";
        else if (happeningFull.HasSignup == 1 && happeningFull.HasPayment == 0)
            txtHasSignupPayment.TextValue = "<color=#D88510>Con registro, sin pago</color>";
        else
            txtHasSignupPayment.TextValue = "<color=#D88510>Con registro y pago</color>";

        // Payment
        gobPaymentDetails.SetActive(happeningFull.HasPayment == 1);
        txtPaymentDetails.TextValue = happeningFull.HasPayment == 1 ? happeningFull.PaymentDetails ?? "" : "";

        if (happeningFull.HasPayment == 1 && !String.IsNullOrWhiteSpace(txtPaymentDetails.TextValue))
        {
            float height = paymentDetailsInitialHeight - paymentDetailsTextInitialHeight + txtPaymentDetails.TextHeight;

            rtfPaymentDetails.sizeDelta = new Vector2(rtfPaymentDetails.sizeDelta.x, height);
            deltaY += height;
        }
        else
            rtfPaymentDetails.sizeDelta = new Vector2(rtfPaymentDetails.sizeDelta.x, 0);

        // Dates
        txtStartDate.TextValue = happeningFull.StartDateTime == null ? "-" : happeningFull.StartDateTime.Value.ToString("dd/MM/yyyy");
        txtStartTime.TextValue = happeningFull.StartDateTime == null ? "-" : happeningFull.StartDateTime.Value.ToString("HH:mm");
        txtEndDate.TextValue = happeningFull.EndDateTime == null ? "-" : happeningFull.EndDateTime.Value.ToString("dd/MM/yyyy");
        txtEndTime.TextValue = happeningFull.EndDateTime == null ? "-" : happeningFull.EndDateTime.Value.ToString("HH:mm");
        
        txtLocation.TextValue = String.IsNullOrWhiteSpace(happeningFull.Location) ? "-" : happeningFull.Location;
        txtLocation.GetComponent<RectTransform>().sizeDelta = new Vector2(txtLocation.GetComponent<RectTransform>().sizeDelta.x, txtLocation.TextHeight);
        deltaY += txtLocation.TextHeight;

        // Contact
        bool hasContact = happeningFull.ContactFull != null && !String.IsNullOrWhiteSpace(happeningFull.ContactFull.Name);

        gobContact.SetActive(hasContact);
        txtContactName.TextValue = hasContact ? happeningFull.ContactFull.Name : "";

        if (hasContact)
        {
            float height = contactInitialHeight - contactTextInitialHeight + txtContactName.TextHeight;

            rtfContact.sizeDelta = new Vector2(rtfContact.sizeDelta.x, height);
            deltaY += height;
        }
        else
            rtfContact.sizeDelta = new Vector2(rtfContact.sizeDelta.x, 0);

        phone = "";
        whatsapp = "";
        email = "";

        for (int i = 0; i < happeningFull.LinkFulls.Count; i++)
        {
            String url = happeningFull.LinkFulls[i].Url;

            if (String.IsNullOrWhiteSpace(url))
                continue;

            String[] split = url.Split('|');

            if (happeningFull.LinkFulls[i].LinkTypeId == 2 && split.Length > 1)
            {
                phone = vllCountry.FindRecordCellString(Convert.ToInt64(split[0]), "PhonePrefix") + split[1];
                btnPhone.Title = phone;
            }
            else if (happeningFull.LinkFulls[i].LinkTypeId == 3 && split.Length > 1)
            {
                whatsapp = vllCountry.FindRecordCellString(Convert.ToInt64(split[0]), "PhonePrefix") + split[1];
                btnWhatsApp.Title = whatsapp;
            }
            else if (happeningFull.LinkFulls[i].LinkTypeId == 4)
            {
                email = url;
                btnEmail.Title = email;
            }
        }

        btnPhone.Interactable = phone.Length != 0;
        btnWhatsApp.Interactable = whatsapp.Length != 0;
        btnEmail.Interactable = email.Length != 0;

        onImagesDisplay.Invoke(happeningFull.ImageSprites);

        // Actions
        if (happeningFull.Favorite != 0)
            tglFavorite.Check();
        else
            tglFavorite.Uncheck();

        if (happeningFull.Selected != 0)
            tglSelected.Check();
        else
            tglSelected.Uncheck();

        // Admin
        gobAdmin.SetActive(happeningFull.AppUserId == StateManager.Instance.AppUser.Id);

        if (happeningFull.AppUserId == StateManager.Instance.AppUser.Id)
            deltaY += gobAdmin.GetComponent<RectTransform>().sizeDelta.y;

        RefreshContents(deltaY);

        PageManager.Instance.ChangePage(pagDetail);
    }

    public void ApplyUpdate()
    {
        onApplyUpdate.Invoke(happeningFull);
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
        configs[StateManager.Instance.FeedDetailType].onFavoriteChanged.Invoke(tglFavorite.Checked);
    }

    // Selected

    public void ApplySelected(bool check)
    {
        Selected selected = new Selected(postId, StateManager.Instance.AppUser.Id);
        if (check)
            postService.RegisterSelected(selected);
        else
            postService.DeleteSelected(selected);
    }

    public void ApplyDetailSelected()
    {
        configs[StateManager.Instance.FeedDetailType].onSelectedChanged.Invoke(tglSelected.Checked);
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

    private void RefreshContents(float deltaY)
    {
        RectTransform content = txtTitle.transform.parent.GetComponent<RectTransform>();

        content.sizeDelta = new Vector2(content.sizeDelta.x, contentInitialHeight + deltaY + contentPadding);

        if (!isRefresh)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    private void OpenPhone()
    {
        Application.OpenURL("tel://" + phone);
    }

    private void OpenWhatsApp()
    {
        Application.OpenURL("https://wa.me/" + whatsapp.Replace(" ", ""));
    }

    private void OpenEmail()
    {
        Application.OpenURL("mailto:" + email);
    }

    // Delete
    private void Delete()
    {
        ChoiceDialog.Instance.Error("Eliminar publicación", "¿Estas seguro de eliminar la publicación?", DoDelete, null, "De acuerdo", "Regresar");
    }

    private void DoDelete()
    {
        ScreenDialog.Instance.Display();

        happeningService.Delete(new PostModerationRequest(happeningFull.PostId, happeningFull.Id));
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