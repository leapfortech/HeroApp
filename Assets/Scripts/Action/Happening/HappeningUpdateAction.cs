using System;
using System.Collections.Generic;
using System.Globalization;
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

    [Title("Event")]
    [SerializeField]
    PostSpriteEvent onPostChanged = null;

    HappeningService happeningService = null;

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

    public void ApplyFull(HappeningFull happeningFull)
    {
        Clear();

        PostHelper.post = new Post(happeningFull);
        dtmPost.PopulateClass<Post>(PostHelper.post);

        happening = new Happening(happeningFull);
        dtmHappening.PopulateClass<Happening>(happening);
        String startTimeStr = happening.StartDateTime.Value.ToString("HH|mm", CultureInfo.InvariantCulture);
        dtmStartTime.PopulateBuiltIn<String>(startTimeStr);

        String endTimeStr = happening.EndDateTime.Value.ToString("HH|mm", CultureInfo.InvariantCulture);
        dtmEndTime.PopulateBuiltIn<String>(endTimeStr);

        dtmImagesVLL.PopulateBuiltInList<Sprite>(happeningFull.ImageSprites);
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        PostHelper.post.Update(dtmPost.BuildClass<Post>());

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

        happeningService.UpdateHappening(new RegisterHappeningRequest(PostHelper.post, strImages, happening));
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
