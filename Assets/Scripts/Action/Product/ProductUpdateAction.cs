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
    Toggle tglDiscountPrice = null;
    [SerializeField]
    InputField ifdDiscountPrice = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmProductFull = null;

    [SerializeField]
    Toggle tglHasContact = null;
    [SerializeField]
    Toggle tglHasPhone = null;
    [SerializeField]
    Toggle tglHasWhatsApp = null;
    [SerializeField]
    Toggle tglHasEmail = null;

    [SerializeField]
    DataMapper dtmContact = null;
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
    [Space, SerializeField]
    UnityEvent onContactChanged = null;

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

        dtmProductFull.PopulateClass<ProductFull>(productFull);

        tglDiscountPrice.Checked = this.productFull.DiscountPrice > 0d;

        // Contact

        if (productFull.ContactFull != null)
        {
            tglHasContact.Checked = true;
            onContactChanged.Invoke();

            dtmContact.PopulateClass<ContactFull>(productFull.ContactFull);
        }
        else
        {
            tglHasContact.Checked = false;
            onContactChanged.Invoke();
        }

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
                    tglHasPhone.Checked = true;
                    onContactChanged.Invoke();

                    String[] phoneStr = linkFull.Url.Split('|', StringSplitOptions.RemoveEmptyEntries);
                    if (phoneStr.Length >= 2)
                        dtmPhone.PopulateClass<Phone>(new Phone(Convert.ToInt64(phoneStr[0]), phoneStr[1]));
                    continue;
                }

                // WhatsApp
                if (linkFull.LinkTypeId == 3)
                {
                    tglHasWhatsApp.Checked = true;
                    onContactChanged.Invoke();

                    String[] whatsAppStr = linkFull.Url.Split('|', StringSplitOptions.RemoveEmptyEntries);
                    if (whatsAppStr.Length >= 2)
                        dtmWhatsApp.PopulateClass<Phone>(new Phone(Convert.ToInt64(whatsAppStr[0]), whatsAppStr[1]));
                    continue;
                }

                // Email
                if (linkFull.LinkTypeId == 4)
                {
                    tglHasEmail.Checked = true;
                    onContactChanged.Invoke();

                    dtmEmail.PopulateClass<LinkFull>(linkFull);
                    continue;
                }
            }
        }

        // Images
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

        if (tglHasPhone.Checked)
        {
            Phone phone = dtmPhone.BuildClass<Phone>();
            if (phone != null && !string.IsNullOrWhiteSpace(phone.PhoneNumber))
                linkFulls.Add(new LinkFull(0, (long)LinkType.Phone, productFull.PostId, $"{phone.PhoneCountryId}|{phone.PhoneNumber}", 0));
        }

        if (tglHasWhatsApp.Checked)
        {
            Phone whatsApp = dtmWhatsApp.BuildClass<Phone>();
            if (whatsApp != null && !string.IsNullOrWhiteSpace(whatsApp.PhoneNumber))
                linkFulls.Add(new LinkFull(0, (long)LinkType.WhatsApp, productFull.PostId, $"{whatsApp.PhoneCountryId}|{whatsApp.PhoneNumber}", 0));
        }

        if (tglHasEmail.Checked)
        {
            LinkFull email = dtmEmail.BuildClass<LinkFull>();
            if (email != null && !string.IsNullOrWhiteSpace(email.Url))
            {
                email.LinkTypeId = (long)LinkType.Email;
                email.PostId = productFull.PostId;
                linkFulls.Add(email);
            }
        }

        if (tglHasContact.Checked && linkFulls.Count == 0)
        {
            ChoiceDialog.Instance.Error("Información de contacto", "Debes ingresar al menos un teléfono, WhatsApp o correo electrónico.");
            return;
        }

        ScreenDialog.Instance.Display();

        productFull.Update(dtmProductFull.BuildClass<ProductFull>());

        // Contact
        ContactFull contactFull = dtmContact.BuildClass<ContactFull>();

        if (!tglHasContact.Checked)
        {
            productFull.ContactFull = null;
            productFull.LinkFulls = null;
        }
        else if (productFull.ContactFull == null)
        {
            productFull.ContactFull = contactFull;
            productFull.LinkFulls = linkFulls;
        }
        else if (contactFull != null)
        {
            productFull.ContactFull.Update(contactFull);
            productFull.LinkFulls = linkFulls;
        }

        //productFull.ContactFull?.Update(dtmContact.BuildClass<ContactFull>());
        //productFull.LinkFulls = linkFulls;

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
