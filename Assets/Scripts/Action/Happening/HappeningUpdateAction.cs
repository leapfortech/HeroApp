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

using Sirenix.OdinInspector;

public class HappeningUpdateAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmPost = null;

    [SerializeField]
    DataMapper dtmHappening = null;
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
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    [Title("Event")]
    [SerializeField]
    PostSpriteEvent onPostChanged = null;
    [SerializeField]
    UnityEvent onPopulated = null;

    HappeningService happeningService = null;

    Contact contact = null;
    Happening happening = null;

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
        dtmPost.ClearElements();

        dtmHappening.ClearElements();
        dtmStartTime.ClearElements();
        dtmEndTime.ClearElements();

        dtmContact.ClearElements();
        dtmPhone.ClearElements();
        dtmWhatsApp.ClearElements();
        dtmEmail.ClearElements();

        dtmImagesVLL.ClearElements();
    }

    public void ApplyFull(HappeningFull happeningFull)
    {
        Clear();

        PostHelper.post = new Post(happeningFull);
        dtmPost.PopulateClass<Post>(PostHelper.post);

        contact = new Contact(happeningFull.ContactFull);
        dtmContact.PopulateClass<Contact>(contact);

        dtmHasPhone.PopulateBuiltIn<string>("0");
        dtmHasWhatsApp.PopulateBuiltIn<string>("0");
        dtmHasEmail.PopulateBuiltIn<string>("0");

        if (happeningFull.LinkFulls == null)
            return;

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
                dtmEmail.PopulateClass<Link>(new Link(linkFull));
                continue;
            }
        }

        happening = new Happening(happeningFull);
        dtmHappening.PopulateClass<Happening>(happening);
        String startTimeStr = happening.StartDateTime.Value.ToString("HH|mm", CultureInfo.InvariantCulture);
        dtmStartTime.PopulateBuiltIn<String>(startTimeStr);

        String endTimeStr = happening.EndDateTime.Value.ToString("HH|mm", CultureInfo.InvariantCulture);
        dtmEndTime.PopulateBuiltIn<String>(endTimeStr);

        dtmImagesVLL.PopulateBuiltInList<Sprite>(happeningFull.ImageSprites);

        onPopulated.Invoke();
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

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

        happening.Update(dtmHappening.BuildClass<Happening>());

        if (happening.StartDateTime.HasValue && happening.EndDateTime.HasValue)
        {
            String[] startTime = dtmStartTime.BuildBuiltIn<String>().Split('|');
            happening.StartDateTime = new DateTime(happening.StartDateTime.Value.Year, happening.StartDateTime.Value.Month, happening.StartDateTime.Value.Day,
                                                   Convert.ToInt32(startTime[0]), Convert.ToInt32(startTime[1]), 0);

            String[] endTime = dtmEndTime.BuildBuiltIn<String>().Split('|');
            happening.EndDateTime = new DateTime(happening.EndDateTime.Value.Year, happening.EndDateTime.Value.Month, happening.EndDateTime.Value.Day,
                                                 Convert.ToInt32(endTime[0]), Convert.ToInt32(endTime[1]), 0);

            if (happening.EndDateTime.Value <= happening.StartDateTime.Value)
            {
                ChoiceDialog.Instance.Error("Fecha inválida","La fecha y hora de finalización debe ser mayor que la fecha y hora de inicio.");
                return;
            }
        }

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        PostHelper.post.ImageCount = images.Count;
        PostHelper.titleSprite = images.Count == 0 ? null : images[0];

        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        happeningService.UpdateHappening(new RegisterHappeningRequest(PostHelper.post, contact, links, strImages, happening));
    }

    public void ApplyHappening(bool updated)
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
}
