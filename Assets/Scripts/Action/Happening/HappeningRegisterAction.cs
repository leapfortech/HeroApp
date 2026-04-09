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
    Button btnRegister = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

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
        dtmPost.ClearElements();
        dtmHappening.ClearElements();
        dtmStartTime.ClearElements();
        dtmEndTime.ClearElements();
        dtmImagesVLL.ClearElements();
    }

    private void Register()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        Post post = dtmPost.BuildClass<Post>();
        post.AppUserId = StateManager.Instance.AppUser.Id;

        //RM REVIEW
        post.CountryId = StateManager.Instance.Identity.BirthCountryId;
        post.StateId = StateManager.Instance.Identity.BirthStateId;

        Happening happening = dtmHappening.BuildClass<Happening>();

        if (happening.StartDateTime.HasValue && happening.EndDateTime.HasValue)
        {
            String startTimeStr = dtmStartTime.BuildBuiltIn<String>();
            String[] startTime = startTimeStr.Split('|');
            happening.StartDateTime = new DateTime(happening.StartDateTime.Value.Year, happening.StartDateTime.Value.Month, happening.StartDateTime.Value.Day,
                                                   Convert.ToInt32(startTime[0]), Convert.ToInt32(startTime[1]), 0);

            String endTimeStr = dtmEndTime.BuildBuiltIn<String>();
            String[] endTime = endTimeStr.Split('|');
            happening.EndDateTime = new DateTime(happening.StartDateTime.Value.Year, happening.StartDateTime.Value.Month, happening.StartDateTime.Value.Day,
                                                   Convert.ToInt32(endTime[0]), Convert.ToInt32(endTime[1]), 0);

            if (happening.EndDateTime.Value <= happening.StartDateTime.Value)
            {
                ChoiceDialog.Instance.Error("Fecha inválida","La fecha y hora de finalización debe ser mayor que la fecha y hora de inicio.");
                return;
            }
        }

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        happeningService.Register(new RegisterHappeningRequest(new RegisterPostRequest(post, null, null, strImages), happening));
    }

    public void ApplyHappening(long happeningId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
