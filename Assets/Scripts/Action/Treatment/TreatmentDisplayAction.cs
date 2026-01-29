using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Core.Tools;

using Sirenix.OdinInspector;


public class TreatmentDisplayAction : MonoBehaviour
{
    [Space]
    [Title("Details")]
    [SerializeField]
    Text txtTitle = null;

    [Space]
    [Title("Images")]
    [SerializeField]
    ListScroller lstImage = null;

    //[Title("Values")]
    //[SerializeField]
    //ValueList vllProjectDescriptionType = null;
    [Title("Event")]
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

    public void Clear()
    {
        
    }

    // Display

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

        lstImage.Clear();

        for (int i = 0; i < images.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(1, true);
            scrollerValue.SetSprite(0, images[i]);
            lstImage.ApplyAddValue(scrollerValue);
        }

        PageManager.Instance.ChangePage(pagDetail);

        onDisplayed.Invoke(new long[2] {treatmentFull.PostId, treatmentFull.Id});
    }
}