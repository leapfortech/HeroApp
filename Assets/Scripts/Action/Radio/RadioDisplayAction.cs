using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Core.Tools;

using Sirenix.OdinInspector;


public class RadioDisplayAction : MonoBehaviour
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

    RadioService radioService;

    long postId = -1, radioId = -1;

    private void Awake()
    {
        radioService = GetComponent<RadioService>();
    }

    public void Clear()
    {
        
    }

    // Display

    public void Display(long postId)
    {
        this.postId = postId;

        RadioFull radioFull = StateManager.Instance.GetRadioFullByPostId(postId);
        if (radioFull != null)
        {
            radioId = radioFull.Id;
            Display(radioFull);
            return;
        }

        ScreenDialog.Instance.Display();
        radioService.GetFullByPostId(postId);
    }

    public void ApplyFull(RadioFull radioFull)
    {
        radioId = radioFull.Id;
        StateManager.Instance.AddRadioFull(radioFull);
        StateManager.Instance.AddRadioImages(radioFull.Id, radioFull.Images);
        Display(radioFull);
    }

    private void Display(RadioFull radioFull)
    {       
        if (radioFull == null)
            return;

        txtTitle.TextValue = radioFull.Title;

        List<Sprite> images = StateManager.Instance.GetRadioImagesById(radioId);

        lstImage.Clear();

        for (int i = 0; i < images.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(1, true);
            scrollerValue.SetSprite(0, images[i]);
            lstImage.ApplyAddValue(scrollerValue);
        }

        PageManager.Instance.ChangePage(pagDetail);

        onDisplayed.Invoke(new long[2] {radioFull.PostId, radioFull.Id});
    }
}