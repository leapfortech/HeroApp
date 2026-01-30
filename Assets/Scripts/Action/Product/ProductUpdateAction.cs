using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;

using Sirenix.OdinInspector;

public class ProductUpdateAction : MonoBehaviour
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
    [SerializeField]
    DataMapper dtmImagesVLL = null;

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    ProductService productService = null;

    long postId = -1, productId = -1;
    Post post = null;
    Contact contact = null;
    Product product = null;

    private void Awake()
    {
        productService = GetComponent<ProductService>();
    }

    private void Start()
    {
        btnUpdate?.AddAction(DoUpdate);
    }

    public void Clear()
    {
        dtmPost.ClearElements();
        dtmProduct.ClearElements();
        dtmContact.ClearElements();
        dtmPhone.ClearElements();
        dtmWhatsApp.ClearElements();
        dtmEmail.ClearElements();
        dtmImagesVLL.ClearElements();
    }

    public void SetIds(long[] ids)
    {
        postId = ids[0];
        productId = ids[1];
    }

    public void Populate()
    {
        ProductFull productFull = StateManager.Instance.GetProductFullById(productId);

        post = new Post(productFull);
        dtmPost.PopulateClass<Post>(post);

        contact = new Contact(productFull.ContactFull);
        dtmContact.PopulateClass<Contact>(contact);

        dtmPhone.PopulateBuiltIn<String>(new Link(productFull.LinkFulls[0]).Url);
        dtmWhatsApp.PopulateBuiltIn<String>(new Link(productFull.LinkFulls[1]).Url);
        dtmEmail.PopulateBuiltIn<String>(new Link(productFull.LinkFulls[2]).Url);

        product = new Product(productFull);
        dtmProduct.PopulateClass<Product>(product);

        List<Sprite> images = StateManager.Instance.GetProductImagesById(productId);
        dtmImagesVLL.PopulateBuiltInList<Sprite>(images);
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        Post postNew = dtmPost.BuildClass<Post>();
        post.Title = postNew.Title;
        post.Summary = postNew.Summary;
        post.Description = postNew.Description;

        Contact contactNew = dtmContact.BuildClass<Contact>();
        contact.Name = contactNew.Name;

        List<Link> linkNews = new();

        Phone phone = dtmPhone.BuildClass<Phone>();
        if (phone != null && !string.IsNullOrWhiteSpace(phone.PhoneNumber))
            linkNews.Add(new Link(0, (long)LinkType.Phone, 0, $"{phone.PhoneCountryId}|{phone.PhoneNumber}", 0));

        Phone whatsApp = dtmWhatsApp.BuildClass<Phone>();
        if (whatsApp != null && !string.IsNullOrWhiteSpace(whatsApp.PhoneNumber))
            linkNews.Add(new Link(0, (long)LinkType.WhatsApp, 0, $"{whatsApp.PhoneCountryId}|{whatsApp.PhoneNumber}", 0));

        Link email = dtmEmail.BuildClass<Link>();
        if (email != null && !string.IsNullOrWhiteSpace(email.Url))
        {
            email.LinkTypeId = (long)LinkType.Email;
            linkNews.Add(email);
        }

        Product productNew = dtmProduct.BuildClass<Product>();
        product.ProductSubtypeId = productNew.ProductSubtypeId;
        product.SaleCountryId = productNew.SaleCountryId;
        product.SaleStateId = productNew.SaleStateId;
        product.CurrencyId = productNew.CurrencyId;
        product.Price = productNew.Price;
        product.DiscountPrice = productNew.DiscountPrice;
        product.DeliveryTypeId = productNew.DeliveryTypeId;
        product.Annotation = productNew.Annotation;

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        productService.UpdateProduct(new RegisterProductRequest(new RegisterPostRequest(post, contact, linkNews, strImages), product));
    }

    public void ApplyUpdate(bool updated)
    {
        if (!updated)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
