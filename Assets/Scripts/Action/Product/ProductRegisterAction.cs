using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;

using Sirenix.OdinInspector;

public class ProductRegisterAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmPost = null;
    [SerializeField]
    DataMapper dtmProduct = null;
    [SerializeField]
    DataMapper dtmContact = null;
    [SerializeField]
    DataMapper dtmPhone = null;
    [SerializeField]
    DataMapper dtmWhatsApp = null;
    [SerializeField]
    DataMapper dtmEmail = null;

    [Space]
    [Title("Images")]
    [SerializeField]
    int maxCount = 4;
    [SerializeField]
    String spriteName = "Product";
    [SerializeField]
    ListScroller lstImage = null;
    [SerializeField]
    Text txtEmpty;

    [Title("Action")]
    [SerializeField]
    Button btnAddImage = null;

    [SerializeField]
    Button btnRegister = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    ProductService productService = null;
    List<Texture2D> images = new List<Texture2D>();

    private void Awake()
    {
        productService = GetComponent<ProductService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(Register);
    }

    public void Clear()
    {
        dtmPost.ClearElements();
        dtmProduct.ClearElements();
        dtmContact.ClearElements();
        dtmPhone.ClearElements();
        dtmWhatsApp.ClearElements();
        dtmEmail.ClearElements();
        images.Clear();
        lstImage.Clear();
    }

    public void RefreshImages()
    {
        lstImage.Clear();

        for (int i = 0; i < images.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(1, true);
            scrollerValue.SetSprite(0, images[i].CreateSprite($"{spriteName}_{i}"));
            lstImage.ApplyAddValue(scrollerValue);
        }

        if (images.Count > 0)
            txtEmpty.gameObject.SetActive(false);
        else
            txtEmpty.gameObject.SetActive(true);

        if (images.Count < maxCount)
            btnAddImage.gameObject.SetActive(true);
        else
            btnAddImage.gameObject.SetActive(false);
    }

    public void AddImage(Texture2D image)
    {
        images.Add(image);
        RefreshImages();
    }

    public void RemoveImage(int idx)
    {
        images.RemoveAt(idx);
        RefreshImages();
    }

    private void Register()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        Post post = dtmPost.BuildClass<Post>();

        post.AppUserId = StateManager.Instance.AppUser.Id;
        post.CountryId = StateManager.Instance.Identity.OriginCountryId;
        post.StateId = StateManager.Instance.Identity.OriginStateId;

        // RM WIP Fill All Params
        Product product = dtmProduct.BuildClass<Product>();

        Contact contact = dtmContact.BuildClass<Contact>();
        Link phone = dtmPhone.BuildClass<Link>();
        phone.LinkTypeId = (long)LinkType.Phone;
        Link whatsApp = dtmWhatsApp.BuildClass<Link>();
        whatsApp.LinkTypeId = (long)LinkType.WhatsApp;
        Link email = dtmEmail.BuildClass<Link>();
        email.LinkTypeId = (long)LinkType.Email;

        List<Link> links = new List<Link>{phone, whatsApp, email};

        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].CreateSprite($"{spriteName}_{i}").ToStrBase64(ImageType.JPG);

        productService.Register(new RegisterProductRequest(new RegisterPostRequest(post, contact, links, strImages),
                                                           product));
    }

    public void ApplyProduct(long productId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
