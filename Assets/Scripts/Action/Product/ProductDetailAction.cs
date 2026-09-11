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

public class ProductDetailAction : MonoBehaviour
{
    [Serializable]
    public class ImagesEvent : UnityEvent<List<Sprite>> { }

    [Space, Title("Details")]
    [SerializeField]
    Text txtTitle = null;
    [SerializeField]
    Text txtSummary = null;
    [SerializeField]
    Text txtDescription = null;

    [SerializeField]
    Text txtProductSubtype = null;
    [SerializeField]
    Text txtPrice = null;
    [SerializeField]
    Text txtDiscountPrice = null;

    [SerializeField]
    Text txtContactName = null;
    [SerializeField]
    Text txtCurrentLocation = null;
    [SerializeField]
    Text txtInterestLocation = null;

    [Space, Title("Images")]
    [SerializeField]
    GameObject goEmptyImages = null;
    [SerializeField]
    GameObject goImages = null;

    [Title("ScrollView")]
    [SerializeField]
    UnityEngine.UI.ScrollRect scrollRect;
    [SerializeField]
    float contentPadding = 160f;

    [Space, Title("Values")]
    [SerializeField]
    ValueList vllProductSubtype = null;
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    [SerializeField]
    ValueList vllCity = null;
    [SerializeField]
    ValueList vllCurrency = null;

    [Space, Title("Actions")]
    [SerializeField]
    Button btnUpdate = null;
    [SerializeField]
    Button btnPhone = null;
    [SerializeField]
    Button btnWhatsApp = null;
    [SerializeField]
    Button btnEmail = null;

    [SerializeField]
    Toggle tglFavorite = null;
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

    ProductService productService;
    PostService postService;

    long postId = -1;
    String phone = "", whatsapp = "", email = "";
    float contentInitialHeight = 0.0f;

    private void Awake()
    {
        productService = GetComponent<ProductService>();
        postService = GetComponent<PostService>();
    }

    private void Start()
    {
        btnPhone?.AddAction(OpenPhone);
        btnWhatsApp?.AddAction(OpenWhatsApp);
        btnEmail?.AddAction(OpenEmail);

        RectTransform content = txtDescription.transform.parent.GetComponent<RectTransform>();
        contentInitialHeight = content.sizeDelta.y - txtDescription.TextHeight;
    }

    public void Display(long postId)
    {
        ScreenDialog.Instance.Display();
        productService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);
    }

    public void ApplyFull(ProductFull productFull)
    {
        postId = productFull.PostId;

        // Post
        txtTitle.TextValue = $"<line-height=70%>{(String.IsNullOrWhiteSpace(productFull.Title) ? "Producto" : productFull.Title)}";
        
        if (txtSummary != null)
            txtSummary.TextValue = String.IsNullOrWhiteSpace(productFull.Summary) ? "-" : productFull.Summary;
        
        txtDescription.TextValue = String.IsNullOrWhiteSpace(productFull.Description) ? "-" : productFull.Description;

        // Product
        txtProductSubtype.TextValue = vllProductSubtype.FindRecordCellString(productFull.ProductSubtypeId, "Name");

        String country = productFull.PostCountryId == -1 ? "" : vllCountry.FindRecordCellString(productFull.PostCountryId, "Name");
        String state = productFull.PostStateId == -1 ? "" : vllState.FindRecordCellString(productFull.PostStateId, "Name");

        String currencySymbol = vllCurrency.FindRecordCellString(productFull.CurrencyId, "Symbol");
        txtPrice.TextValue = currencySymbol + " " + productFull.Price.ToString("N2");
        txtDiscountPrice.TextValue = productFull.DiscountPrice <= 0d ? "-" : currencySymbol + " " + productFull.DiscountPrice.ToString("N2");

        txtContactName.TextValue = String.IsNullOrEmpty(productFull.ContactFull.Name) ? "-" : productFull.ContactFull.Name;

        String currCountry = productFull.AppUserInfo.CurrentLocality.CountryId == -1 ? "" : vllCountry.FindRecordCellString(productFull.AppUserInfo.CurrentLocality.CountryId, "Name");
        String currState = productFull.AppUserInfo.CurrentLocality.StateId == -1 ? "" : vllState.FindRecordCellString(productFull.AppUserInfo.CurrentLocality.StateId, "Name");
        String currCity = productFull.AppUserInfo.CurrentLocality.CityId == -1 ? "" : vllCity.FindRecordCellString(productFull.AppUserInfo.CurrentLocality.CityId, "Name");

        String intCountry = productFull.AppUserInfo.InterestLocality.CountryId == -1 ? "" : vllCountry.FindRecordCellString(productFull.AppUserInfo.InterestLocality.CountryId, "Name");
        String intState = productFull.AppUserInfo.InterestLocality.StateId == -1 ? "" : vllState.FindRecordCellString(productFull.AppUserInfo.InterestLocality.StateId, "Name");
        String intCity = productFull.AppUserInfo.InterestLocality.CityId == -1 ? "" : vllCity.FindRecordCellString(productFull.AppUserInfo.InterestLocality.CityId, "Name");

        String currentLocation = currCountry + (String.IsNullOrWhiteSpace(currState) ? "" : ", " + currState) + (String.IsNullOrWhiteSpace(currCity) ? "" : ", " + currCity);
        String interestLocation = intCountry + (String.IsNullOrWhiteSpace(intState) ? "" : ", " + intState) + (String.IsNullOrWhiteSpace(intCity) ? "" : ", " + intCity);

        txtCurrentLocation.TextValue = String.IsNullOrWhiteSpace(currCountry + currState + currCity) ? "" : "Vive en: " + currentLocation;
        txtInterestLocation.TextValue = String.IsNullOrWhiteSpace(intCountry + intState + intCity) ? "" : "Originario de: " + interestLocation;

        phone = "";
        whatsapp = "";
        email = "";

        for (int i = 0; i < productFull.LinkFulls.Count; i++)
        {
            String url = productFull.LinkFulls[i].Url;

            if (String.IsNullOrWhiteSpace(url))
                continue;

            String[] split = url.Split('|');

            if (productFull.LinkFulls[i].LinkTypeId == 2 && split.Length > 1)
                phone = vllCountry.FindRecordCellString(Convert.ToInt64(split[0]), "PhonePrefix") + split[1];
            else if (productFull.LinkFulls[i].LinkTypeId == 3 && split.Length > 1)
                whatsapp = vllCountry.FindRecordCellString(Convert.ToInt64(split[0]), "PhonePrefix") + split[1];
            else if (productFull.LinkFulls[i].LinkTypeId == 4)
                email = url;
        }

        btnPhone.Interactable = phone.Length != 0;
        btnWhatsApp.Interactable = whatsapp.Length != 0;
        btnEmail.Interactable = email.Length != 0;

        // Images
        goEmptyImages.SetActive(productFull.ImageSprites.Count == 0);
        goImages.SetActive(productFull.ImageSprites.Count != 0);

        onImagesDisplay.Invoke(productFull.ImageSprites);

        // Actions
        SetToggle(tglFavorite, productFull.Favorite != 0);

        RefreshContents();

        btnUpdate.gameObject.SetActive(productFull.AppUserId == StateManager.Instance.AppUser.Id);

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

        content.sizeDelta = new Vector2(content.sizeDelta.x, contentInitialHeight + txtDescription.TextHeight + contentPadding);

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
}