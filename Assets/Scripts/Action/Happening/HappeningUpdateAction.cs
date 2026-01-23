using System;
using System.Collections.Generic;
using UnityEngine;

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
    DataMapper dtmImagesVLL = null;

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    HappeningService happeningService = null;

    HappeningFull happeningFull = null;
    Post post = null;
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
        dtmImagesVLL.ClearElements();
    }

    public void SetHappeningFull(HappeningFull happeningFull)
    {
        this.happeningFull = happeningFull;
    }

    public void Populate()
    {
        post = new Post(happeningFull);
        dtmPost.PopulateClass<Post>(post);

        happening = new Happening(happeningFull);
        dtmHappening.PopulateClass<Happening>(happening);
        dtmStartTime.PopulateBuiltIn<String>(happening.StartDateTime != null ? happening.StartDateTime.Value.ToString("HH|mm") : null);
        dtmEndTime.PopulateBuiltIn<String>(happening.EndDateTime != null ? happening.EndDateTime.Value.ToString("HH|mm") : null);

        //List<Sprite> images = StateManager.Instance.GetHappeningImagesById(treatmentFull.Id);
        //dtmImagesVLL.PopulateBuiltInList<Sprite>(images);
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        Post postNew = dtmPost.BuildClass<Post>();
        post.Title = postNew.Title;
        post.Summary = postNew.Summary;
        post.Description = postNew.Description;

        Happening happeningNew = dtmHappening.BuildClass<Happening>();
        happening.EventTypeId = happeningNew.EventTypeId;
        happening.CountryId = happeningNew.CountryId;
        happening.StateId = happeningNew.StateId;
        happening.IsPublic = happeningNew.IsPublic;
        happening.HasSignup = happeningNew.HasSignup;
        happening.HasPayment = happeningNew.HasPayment;
        happening.PaymentDetails = happeningNew.PaymentDetails;
        happening.Location = happeningNew.Location;
        happening.Latitude = happeningNew.Latitude;
        happening.Longitude = happeningNew.Longitude;

        if (happeningNew.StartDateTime.HasValue && happeningNew.EndDateTime.HasValue)
        {
            String startTimeStr = dtmStartTime.BuildBuiltIn<String>();
            String[] startTime = startTimeStr.Split('|');
            happening.StartDateTime = new DateTime(happeningNew.StartDateTime.Value.Year, happeningNew.StartDateTime.Value.Month, happeningNew.StartDateTime.Value.Day,
                                                   Convert.ToInt32(startTime[0]), Convert.ToInt32(startTime[1]), 0);

            String endTimeStr = dtmEndTime.BuildBuiltIn<String>();
            String[] endTime = endTimeStr.Split('|');
            happening.EndDateTime = new DateTime(happeningNew.StartDateTime.Value.Year, happeningNew.StartDateTime.Value.Month, happeningNew.StartDateTime.Value.Day,
                                                 Convert.ToInt32(endTime[0]), Convert.ToInt32(endTime[1]), 0);

            if (happeningNew.EndDateTime.Value <= happeningNew.StartDateTime.Value)
            {
                ChoiceDialog.Instance.Error("Fecha inválida","La fecha y hora de finalización debe ser mayor que la fecha y hora de inicio.");
                return;
            }
        }

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        happeningService.UpdateHappening(new RegisterHappeningRequest(new RegisterPostRequest(post, null, null, strImages), happening));
    }

    public void ApplyHappening(bool updated)
    {
        if (!updated)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
