using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;
using Leap.Core.Tools;

using Sirenix.OdinInspector;

public class TreatmentUpdateAction : MonoBehaviour
{

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

    [Title("Event")]
    [SerializeField]
    UnityLongsEvent OnPopulated = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    TreatmentService treatmentService = null;

    Post post = null;
    Treatment treatment = null;

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
        post = new Post(treatmentFull);
        dtmPost.PopulateClass<Post>(post);

        treatment = new Treatment(treatmentFull);
        dtmTreatment.PopulateClass<Treatment>(treatment);

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

        post.Update(dtmPost.BuildClass<Post>());

        treatment.Update(dtmTreatment.BuildClass<Treatment>());

        List<Disease> diseases = dtmDiseaseVLL.BuildClassList<Disease>();

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        treatmentService.UpdateTreatment(new RegisterTreatmentRequest(post, strImages, treatment, diseases));
    }

    public void ApplyUpdate(bool updated)
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
