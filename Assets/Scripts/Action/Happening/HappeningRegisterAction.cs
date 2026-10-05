using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;

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

        dtmContact.ClearElements();
        dtmPhone.ClearElements();
        dtmWhatsApp.ClearElements();
        dtmEmail.ClearElements();

        dtmImagesVLL.ClearElements();
    }

    public void ClearContact()
    {
        dtmHasPhone.PopulateBuiltIn<String>("0");
        dtmHasWhatsApp.PopulateBuiltIn<String>("0");
        dtmHasEmail.PopulateBuiltIn<String>("0");
    }

    private void Register()
    {
        ChoiceDialog.Instance.Warning(disclaimerTitle, disclaimerMessage, DoRegister, null, "De acuerdo", "Regresar");
    }

    private void DoRegister()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        HappeningFull happeningFull = dtmHappeningFull.BuildClass<HappeningFull>();
        happeningFull.AppUserId = StateManager.Instance.AppUser.Id;
        happeningFull.PostCountryId =  StateManager.Instance.CurrentLocality.CountryId;
        happeningFull.PostStateId = StateManager.Instance.CurrentLocality.StateId;

        happeningFull.ContactFull = dtmContact.BuildClass<ContactFull>();

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
                ChoiceDialog.Instance.Error("Fecha inválida","La fecha y hora de finalización debe ser mayor que la fecha y hora de inicio.");
                return;
            }
        }

        happeningFull.LinkFulls = new();

        String hasPhone = dtmHasPhone.BuildBuiltIn<String>();
        if (hasPhone == "1")
        {
            Phone phone = dtmPhone.BuildClass<Phone>();
            if (phone != null && !string.IsNullOrWhiteSpace(phone.PhoneNumber))
                happeningFull.LinkFulls.Add(new LinkFull(0, (long)LinkType.Phone, 0, $"{phone.PhoneCountryId}|{phone.PhoneNumber}", 0));
        }

        String hasWhatsApp = dtmHasWhatsApp.BuildBuiltIn<String>();

        if (hasWhatsApp == "1")
        {
            Phone whatsApp = dtmWhatsApp.BuildClass<Phone>();
            if (whatsApp != null && !string.IsNullOrWhiteSpace(whatsApp.PhoneNumber))
                happeningFull.LinkFulls.Add(new LinkFull(0, (long)LinkType.WhatsApp, 0, $"{whatsApp.PhoneCountryId}|{whatsApp.PhoneNumber}", 0));
        }

        String hasEmail = dtmHasEmail.BuildBuiltIn<String>();
        if (hasEmail == "1")
        {
            LinkFull email = dtmEmail.BuildClass<LinkFull>();
            if (email != null && !String.IsNullOrWhiteSpace(email.Url))
            {
                email.LinkTypeId = (long)LinkType.Email;
                happeningFull.LinkFulls.Add(email);
            }
        }

        if (happeningFull.LinkFulls.Count == 0)
        {
            ChoiceDialog.Instance.Error("Información de contacto", "Debes ingresar al menos un teléfono, WhatsApp o correo electrónico.");
            return;
        }

        happeningFull.ImageSprites = dtmImagesVLL.BuildBuiltInList<Sprite>();

        happeningService.Register(happeningFull);
    }

    public void ApplyHappening(long happeningId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
