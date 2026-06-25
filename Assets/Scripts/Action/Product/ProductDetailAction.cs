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

    [Space, Title("Actions")]
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

    [Space, Title("Event")]
    [SerializeField]
    ImagesEvent onImagesDisplay = null;
    [SerializeField]
    UnityLongsEvent onDisplayed = null;

    [Space, Title("Page")]
    [SerializeField]
    Page pagDetail;

    ProductService productService;
    PostService postService;

    long postId = -1;

    private void Awake()
    {
        productService = GetComponent<ProductService>();
        postService = GetComponent<PostService>();
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
        txtAlias.TextValue = $"@{productFull.AppUserAlias}";
        txtTitle.TextValue = String.IsNullOrWhiteSpace(productFull.Title) ? "Producto" : productFull.Title;
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

        txtDeliveryType.TextValue = productFull.DeliveryTypeId == -1 ? "-" : vllDeliveryType.FindRecordCellString(productFull.DeliveryTypeId, "Name");
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
        goEmptyImages.SetActive(productFull.Images.Length == 0);
        goImages.SetActive(productFull.Images.Length != 0);

        List<Sprite> images = new List<Sprite>();
        for (int i = 0; i < productFull.Images.Length; i++)
            images.Add(productFull.Images[i].CreateSprite($"ProductImage_{i}"));
        onImagesDisplay.Invoke(images);
        onDisplayed.Invoke(new long[2] {productFull.PostId, productFull.Id});

        // Actions
        SetToggle(tglFavorite, productFull.Favorite != 0);
        SetToggle(tglLike, productFull.Like == 5);
        SetToggle(tglDislike, productFull.Like == 1);
        SetToggle(tglReaction, productFull.ReactionPhraseId != -1);

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

    public void ApplyReaction(bool check)
    {
        tglReaction.Uncheck();

        if (!check)
        {
            postService.DeleteReaction(new Reaction(-1, postId, StateManager.Instance.AppUser.Id));
            return;
        }

        // Dialog
        cmbReaction.Combo.Click();
    }

    public void RegisterReaction()
    {
        long reactionPhraseId = cmbReaction.GetSelectedId();

        Reaction reaction = new Reaction(reactionPhraseId, postId, StateManager.Instance.AppUser.Id);
        postService.RegisterReaction(reaction);
        tglReaction.Check();
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