using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;
using Leap.Data.Collections;

using Sirenix.OdinInspector;

public class ProductUpdateAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;
    [Space, SerializeField]
    InputField ifdPrice = null;
    [SerializeField]
    InputField ifdDiscountPrice = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmPost = null;
    [SerializeField]
    DataMapper dtmProduct = null;
    [SerializeField]
    DataMapper dtmHasDiscountPrice = null;

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
    ValueList vllImages = null;
    [SerializeField]
    String spriteName = "Product";

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    [Title("Event")]
    [SerializeField]
    PostSpriteEvent onPostChanged = null;
    [SerializeField]
    UnityEvent onPopulated = null;

    ProductService productService = null;

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

        vllImages.ClearRecords();
    }

    public void ApplyFull(ProductFull productFull)
    {
        Clear();

        PostHelper.post = new Post(productFull);
        dtmPost.PopulateClass<Post>(PostHelper.post);

        contact = new Contact(productFull.ContactFull);
        dtmContact.PopulateClass<Contact>(contact);

        dtmHasPhone.PopulateBuiltIn<String>("0");
        dtmHasWhatsApp.PopulateBuiltIn<String>("0");
        dtmHasEmail.PopulateBuiltIn<String>("0");

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
                    dtmPhone.PopulateClass<Phone>(new Phone(Convert.ToInt64(phoneStr[0]), phoneStr[1]));
                continue;
            }

            // WhatsApp
            if (linkFull.LinkTypeId == 3)
            {
                dtmHasWhatsApp.PopulateBuiltIn<String>("1");

                String[] whatsAppStr = linkFull.Url.Split('|', StringSplitOptions.RemoveEmptyEntries);
                if (whatsAppStr.Length >= 2)
                    dtmWhatsApp.PopulateClass<Phone>(new Phone(Convert.ToInt64(whatsAppStr[0]), whatsAppStr[1]));
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
        dtmHasDiscountPrice.PopulateBuiltIn<String>(product.DiscountPrice == 0.0f ? "0" : "1");
        dtmProduct.PopulateClass<Product>(product);

        for (int i = 0; i < productFull.ImageSprites.Count; i++)
            vllImages.AddRecord(productFull.ImageSprites[i].Clone($"Edt_{spriteName}_{i}"));

        onPopulated.Invoke();
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        if (!ValidatePrice())
        {
            ChoiceDialog.Instance.Error("Precio de descuento", "El precio de descuento es mayor al precio regular.");
            return;
        }

        ScreenDialog.Instance.Display();

        PostHelper.post.Update(dtmPost.BuildClass<Post>());

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

        String[] strImages = new String[vllImages.RecordCount];
        for (int i = 0; i < vllImages.RecordCount; i++)
            strImages[i] = vllImages[i].GetCellSprite(0).ToStrBase64(ImageType.JPG);

        PostHelper.post.ImageCount = vllImages.RecordCount;
        PostHelper.titleSprite = vllImages.RecordCount == 0 ? null : vllImages[0].GetCellSprite(0);

        productService.UpdateProduct(new RegisterProductRequest(PostHelper.post, contact, links, strImages, product));
    }

    public void ApplyUpdate(bool updated)
    {
        if (!updated)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        onPostChanged.Invoke(PostHelper.post, PostHelper.titleSprite);

        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }

    public bool ValidatePrice()
    {
        double.TryParse(ifdPrice.Text, out double price);
        double.TryParse(ifdDiscountPrice.Text, out double discountPrice);

        if (discountPrice > 0 && discountPrice > price)
            return false;

        return true;
    }
}
