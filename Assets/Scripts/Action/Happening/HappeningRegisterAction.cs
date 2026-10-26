using System;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Extensions;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;

public class HappeningRegisterAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmHappeningFull = null;
    [SerializeField]
    DataMapper dtmStartTime = null;
    [SerializeField]
    DataMapper dtmEndTime = null;


    [SerializeField]
    Toggle tglIsPublic = null;
    [SerializeField]
    Toggle tglHasSignup = null;
    [SerializeField]
    Toggle tglHasPayment = null;

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

    [SerializeField]
    ComboAdapter cmbHappeningCountry = null;
    [SerializeField]
    ComboAdapter cmbHappeningState = null;

    [Title("Action")]
    [SerializeField]
    Button btnRegister = null;

    [Title("Events")]
    [SerializeField]
    UnityEvent onCleared = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    [Title("Message")]
    [SerializeField]
    String disclaimerTitle = "Aviso importante";
    [Space, SerializeField, TextArea(2, 4)]
    String disclaimerMessage = "Te recomendamos";

    HappeningService happeningService = null;

    private void Awake()
    {
        happeningService = GetComponent<HappeningService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(Register);
    }

    public void Clear()
    {
        dtmHappeningFull.ClearElements();

        dtmStartTime.ClearElements();
        dtmEndTime.ClearElements();
        dtmImagesVLL.ClearElements();

        tglHasPayment.Checked = false;
        tglIsPublic.Checked = false;
        tglHasSignup.Checked = false;

        onCleared.Invoke();

        FillCombos();
    }

    public void FillCombos()
    {
        cmbHappeningCountry.Select(StateManager.Instance.CurrentLocality.CountryId);
        cmbHappeningState.Select(StateManager.Instance.CurrentLocality.StateId);
    }

    private void Register()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        HappeningFull happeningFull = dtmHappeningFull.BuildClass<HappeningFull>();
        happeningFull.AppUserId = StateManager.Instance.AppUser.Id;
        happeningFull.PostCountryId = StateManager.Instance.CurrentLocality.CountryId;
        happeningFull.PostStateId = StateManager.Instance.CurrentLocality.StateId;

        if (happeningFull.StartDateTime.HasValue && happeningFull.EndDateTime.HasValue)
        {
            String startTimeStr = dtmStartTime.BuildBuiltIn<String>();
            String[] startTime = startTimeStr.Split('|');
            happeningFull.StartDateTime = new DateTime(happeningFull.StartDateTime.Value.Year, happeningFull.StartDateTime.Value.Month, happeningFull.StartDateTime.Value.Day,
                                                   Convert.ToInt32(startTime[0]), Convert.ToInt32(startTime[1]), 0);

            String endTimeStr = dtmEndTime.BuildBuiltIn<String>();
            String[] endTime = endTimeStr.Split('|');
            happeningFull.EndDateTime = new DateTime(happeningFull.EndDateTime.Value.Year, happeningFull.EndDateTime.Value.Month, happeningFull.EndDateTime.Value.Day,
                                                 Convert.ToInt32(endTime[0]), Convert.ToInt32(endTime[1]), 0);

            if (happeningFull.EndDateTime.Value <= happeningFull.StartDateTime.Value)
            {
                ChoiceDialog.Instance.Error("Fecha inválida", "La fecha y hora de finalización debe ser mayor que la fecha y hora de inicio.");
                return;
            }
        }

        happeningFull.IsPublic = tglIsPublic.Checked ? 1 : 0;
        happeningFull.HasSignup = tglHasSignup.Checked ? 1 : 0;
        happeningFull.HasPayment = tglHasPayment.Checked ? 1 : 0;

        // Contact
        happeningFull.ContactFull = dtmContact.BuildClass<ContactFull>();

        happeningFull.LinkFulls = new();

        bool hasContact = tglHasContact.Checked;

        bool hasPhone = tglHasPhone.Checked;
        if (hasPhone)
        {
            Phone phone = dtmPhone.BuildClass<Phone>();
            if (phone != null && !String.IsNullOrWhiteSpace(phone.PhoneNumber))
                happeningFull.LinkFulls.Add(new LinkFull(0, (long)LinkType.Phone, 0, $"{phone.PhoneCountryId}|{phone.PhoneNumber}", 0));
        }

        bool hasWhatsApp = tglHasWhatsApp.Checked;

        if (hasWhatsApp)
        {
            Phone whatsApp = dtmWhatsApp.BuildClass<Phone>();
            if (whatsApp != null && !String.IsNullOrWhiteSpace(whatsApp.PhoneNumber))
                happeningFull.LinkFulls.Add(new LinkFull(0, (long)LinkType.WhatsApp, 0, $"{whatsApp.PhoneCountryId}|{whatsApp.PhoneNumber}", 0));
        }

        bool hasEmail = tglHasEmail.Checked;
        if (hasEmail)
        {
            LinkFull email = dtmEmail.BuildClass<LinkFull>();
            if (email != null && !String.IsNullOrWhiteSpace(email.Url))
            {
                email.LinkTypeId = (long)LinkType.Email;
                happeningFull.LinkFulls.Add(email);
            }
        }

        if (hasContact && happeningFull.LinkFulls.Count == 0)
        {
            ChoiceDialog.Instance.Error("Información de contacto", "Debes ingresar al menos un teléfono, WhatsApp o correo electrónico.");
            return;
        }

        if (!hasContact)
        {
            happeningFull.ContactFull = null;
            happeningFull.LinkFulls = null;
        }

        // Images
        happeningFull.ImageSprites = dtmImagesVLL.BuildBuiltInList<Sprite>();

        ChoiceDialog.Instance.Warning(disclaimerTitle, disclaimerMessage, () => DoRegister(happeningFull), null, "De acuerdo", "Regresar");
    }

    private void DoRegister(HappeningFull happeningFull)
    {
        ScreenDialog.Instance.Display();
        happeningService.Register(happeningFull);
    }

    public void ApplyHappening(long happeningId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
