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
    [Serializable]
    public class ProductFullEvent : UnityEvent<ProductFull> { }

    [Space, Title("Details")]
    [SerializeField]
    Text txtTitle = null;
    [SerializeField]
    Text txtDescription = null;

    [SerializeField]
    Text txtProductSubtype = null;
    [SerializeField]
    Text txtPrice = null;
    [SerializeField]
    GameObject gobDiscountPrice = null;
    [SerializeField]
    Text txtDiscountPrice = null;
    [SerializeField]
    Text txtDate = null;
    [SerializeField]
    Text txtLocation = null;

    [Space, Title("AppUser")]
    [SerializeField]
    Text txtAlias = null;
    [SerializeField]
    Image imgThumbnail = null;
    [SerializeField]
    Text txtCurrentLocation = null;
    [SerializeField]
    Text txtInterestLocation = null;

    [SerializeField]
    RectTransform rtfContact = null;
    [SerializeField]
    GameObject gobContactDetails = null;

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
    Button btnDelete = null;
    [SerializeField]
    Button btnContact = null;
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

    [Title("Configs")]
    [SerializeField]
    FeedDetailConfig[] configs = null;


    [Space, Title("Page")]
    [SerializeField]
    Page pagDetail;

    [Space, Title("Events")]
    [SerializeField]
    ImagesEvent onImagesDisplay = null;

    [SerializeField]
    ProductFullEvent onApplyUpdate = null;

    [SerializeField]
    UnityEvent onDeleted = null;

    ProductService productService;
    PostService postService;

    long postId = -1;
    String phone = "", whatsapp = "", email = "";
    float contentInitialHeight = 0.0f, contactInitialHeight = 0.0f;
    bool isRefresh = false;
    ProductFull productFull = null;

    private void Awake()
    {
        productService = GetComponent<ProductService>();
        postService = GetComponent<PostService>();
    }

    private void Start()
    {
        RectTransform content = txtTitle.transform.parent.GetComponent<RectTransform>();
        RectTransform rtfAdmin = gobAdmin.GetComponent<RectTransform>();
        
        contactInitialHeight = rtfContact.sizeDelta.y;

        contentInitialHeight = content.sizeDelta.y
                               - txtDescription.TextHeight
                               - contactInitialHeight
                               - rtfAdmin.sizeDelta.y;

        btnContact?.AddAction(OpenContact);
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
        productService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);
    }

    public bool Refresh()
    {
        isRefresh = true;
        ScreenDialog.Instance.Display();
        productService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);

        return false;
    }

    public void ApplyFull(ProductFull productFull)
    {
        this.productFull = productFull;

        postId = productFull.PostId;

        // Post
        txtTitle.TextValue = $"<line-height=70%>{(String.IsNullOrWhiteSpace(productFull.Title) ? "Producto" : productFull.Title)}";
        float deltaY = txtTitle.TextHeight;

        txtDescription.TextValue = String.IsNullOrWhiteSpace(productFull.Description) ? "-" : productFull.Description;
        deltaY += txtDescription.TextHeight;

        // Product
        txtProductSubtype.TextValue = vllProductSubtype.FindRecordCellString(productFull.ProductSubtypeId, "Name");

        String country = productFull.PostCountryId == -1 ? "" : vllCountry.FindRecordCellString(productFull.PostCountryId, "Name");
        String state = productFull.PostStateId == -1 ? "" : vllState.FindRecordCellString(productFull.PostStateId, "Name");

        String currencySymbol = vllCurrency.FindRecordCellString(productFull.CurrencyId, "Symbol");

        gobDiscountPrice.SetActive(productFull.DiscountPrice > 0d);
        if (productFull.DiscountPrice > 0d)
        {
            txtPrice.TextValue = currencySymbol + " " + productFull.DiscountPrice.ToString("N2");
            txtDiscountPrice.TextValue = currencySymbol + " " + productFull.Price.ToString("N2");
        }
        else
            txtPrice.TextValue = currencySymbol + " " + productFull.Price.ToString("N2");

        txtDate.TextValue = productFull.PublicationDateTime.ToLocalTime().ToString("dd/MM/yyyy");
        txtLocation.TextValue = vllCountry.FindRecordCellString(productFull.SaleCountryId, "Name") + (productFull.SaleStateId != -1 ? ", " + vllState.FindRecordCellString(productFull.SaleStateId, "Name") : "");

        // AppUserInfo
        txtAlias.TextValue = productFull.AppUserInfo.Alias;
        imgThumbnail.Sprite = productFull.AppUserInfo.ThumbnailSprite;

        String currCountry = productFull.AppUserInfo.CurrentLocality.CountryId == -1 ? "" : vllCountry.FindRecordCellString(productFull.AppUserInfo.CurrentLocality.CountryId, "Name");
        String currState = productFull.AppUserInfo.CurrentLocality.StateId == -1 ? "" : vllState.FindRecordCellString(productFull.AppUserInfo.CurrentLocality.StateId, "Name");
        String currCity = productFull.AppUserInfo.CurrentLocality.CityId == -1 ? "" : vllCity.FindRecordCellString(productFull.AppUserInfo.CurrentLocality.CityId, "Name");

        String intCountry = productFull.AppUserInfo.InterestLocality.CountryId == -1 ? "" : vllCountry.FindRecordCellString(productFull.AppUserInfo.InterestLocality.CountryId, "Name");
        String intState = productFull.AppUserInfo.InterestLocality.StateId == -1 ? "" : vllState.FindRecordCellString(productFull.AppUserInfo.InterestLocality.StateId, "Name");
        String intCity = productFull.AppUserInfo.InterestLocality.CityId == -1 ? "" : vllCity.FindRecordCellString(productFull.AppUserInfo.InterestLocality.CityId, "Name");

        String currentLocation = !String.IsNullOrWhiteSpace(currCity) ? currCity : !String.IsNullOrWhiteSpace(currState) ? currState : currCountry;
        String interestLocation = !String.IsNullOrWhiteSpace(intCity) ? intCity : !String.IsNullOrWhiteSpace(intState) ? intState : intCountry;

        txtCurrentLocation.TextValue = String.IsNullOrWhiteSpace(currentLocation) ? "" : "Vive en: " + currentLocation;
        txtInterestLocation.TextValue = String.IsNullOrWhiteSpace(interestLocation) ? "" : "Originario de: " + interestLocation;

        // Contact
        bool hasContact = productFull.ContactFull != null && !String.IsNullOrWhiteSpace(productFull.ContactFull.Name);

        gobContactDetails.SetActive(hasContact);
        txtContactName.TextValue = hasContact ? productFull.ContactFull.Name : "";

        phone = "";
        whatsapp = "";
        email = "";

        btnPhone.gameObject.SetActive(false);
        btnWhatsApp.gameObject.SetActive(false);
        btnEmail.gameObject.SetActive(false);

        float offset = 0f;

        Vector2 initialPosition = btnPhone.GetComponent<RectTransform>().anchoredPosition;

        if (productFull.LinkFulls != null)
        {
            for (int i = 0; i < productFull.LinkFulls.Count; i++)
            {
                String url = productFull.LinkFulls[i].Url;

                if (String.IsNullOrWhiteSpace(url))
                    continue;

                String[] split = url.Split('|');
                GameObject buttonObject = null;

                if (productFull.LinkFulls[i].LinkTypeId == 2 && split.Length > 1)
                {
                    phone = vllCountry.FindRecordCellString(Convert.ToInt64(split[0]), "PhonePrefix") + split[1];

                    btnPhone.Title = phone;
                    buttonObject = btnPhone.gameObject;
                }
                else if (productFull.LinkFulls[i].LinkTypeId == 3 && split.Length > 1)
                {
                    whatsapp = vllCountry.FindRecordCellString(Convert.ToInt64(split[0]), "PhonePrefix") + split[1];

                    btnWhatsApp.Title = whatsapp;
                    buttonObject = btnWhatsApp.gameObject;
                }
                else if (productFull.LinkFulls[i].LinkTypeId == 4)
                {
                    email = url;
                    btnEmail.Title = email;
                    buttonObject = btnEmail.gameObject;
                }

                if (buttonObject != null && !buttonObject.activeSelf)
                {
                    RectTransform rect = buttonObject.GetComponent<RectTransform>();
                    rect.anchoredPosition = initialPosition - new Vector2(0f, offset);

                    buttonObject.SetActive(true);
                    offset += btnPhone.GetComponent<RectTransform>().sizeDelta.y;
                }
            }
        }

        if (hasContact)
        {
            offset += contactInitialHeight;
            rtfContact.sizeDelta = new Vector2(rtfContact.sizeDelta.x, offset);
            deltaY += offset;
        }
        else
            rtfContact.sizeDelta = new Vector2(rtfContact.sizeDelta.x, 0f);

        btnPhone.gameObject.SetActive(phone.Length != 0);
        btnWhatsApp.gameObject.SetActive(whatsapp.Length != 0);
        btnEmail.gameObject.SetActive(email.Length != 0);

        // Images
        onImagesDisplay.Invoke(productFull.ImageSprites);

        // Actions
        if (productFull.Favorite != 0)
            tglFavorite.Check();
        else
            tglFavorite.Uncheck();

        // Admin
        gobAdmin.SetActive(productFull.AppUserId == StateManager.Instance.AppUser.Id);

        if (productFull.AppUserId == StateManager.Instance.AppUser.Id)
            deltaY += gobAdmin.GetComponent<RectTransform>().sizeDelta.y;

        RefreshContents(deltaY);

        PageManager.Instance.ChangePage(pagDetail);
    }

    public void ApplyUpdate()
    {
        onApplyUpdate.Invoke(productFull);
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

    private void OpenContact()
    {
        if (!String.IsNullOrWhiteSpace(phone))
            OpenPhone();
        else if (!String.IsNullOrWhiteSpace(whatsapp))
            OpenWhatsApp();
        else if (!String.IsNullOrWhiteSpace(email))
            OpenEmail();
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

        productService.Delete(new PostModerationRequest(productFull.PostId, productFull.Id));
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