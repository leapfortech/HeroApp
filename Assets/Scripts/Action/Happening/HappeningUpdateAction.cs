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

    HappeningService happeningService = null;

    long postId = -1, happeningId = -1;
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

    public void SetIds(long[] ids)
    {
        postId = ids[0];
        happeningId = ids[1];
    }

    public void Populate()
    {
        HappeningFull happeningFull = StateManager.Instance.GetHappeningFullById(happeningId);

        post = new Post(happeningFull);
        dtmPost.PopulateClass<Post>(post);

        happening = new Happening(happeningFull);
        dtmHappening.PopulateClass<Happening>(happening);
        String startTimeStr = happening.StartDateTime.Value.ToString("HH|mm", CultureInfo.InvariantCulture);
        dtmStartTime.PopulateBuiltIn<String>(startTimeStr);

        String endTimeStr = happening.EndDateTime.Value.ToString("HH|mm", CultureInfo.InvariantCulture);
        dtmEndTime.PopulateBuiltIn<String>(endTimeStr);

        List<Sprite> images = StateManager.Instance.GetHappeningImagesById(happeningId);
        dtmImagesVLL.PopulateBuiltInList<Sprite>(images);
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
        
        happening.Update(dtmHappening.BuildClass<Happening>());

        if (happening.StartDateTime.HasValue && happening.EndDateTime.HasValue)
        {
            String[] startTime = dtmStartTime.BuildBuiltIn<String>().Split('|');
            happening.StartDateTime = new DateTime(happening.StartDateTime.Value.Year, happening.StartDateTime.Value.Month, happening.StartDateTime.Value.Day,
                                                   Convert.ToInt32(startTime[0]), Convert.ToInt32(startTime[1]), 0);

            String[] endTime = dtmEndTime.BuildBuiltIn<String>().Split('|');
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
