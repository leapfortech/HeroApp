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
    DataMapper dtmHasPhone = null;
    [SerializeField]
    DataMapper dtmHasWhatsApp = null;
    [SerializeField]
    DataMapper dtmHasEmail = null;
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

        if (productFull.LinkFulls?.Count > 0 && productFull.LinkFulls[0] != null)
        {
            dtmHasPhone.PopulateBuiltIn<String>("1");
            String[] phoneStr = productFull.LinkFulls[0].Url.Split('|', StringSplitOptions.RemoveEmptyEntries);
            dtmPhone.PopulateClass<Phone>(new Phone(Convert.ToInt64(phoneStr[0]), phoneStr[1]));
        }
        else
            dtmHasPhone.PopulateBuiltIn<String>("0");

        if (productFull.LinkFulls?.Count > 1 && productFull.LinkFulls[1] != null)
        {
            dtmHasWhatsApp.PopulateBuiltIn<String>("1");
            String[] whatsAppStr = productFull.LinkFulls[1].Url.Split('|', StringSplitOptions.RemoveEmptyEntries);
            dtmWhatsApp.PopulateClass<Phone>(new Phone(Convert.ToInt64(whatsAppStr[0]), whatsAppStr[1]));
        }
        else
            dtmHasWhatsApp.PopulateBuiltIn<String>("0");

        if (productFull.LinkFulls?.Count > 2 && productFull.LinkFulls[2] != null)
        {
            dtmHasEmail.PopulateBuiltIn<String>("1");
            dtmEmail.PopulateClass<Link>(new Link(productFull.LinkFulls[2]));
        }
        else
            dtmHasEmail.PopulateBuiltIn<String>("0");

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

        post.Update(dtmPost.BuildClass<Post>());

        contact.Update(dtmContact.BuildClass<Contact>());

        List<Link> linkNews = new();

        String hasPhone = dtmHasPhone.BuildBuiltIn<String>();
        if (hasPhone == "1")
        {
            Phone phone = dtmPhone.BuildClass<Phone>();
            if (phone != null && !string.IsNullOrWhiteSpace(phone.PhoneNumber))
                linkNews.Add(new Link(0, (long)LinkType.Phone, 0, $"{phone.PhoneCountryId}|{phone.PhoneNumber}", 0));
        }

        String hasWhatsApp = dtmHasWhatsApp.BuildBuiltIn<String>();
        if (hasWhatsApp == "1")
        {
            Phone whatsApp = dtmWhatsApp.BuildClass<Phone>();
            if (whatsApp != null && !string.IsNullOrWhiteSpace(whatsApp.PhoneNumber))
                linkNews.Add(new Link(0, (long)LinkType.WhatsApp, 0, $"{whatsApp.PhoneCountryId}|{whatsApp.PhoneNumber}", 0));
        }

        String hasEmail = dtmHasEmail.BuildBuiltIn<String>();
        if (hasEmail == "1")
        {
            Link email = dtmEmail.BuildClass<Link>();
            if (email != null && !string.IsNullOrWhiteSpace(email.Url))
            {
                email.LinkTypeId = (long)LinkType.Email;
                linkNews.Add(email);
            }
        }

        product.Update(dtmProduct.BuildClass<Product>());

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
