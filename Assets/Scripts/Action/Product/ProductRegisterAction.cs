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
    Button btnRegister = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    ProductService productService = null;

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
        dtmImagesVLL.ClearElements();
    }

    private void Register()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        Post post = dtmPost.BuildClass<Post>();
        post.AppUserId = StateManager.Instance.AppUser.Id;
        post.CountryId = interestLocality ? StateManager.Instance.InterestLocality.CountryId : StateManager.Instance.CurrentLocality.CountryId;
        post.StateId = interestLocality ? StateManager.Instance.InterestLocality.StateId : StateManager.Instance.CurrentLocality.StateId;

        Contact contact = dtmContact.BuildClass<Contact>();

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

        Product product = dtmProduct.BuildClass<Product>();

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        productService.Register(new RegisterProductRequest(post, contact, links, strImages, product));
    }

    public void ApplyProduct(long productId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }

    // Locality

    bool interestLocality = true;

    public void ApplyLocality(bool interestLocality)
    {
        this.interestLocality = interestLocality;
    }
}
