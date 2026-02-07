using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Core.Tools;

using Sirenix.OdinInspector;


public class TreatmentDisplayAction : MonoBehaviour
{
    [Serializable]
    public class ImagesEvent : UnityEvent<List<Sprite>> { }
    [Space]
    [Title("Details")]
    [SerializeField]
    Text txtTitle = null;

    [Title("Event")]
    [SerializeField]
    ImagesEvent onImagesDisplay = null;
    [SerializeField]
    UnityLongsEvent onDisplayed = null;

    [Title("Page")]
    [SerializeField]
    Page pagDetail;

    TreatmentService treatmentService;

    long postId = -1, treatmentId = -1;

    private void Awake()
    {
        treatmentService = GetComponent<TreatmentService>();
    }

    public void Display(long postId)
    {
        this.postId = postId;

        TreatmentFull treatmentFull = StateManager.Instance.GetTreatmentFullByPostId(postId);
        if (treatmentFull != null)
        {
            treatmentId = treatmentFull.Id;
            Display(treatmentFull);
            return;
        }

        ScreenDialog.Instance.Display();
        treatmentService.GetFullByPostId(postId);
    }

    public void ApplyFull(TreatmentFull treatmentFull)
    {
        treatmentId = treatmentFull.Id;
        StateManager.Instance.AddTreatmentFull(treatmentFull);
        StateManager.Instance.AddTreatmentImages(treatmentFull.Id, treatmentFull.Images);
        Display(treatmentFull);
    }

    private void Display(TreatmentFull treatmentFull)
    {       
        if (treatmentFull == null)
            return;

        txtTitle.TextValue = treatmentFull.Title;

        List<Sprite> images = StateManager.Instance.GetTreatmentImagesById(treatmentId);

        onImagesDisplay.Invoke(images);
        onDisplayed.Invoke(new long[2] {treatmentFull.PostId, treatmentFull.Id});
        
        PageManager.Instance.ChangePage(pagDetail);
    }
}