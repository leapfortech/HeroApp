using System;

using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;

public class ProductRegisterAction : MonoBehaviour
{
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
    DataMapper dtmImagesVLL = null;

    [Title("Action")]
    [SerializeField]
    Button btnRegister = null;

    [Title("Events")]
    [SerializeField]
    UnityEvent onCleared = null;

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
        dtmProductFull.ClearElements();
        dtmImagesVLL.ClearElements();

        tglDiscountPrice.Checked = false;
        tglHasContact.Checked = true;

        onCleared.Invoke();
    }

    private void Register()
    {
        if (!ElementHelper.Validate(elementValues))
            return;
        
        if (!ValidatePrice())
        {
            ChoiceDialog.Instance.Error("Precio de descuento", "El precio de descuento es mayor al precio regular.");
            return;
        }

        ScreenDialog.Instance.Display();

        ProductFull productFull = dtmProductFull.BuildClass<ProductFull>();
        productFull.AppUserId = StateManager.Instance.AppUser.Id;
        productFull.PostCountryId = interestLocality ? StateManager.Instance.InterestLocality.CountryId : StateManager.Instance.CurrentLocality.CountryId;
        productFull.PostStateId = interestLocality ? StateManager.Instance.InterestLocality.StateId : StateManager.Instance.CurrentLocality.StateId;

        // Contact
        productFull.ContactFull = dtmContact.BuildClass<ContactFull>();

        productFull.LinkFulls = new();

        bool hasContact = tglHasContact.Checked;

        bool hasPhone = tglHasPhone.Checked;
        if (hasPhone)
        {
            Phone phone = dtmPhone.BuildClass<Phone>();
            if (phone != null && !String.IsNullOrWhiteSpace(phone.PhoneNumber))
                productFull.LinkFulls.Add(new LinkFull(0, (long)LinkType.Phone, 0, $"{phone.PhoneCountryId}|{phone.PhoneNumber}", 0));
        }

        bool hasWhatsApp = tglHasWhatsApp.Checked;

        if (hasWhatsApp)
        {
            Phone whatsApp = dtmWhatsApp.BuildClass<Phone>();
            if (whatsApp != null && !String.IsNullOrWhiteSpace(whatsApp.PhoneNumber))
                productFull.LinkFulls.Add(new LinkFull(0, (long)LinkType.WhatsApp, 0, $"{whatsApp.PhoneCountryId}|{whatsApp.PhoneNumber}", 0));
        }

        bool hasEmail = tglHasEmail.Checked;
        if (hasEmail)
        {
            LinkFull email = dtmEmail.BuildClass<LinkFull>();
            if (email != null && !String.IsNullOrWhiteSpace(email.Url))
            {
                email.LinkTypeId = (long)LinkType.Email;
                productFull.LinkFulls.Add(email);
            }
        }

        //if (hasContact && productFull.LinkFulls.Count == 0)
        //{
        //    ChoiceDialog.Instance.Error("Información de contacto", "Debes ingresar al menos un teléfono, WhatsApp o correo electrónico.");
        //    return;
        //}

        //if (!hasContact)
        //{
        //    productFull.ContactFull = null;
        //    productFull.LinkFulls = null;
        //}

        // Images
        productFull.ImageSprites = dtmImagesVLL.BuildBuiltInList<Sprite>();

        productService.Register(productFull);
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

    public bool ValidatePrice()
    {
        double.TryParse(ifdPrice.Text, out double price);
        double.TryParse(ifdDiscountPrice.Text, out double discountPrice);

        if (discountPrice > 0 && discountPrice > price)
            return false;

        return true;
    }
}
