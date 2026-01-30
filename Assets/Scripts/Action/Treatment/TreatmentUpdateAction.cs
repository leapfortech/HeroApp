using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;
using Leap.Data.Collections;
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

    long postId = -1, treatmentId = -1;
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

    public void SetIds(long[] ids)
    {
        postId = ids[0];
        treatmentId = ids[1];
    }

    public void Populate()
    {
        TreatmentFull treatmentFull = StateManager.Instance.GetTreatmentFullById(treatmentId);

        post = new Post(treatmentFull);
        dtmPost.PopulateClass<Post>(post);

        treatment = new Treatment(treatmentFull);
        dtmTreatment.PopulateClass<Treatment>(treatment);

        long[] diseaseIds = new long[treatmentFull.DiseaseFulls.Count];

        for (int i = 0; i < treatmentFull.DiseaseFulls.Count; i++)
            diseaseIds[i] = treatmentFull.DiseaseFulls[i].DiseaseTypeId;

        OnPopulated?.Invoke(diseaseIds);

        List<Sprite> images = StateManager.Instance.GetTreatmentImagesById(treatmentId);
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

        Treatment treatmentNew = dtmTreatment.BuildClass<Treatment>();
        treatment.Ingredients = treatmentNew.Ingredients;
        treatment.Preparation = treatmentNew.Preparation;
        treatment.Usage = treatmentNew.Usage;
        treatment.Annotation = treatmentNew.Annotation;

        List<Disease> diseasesNew = dtmDiseaseVLL.BuildClassList<Disease>();

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        treatmentService.UpdateTreatment(new RegisterTreatmentRequest(new RegisterPostRequest(post, null, null, strImages), treatment, diseasesNew));
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
