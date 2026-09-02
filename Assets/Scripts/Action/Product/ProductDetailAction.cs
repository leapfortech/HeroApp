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
    //[SerializeField]
    //Image imgThumbnail = null;
    //[SerializeField]
    //Text txtAlias = null;
    //[SerializeField]
    //Text txtDateTime = null;
    //[SerializeField]
    //Text txtTitle = null;
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
    //[SerializeField]
    //Text txtDeliveryType = null;
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
    //[SerializeField]
    //ValueList vllDeliveryType = null;

    [Space, Title("Actions")]
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

    [Space, Title("Page")]
    [SerializeField]
    Page pagDetail;

    [Space, Title("Events")]
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
        //imgThumbnail.Sprite = productFull.ThumbnailSprite;

        //txtAlias.TextValue = $"@{productFull.AppUserAlias}";
        //txtTitle.TextValue = $"<line-height=70%>{(String.IsNullOrWhiteSpace(productFull.Title) ? "Producto" : productFull.Title)}";
        //txtDateTime.TextValue = productFull.PublicationDateTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
        
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

        //txtDeliveryType.TextValue = productFull.DeliveryTypeId == -1 ? "-" : vllDeliveryType.FindRecordCellString(productFull.DeliveryTypeId, "Name");
        //txtAnnotation.TextValue = String.IsNullOrEmpty(productFull.Annotation) ? "-" : productFull.Annotation;
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
        SetToggle(tglLike, productFull.Like == 5);
        SetToggle(tglDislike, productFull.Like == 1);
        SetToggle(tglReaction, productFull.ReactionPhraseId != -1);

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

        content.sizeDelta = new Vector2(content.sizeDelta.x, contentInitialHeight + txtDescription.TextHeight + contentPadding);

        scrollRect.verticalNormalizedPosition = 1f;
    }
}