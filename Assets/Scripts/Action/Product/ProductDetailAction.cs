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
    Text txtPlace = null;
    [SerializeField]
    Text txtPrice = null;
    [SerializeField]
    Text txtDiscountPrice = null;

    [SerializeField]
    Text txtContactName = null;
    [SerializeField]
    Text txtPhone = null;
    [SerializeField]
    Text txtWhatsApp = null;
    [SerializeField]
    Text txtEmail = null;

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
    //[SerializeField]
    //ValueList vllCity = null;
    [SerializeField]
    ValueList vllCurrency = null;

    [Space, Title("Actions")]
    [SerializeField]
    Button btnUpdate = null;
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
    float contentInitialHeight = 0.0f;

    private void Awake()
    {
        productService = GetComponent<ProductService>();
        postService = GetComponent<PostService>();
    }

    private void Start()
    {
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
        txtPlace.TextValue = country + (!String.IsNullOrWhiteSpace(country) && !String.IsNullOrWhiteSpace(state) ? ", " : "") + state;

        String currencySymbol = vllCurrency.FindRecordCellString(productFull.CurrencyId, "Symbol");
        txtPrice.TextValue = currencySymbol + " " + productFull.Price.ToString("N2");
        txtDiscountPrice.TextValue = productFull.DiscountPrice <= 0d ? "-" : currencySymbol + " " + productFull.DiscountPrice.ToString("N2");

        txtContactName.TextValue = String.IsNullOrEmpty(productFull.ContactFull.Name) ? "-" : productFull.ContactFull.Name;

        txtPhone.TextValue = "-";
        txtWhatsApp.TextValue = "-";
        txtEmail.TextValue = "-";

        for (int i = 0; i < productFull.LinkFulls.Count; i++)
        {
            String url = productFull.LinkFulls[i].Url;

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

            if (productFull.LinkFulls[i].LinkTypeId == 2)
                txtPhone.TextValue = fullPhone;

            else if (productFull.LinkFulls[i].LinkTypeId == 3)
                txtWhatsApp.TextValue = fullPhone;

            else if (productFull.LinkFulls[i].LinkTypeId == 4)
                txtEmail.TextValue = productFull.LinkFulls[i].Url;
        }

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
}