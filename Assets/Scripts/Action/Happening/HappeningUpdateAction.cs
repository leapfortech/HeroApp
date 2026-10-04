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
    Page pagNext = null;

    [Title("Event")]
    [SerializeField]
    HappeningFeedEvent onHappeningChanged = null;
    [SerializeField]
    UnityEvent onPopulated = null;

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

        dtmContact.ClearElements();
        dtmPhone.ClearElements();
        dtmWhatsApp.ClearElements();
        dtmEmail.ClearElements();

        vllImages.ClearRecords();
    }

    public void ApplyFull(HappeningFull happeningFull)
    {
        Clear();

        this.happeningFull = happeningFull;

        dtmHappeningFull.PopulateClass<HappeningFull>(happeningFull);

        if (happeningFull.ContactFull != null)
            dtmContact.PopulateClass<ContactFull>(happeningFull.ContactFull);

        dtmHasPhone.PopulateBuiltIn<String>("0");
        dtmHasWhatsApp.PopulateBuiltIn<String>("0");
        dtmHasEmail.PopulateBuiltIn<String>("0");

        String startTimeStr = this.happeningFull.StartDateTime.Value.ToString("HH|mm", CultureInfo.InvariantCulture);
        dtmStartTime.PopulateBuiltIn<String>(startTimeStr);

        String endTimeStr = this.happeningFull.EndDateTime.Value.ToString("HH|mm", CultureInfo.InvariantCulture);
        dtmEndTime.PopulateBuiltIn<String>(endTimeStr);

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

        for (int i = 0; i < happeningFull.ImageSprites.Count; i++)
            vllImages.AddRecord(happeningFull.ImageSprites[i].Clone($"Edt_{PostType.Names[PostType.Happening]}_{i}"));

        onPopulated.Invoke();
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        happeningFull.Update(dtmHappeningFull.BuildClass<PostFull>());

        happeningFull.ContactFull?.Update(dtmContact.BuildClass<ContactFull>());

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

        List<LinkFull> linkFulls = new();

        String hasPhone = dtmHasPhone.BuildBuiltIn<String>();
        if (hasPhone == "1")
        {
            Phone phone = dtmPhone.BuildClass<Phone>();
            if (phone != null && !string.IsNullOrWhiteSpace(phone.PhoneNumber))
                linkFulls.Add(new LinkFull(0, (long)LinkType.Phone, 0, $"{phone.PhoneCountryId}|{phone.PhoneNumber}", 0));
        }

        String hasWhatsApp = dtmHasWhatsApp.BuildBuiltIn<String>();
        if (hasWhatsApp == "1")
        {
            Phone whatsApp = dtmWhatsApp.BuildClass<Phone>();
            if (whatsApp != null && !string.IsNullOrWhiteSpace(whatsApp.PhoneNumber))
                linkFulls.Add(new LinkFull(0, (long)LinkType.WhatsApp, 0, $"{whatsApp.PhoneCountryId}|{whatsApp.PhoneNumber}", 0));
        }

        String hasEmail = dtmHasEmail.BuildBuiltIn<String>();
        if (hasEmail == "1")
        {
            LinkFull email = dtmEmail.BuildClass<LinkFull>();
            if (email != null && !string.IsNullOrWhiteSpace(email.Url))
            {
                email.LinkTypeId = (long)LinkType.Email;
                linkFulls.Add(email);
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

        onHappeningChanged.Invoke(new HappeningFeed(happeningFull));

        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
