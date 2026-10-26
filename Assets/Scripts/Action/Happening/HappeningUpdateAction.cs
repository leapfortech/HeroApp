using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;
using Leap.Data.Collections;

using Sirenix.OdinInspector;

public class HappeningUpdateAction : MonoBehaviour
{
    [Serializable]
    class HappeningFeedEvent : UnityEvent<HappeningFeed> { }

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
    ValueList vllImages = null;

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page[] pagNexts = null;

    [Title("Events")]
    [SerializeField]
    HappeningFeedEvent[] onHappeningChanged = null;
    [Space, SerializeField]
    UnityEvent onPopulated = null;
    [Space, SerializeField]
    UnityEvent onContactChanged = null;

    HappeningService happeningService = null;

    HappeningFull happeningFull = null;

    private void Awake()
    {
        happeningService = GetComponent<HappeningService>();
    }

    private void Start()
    {
        btnUpdate?.AddAction(DoUpdate);
    }

    public void Clear()
    {
        happeningFull = null;

        dtmHappeningFull.ClearElements();

        dtmStartTime.ClearElements();
        dtmEndTime.ClearElements();

        vllImages.ClearRecords();
    }

    public void ApplyFull(HappeningFull happeningFull)
    {
        Clear();

        this.happeningFull = happeningFull;

        dtmHappeningFull.PopulateClass<HappeningFull>(happeningFull);

        tglIsPublic.Checked = this.happeningFull.IsPublic == 1;
        tglHasSignup.Checked = this.happeningFull.HasSignup == 1;
        tglHasPayment.Checked = this.happeningFull.HasPayment == 1;

        String startTimeStr = this.happeningFull.StartDateTime.Value.ToString("HH|mm", CultureInfo.InvariantCulture);
        dtmStartTime.PopulateBuiltIn<String>(startTimeStr);

        String endTimeStr = this.happeningFull.EndDateTime.Value.ToString("HH|mm", CultureInfo.InvariantCulture);
        dtmEndTime.PopulateBuiltIn<String>(endTimeStr);

        // Contact

        if (happeningFull.ContactFull != null)
        {
            tglHasContact.Checked = true;
            onContactChanged.Invoke();

            dtmContact.PopulateClass<ContactFull>(happeningFull.ContactFull);
        }
        else
        {
            tglHasContact.Checked = false;
            onContactChanged.Invoke();
        }

        if (happeningFull.LinkFulls != null)
        {
            for (int i = 0; i < happeningFull.LinkFulls.Count; i++)
            {
                LinkFull linkFull = happeningFull.LinkFulls[i];
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
        for (int i = 0; i < happeningFull.ImageSprites.Count; i++)
            vllImages.AddRecord(happeningFull.ImageSprites[i].Clone($"Edt_{PostType.Names[PostType.Happening]}_{i}"));

        onPopulated.Invoke();
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        List<LinkFull> linkFulls = new();

        if (tglHasPhone.Checked)
        {
            Phone phone = dtmPhone.BuildClass<Phone>();
            if (phone != null && !string.IsNullOrWhiteSpace(phone.PhoneNumber))
                linkFulls.Add(new LinkFull(0, (long)LinkType.Phone, happeningFull.PostId, $"{phone.PhoneCountryId}|{phone.PhoneNumber}", 0));
        }

        if (tglHasWhatsApp.Checked)
        {
            Phone whatsApp = dtmWhatsApp.BuildClass<Phone>();
            if (whatsApp != null && !string.IsNullOrWhiteSpace(whatsApp.PhoneNumber))
                linkFulls.Add(new LinkFull(0, (long)LinkType.WhatsApp, happeningFull.PostId, $"{whatsApp.PhoneCountryId}|{whatsApp.PhoneNumber}", 0));
        }

        if (tglHasEmail.Checked)
        {
            LinkFull email = dtmEmail.BuildClass<LinkFull>();
            if (email != null && !string.IsNullOrWhiteSpace(email.Url))
            {
                email.LinkTypeId = (long)LinkType.Email;
                email.PostId = happeningFull.PostId;
                linkFulls.Add(email);
            }
        }

        if (tglHasContact.Checked && linkFulls.Count == 0)
        {
            ChoiceDialog.Instance.Error("Información de contacto", "Debes ingresar al menos un teléfono, WhatsApp o correo electrónico.");
            return;
        }

        ScreenDialog.Instance.Display();

        happeningFull.Update(dtmHappeningFull.BuildClass<HappeningFull>());

        happeningFull.IsPublic = tglIsPublic.Checked ? 1 : 0;
        happeningFull.HasSignup = tglHasSignup.Checked ? 1 : 0;
        happeningFull.HasPayment = tglHasPayment.Checked ? 1 : 0;

        // Contact
        ContactFull contactFull = dtmContact.BuildClass<ContactFull>();

        if (!tglHasContact.Checked)
        {
            happeningFull.ContactFull = null;
            happeningFull.LinkFulls = null;
        }
        else if (happeningFull.ContactFull == null)
        {
            happeningFull.ContactFull = contactFull;
            happeningFull.LinkFulls = linkFulls;
        }
        else if (contactFull != null)
        {
            happeningFull.ContactFull.Update(contactFull);
            happeningFull.LinkFulls = linkFulls;
        }

        // Dates
        if (happeningFull.StartDateTime.HasValue && happeningFull.EndDateTime.HasValue)
        {
            String[] startTime = dtmStartTime.BuildBuiltIn<String>().Split('|');
            happeningFull.StartDateTime = new DateTime(happeningFull.StartDateTime.Value.Year, happeningFull.StartDateTime.Value.Month, happeningFull.StartDateTime.Value.Day,
                                                   Convert.ToInt32(startTime[0]), Convert.ToInt32(startTime[1]), 0);

            String[] endTime = dtmEndTime.BuildBuiltIn<String>().Split('|');
            happeningFull.EndDateTime = new DateTime(happeningFull.EndDateTime.Value.Year, happeningFull.EndDateTime.Value.Month, happeningFull.EndDateTime.Value.Day,
                                                 Convert.ToInt32(endTime[0]), Convert.ToInt32(endTime[1]), 0);

            if (happeningFull.EndDateTime.Value <= happeningFull.StartDateTime.Value)
            {
                ChoiceDialog.Instance.Error("Fecha inválida", "La fecha y hora de finalización debe ser mayor que la fecha y hora de inicio.");
                return;
            }
        }

        String[] strImages = new String[vllImages.RecordCount];
        for (int i = 0; i < vllImages.RecordCount; i++)
            strImages[i] = vllImages[i].GetCellSprite(0).ToStrBase64(ImageType.JPG);
        happeningFull.Images = strImages;
        happeningFull.ImageCount = vllImages.RecordCount;
        happeningFull.TitleSprite = vllImages.RecordCount == 0 ? null : vllImages[0].GetCellSprite(0);

        happeningService.UpdateHappening(happeningFull);
    }

    public void ApplyHappening(bool updated)
    {
        if (!updated)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        onHappeningChanged[StateManager.Instance.FeedDetailType].Invoke(new HappeningFeed(happeningFull));

        Clear();
        PageManager.Instance.ChangePage(pagNexts[StateManager.Instance.FeedDetailType]);
    }
}
