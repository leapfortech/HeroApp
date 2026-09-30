using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;
using Leap.Core.Tools;

using Sirenix.OdinInspector;

public class TreatmentUpdateAction : MonoBehaviour
{
    [Serializable]
    class TreatmentFeedEvent : UnityEvent<TreatmentFeed> { }

    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmPost = null;
    [SerializeField]
    DataMapper dtmTreatment = null;
    [SerializeField]
    DataMapper dtmDiseaseVLL = null;
    [SerializeField]
    DataMapper dtmImagesVLL = null;

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    [Title("Events")]
    [SerializeField]
    UnityLongsEvent OnPopulated = null;
    [SerializeField]
    TreatmentFeedEvent onTreatmentChanged = null;

    TreatmentService treatmentService = null;
    TreatmentFull treatmentFull = null;

    private void Awake()
    {
        treatmentService = GetComponent<TreatmentService>();
    }

    private void Start()
    {
        btnUpdate?.AddAction(DoUpdate);
    }

    public void Clear()
    {
        dtmPost.ClearElements();
        dtmTreatment.ClearElements();
        dtmImagesVLL.ClearElements();
    }

    public void ApplyFull(TreatmentFull treatmentFull)
    {
        Clear();

        this.treatmentFull = treatmentFull;

        dtmPost.PopulateClass<PostFull>(treatmentFull);
        dtmTreatment.PopulateClass<TreatmentFull>(treatmentFull);

        long[] diseaseIds = new long[treatmentFull.DiseaseFulls.Count];
        for (int i = 0; i < treatmentFull.DiseaseFulls.Count; i++)
            diseaseIds[i] = treatmentFull.DiseaseFulls[i].DiseaseTypeId;
        OnPopulated?.Invoke(diseaseIds);

        dtmImagesVLL.PopulateBuiltInList<Sprite>(treatmentFull.ImageSprites);
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        treatmentFull.Update(dtmTreatment.BuildClass<TreatmentFull>());

        treatmentFull.DiseaseFulls = dtmDiseaseVLL.BuildClassList<DiseaseFull>();

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        treatmentFull.ImageCount = images.Count;
        treatmentFull.TitleSprite = images.Count == 0 ? null : images[0];

        treatmentFull.Images = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            treatmentFull.Images[i] = images[i].ToStrBase64(ImageType.JPG);

        treatmentService.UpdateTreatment(treatmentFull);
    }

    public void ApplyUpdate(bool updated)
    {
        if (!updated)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        onTreatmentChanged.Invoke(new TreatmentFeed());

        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
