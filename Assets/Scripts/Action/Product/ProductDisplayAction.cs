using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Core.Tools;
using Leap.Data.Collections;

using Sirenix.OdinInspector;


public class ProductDisplayAction : MonoBehaviour
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
    Text txtProductSubtype = null;
    [SerializeField]
    Text txtPlace = null;
    [SerializeField]
    Text txtPrice = null;
    [SerializeField]
    Text txtDiscountPrice = null;
    [SerializeField]
    Text txtDeliveryType = null;
    //[SerializeField]
    //Text txtAnnotation = null;

    [SerializeField]
    Text txtContactName = null;
    [SerializeField]
    Text txtPhone = null;
    [SerializeField]
    Text txtWhatsApp = null;
    [SerializeField]
    Text txtEmail = null;

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
    ValueList vllProductSubtype = null;
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    //[SerializeField]
    //ValueList vllCity = null;
    [SerializeField]
    ValueList vllCurrency = null;
    [SerializeField]
    ValueList vllDeliveryType = null;

    [Space, Title("Event")]
    [SerializeField]
    ImagesEvent onImagesDisplay = null;
    [SerializeField]
    UnityLongsEvent onDisplayed = null;

    [Space, Title("Page")]
    [SerializeField]
    Page pagDetail;

    ProductService productService;

    long postId = -1, productId = -1;

    private void Awake()
    {
        productService = GetComponent<ProductService>();
    }

    public void Display(long postId)
    {
        this.postId = postId;

        ProductFull productFull = StateManager.Instance.GetProductFullByPostId(postId);
        if (productFull != null)
        {
            productId = productFull.Id;
            Display(productFull);
            return;
        }

        ScreenDialog.Instance.Display();
        productService.GetFullByPostId(postId);
    }

    public void ApplyFull(ProductFull productFull)
    {
        productId = productFull.Id;
        StateManager.Instance.AddProductFull(productFull);
        StateManager.Instance.AddProductImages(productFull.Id, productFull.Images);
        Display(productFull);
    }

    private void Display(ProductFull productFull)
    {       
        if (productFull == null)
            return;

        // Post
        txtAlias.TextValue = $"Publicado por: <b>@{productFull.AppUserAlias}</b>";
        txtTitle.TextValue = String.IsNullOrWhiteSpace(productFull.Title) ? "-" : productFull.Title;
        txtDateTime.TextValue = productFull.PublicationDateTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
        
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

        txtDeliveryType.TextValue = productFull.DeliveryTypeId == -1 ? "-" : vllDeliveryType.FindRecordCellString(productFull.DeliveryTypeId, name);
        //txtAnnotation.TextValue = String.IsNullOrEmpty(productFull.Annotation) ? "-" : productFull.Annotation;
        txtContactName.TextValue = String.IsNullOrEmpty(productFull.ContactFull.Name) ? "-" : productFull.ContactFull.Name;

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
        List<Sprite> images = StateManager.Instance.GetProductImagesById(productId);
        onImagesDisplay.Invoke(images);
        onDisplayed.Invoke(new long[2] {productFull.PostId, productFull.Id});

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