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
    [Serializable]
    class ProductFeedEvent : UnityEvent<ProductFeed> { }

    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;
    [Space, SerializeField]
    InputField ifdPrice = null;
    [SerializeField]
    InputField ifdDiscountPrice = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmProductFull = null;
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

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page[] pagNexts = null;

    [Title("Events")]
    [SerializeField]
    ProductFeedEvent[] onProductChanged = null;
    [Space, SerializeField]
    UnityEvent onPopulated = null;

    ProductService productService = null;

    ProductFull productFull = null;

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
        productFull = null;

        dtmProductFull.ClearElements();

        dtmContact.ClearElements();
        dtmPhone.ClearElements();
        dtmWhatsApp.ClearElements();
        dtmEmail.ClearElements();

        vllImages.ClearRecords();
    }

    public void ApplyFull(ProductFull productFull)
    {
        Clear();

        this.productFull = productFull;

        dtmHasDiscountPrice.PopulateBuiltIn<String>(productFull.DiscountPrice == 0.0f ? "0" : "1");

        dtmProductFull.PopulateClass<ProductFull>(productFull);

        if (productFull.ContactFull != null)
            dtmContact.PopulateClass<ContactFull>(productFull.ContactFull);

        dtmHasPhone.PopulateBuiltIn<String>("0");
        dtmHasWhatsApp.PopulateBuiltIn<String>("0");
        dtmHasEmail.PopulateBuiltIn<String>("0");

        if (productFull.LinkFulls != null)
        {
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
                    dtmEmail.PopulateClass<LinkFull>(linkFull);
                    continue;
                }
            }
        }

        for (int i = 0; i < productFull.ImageSprites.Count; i++)
            vllImages.AddRecord(productFull.ImageSprites[i].Clone($"Edt_{PostType.Names[PostType.Product]}_{i}"));

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

        List<LinkFull> linkFulls = new();

        String hasPhone = dtmHasPhone.BuildBuiltIn<String>();
        if (hasPhone == "1")
        {
            Phone phone = dtmPhone.BuildClass<Phone>();
            if (phone != null && !String.IsNullOrWhiteSpace(phone.PhoneNumber))
                linkFulls.Add(new LinkFull(0, (long)LinkType.Phone, productFull.PostId, $"{phone.PhoneCountryId}|{phone.PhoneNumber}", 0));
        }

        String hasWhatsApp = dtmHasWhatsApp.BuildBuiltIn<String>();
        if (hasWhatsApp == "1")
        {
            Phone whatsApp = dtmWhatsApp.BuildClass<Phone>();
            if (whatsApp != null && !String.IsNullOrWhiteSpace(whatsApp.PhoneNumber))
                linkFulls.Add(new LinkFull(0, (long)LinkType.WhatsApp, productFull.PostId, $"{whatsApp.PhoneCountryId}|{whatsApp.PhoneNumber}", 0));
        }

        String hasEmail = dtmHasEmail.BuildBuiltIn<String>();
        if (hasEmail == "1")
        {
            LinkFull email = dtmEmail.BuildClass<LinkFull>();
            if (email != null && !String.IsNullOrWhiteSpace(email.Url))
            {
                email.LinkTypeId = (long)LinkType.Email;
                email.PostId = productFull.PostId;
                linkFulls.Add(email);
            }
        }

        if (linkFulls.Count == 0)
        {
            ChoiceDialog.Instance.Error("Información de contacto", "Debes ingresar al menos un teléfono, WhatsApp o correo electrónico.");
            return;
        }

        ScreenDialog.Instance.Display();

        productFull.Update(dtmProductFull.BuildClass<ProductFull>());

        productFull.ContactFull?.Update(dtmContact.BuildClass<ContactFull>());

        productFull.LinkFulls = linkFulls;

        String[] strImages = new String[vllImages.RecordCount];
        for (int i = 0; i < vllImages.RecordCount; i++)
            strImages[i] = vllImages[i].GetCellSprite(0).ToStrBase64(ImageType.JPG);
        productFull.Images = strImages;
        productFull.ImageCount = vllImages.RecordCount;
        productFull.TitleSprite = vllImages.RecordCount == 0 ? null : vllImages[0].GetCellSprite(0);

        productService.UpdateProduct(productFull);
    }

    public void ApplyUpdate(bool updated)
    {
        if (!updated)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        onProductChanged[StateManager.Instance.FeedDetailType].Invoke(new ProductFeed(productFull));

        Clear();
        PageManager.Instance.ChangePage(pagNexts[StateManager.Instance.FeedDetailType]);
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
