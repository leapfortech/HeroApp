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

    public void ApplyFull(ProductFull productFull)
    {
        post = new Post(productFull);
        dtmPost.PopulateClass<Post>(post);

        contact = new Contact(productFull.ContactFull);
        dtmContact.PopulateClass<Contact>(contact);

        dtmHasPhone.PopulateBuiltIn<string>("0");
        dtmHasWhatsApp.PopulateBuiltIn<string>("0");
        dtmHasEmail.PopulateBuiltIn<string>("0");

        if (productFull.LinkFulls == null)
            return;

        for (int i = 0; i < productFull.LinkFulls.Count; i++)
        {
            LinkFull linkFull = productFull.LinkFulls[i];
            if (linkFull == null)
                continue;

            // Phone
            if (linkFull.LinkTypeId == 2)
            {
                dtmHasPhone.PopulateBuiltIn<String>("1");

                String[] phoneStr = linkFull.Url.Split('|', StringSplitOptions.RemoveEmptyEntries);
                if (phoneStr.Length >= 2)
                    dtmPhone.PopulateClass<Phone>(
                        new Phone(Convert.ToInt64(phoneStr[0]), phoneStr[1])
                    );

                continue;
            }

            // WhatsApp
            if (linkFull.LinkTypeId == 3)
            {
                dtmHasWhatsApp.PopulateBuiltIn<String>("1");

                String[] whatsAppStr = linkFull.Url.Split('|', StringSplitOptions.RemoveEmptyEntries);
                if (whatsAppStr.Length >= 2)
                    dtmWhatsApp.PopulateClass<Phone>(
                        new Phone(Convert.ToInt64(whatsAppStr[0]), whatsAppStr[1])
                    );

                continue;
            }

            // Email
            if (linkFull.LinkTypeId == 4)
            {
                dtmHasEmail.PopulateBuiltIn<String>("1");
                dtmEmail.PopulateClass<Link>(new Link(linkFull));
                continue;
            }
        }

        product = new Product(productFull);
        dtmProduct.PopulateClass<Product>(product);

        dtmImagesVLL.PopulateBuiltInList<Sprite>(productFull.ImageSprites);
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        post.Update(dtmPost.BuildClass<Post>());

        contact.Update(dtmContact.BuildClass<Contact>());

        List<Link> links = new();

        String hasPhone = dtmHasPhone.BuildBuiltIn<String>();
        if (hasPhone == "1")
        {
            Phone phone = dtmPhone.BuildClass<Phone>();
            if (phone != null && !string.IsNullOrWhiteSpace(phone.PhoneNumber))
                links.Add(new Link(0, (long)LinkType.Phone, 0, $"{phone.PhoneCountryId}|{phone.PhoneNumber}", 0));
        }

        String hasWhatsApp = dtmHasWhatsApp.BuildBuiltIn<String>();
        if (hasWhatsApp == "1")
        {
            Phone whatsApp = dtmWhatsApp.BuildClass<Phone>();
            if (whatsApp != null && !string.IsNullOrWhiteSpace(whatsApp.PhoneNumber))
                links.Add(new Link(0, (long)LinkType.WhatsApp, 0, $"{whatsApp.PhoneCountryId}|{whatsApp.PhoneNumber}", 0));
        }

        String hasEmail = dtmHasEmail.BuildBuiltIn<String>();
        if (hasEmail == "1")
        {
            Link email = dtmEmail.BuildClass<Link>();
            if (email != null && !string.IsNullOrWhiteSpace(email.Url))
            {
                email.LinkTypeId = (long)LinkType.Email;
                links.Add(email);
            }
        }

        product.Update(dtmProduct.BuildClass<Product>());

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        productService.UpdateProduct(new RegisterProductRequest(post, contact, links, strImages, product));
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
