using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Core.Tools;

using Sirenix.OdinInspector;


public class TaleDisplayDetailsAction : MonoBehaviour
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

    TaleService taleService;

    long postId = -1, taleId = -1;

    private void Awake()
    {
        taleService = GetComponent<TaleService>();
    }

    public void Clear()
    {
        
    }

    // Display

    public void GetFull(long postId)
    {
        this.postId = postId;

        TaleFull taleFull = StateManager.Instance.GetTaleFullByPostId(postId);
        if (taleFull != null)
        {
            taleId = taleFull.Id;
            DisplayDetail(taleFull);
            return;
        }

        ScreenDialog.Instance.Display();
        taleService.GetFullByPostId(postId);
    }

    public void ApplyFull(TaleFull taleFull)
    {
        taleId = taleFull.Id;
        StateManager.Instance.AddTaleFull(taleFull);
        StateManager.Instance.AddTaleImages(taleFull.Id, taleFull.Images);
        DisplayDetail(taleFull);
    }

    public void DisplayDetail(TaleFull taleFull)
    {       
        if (taleFull == null)
            return;

        // Fields
        txtTitle.TextValue = taleFull.Title;

        // Images
        List<Sprite> images = StateManager.Instance.GetTaleImagesById(taleId);

        lstImage.Clear();

        for (int i = 0; i < images.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(1, true);
            scrollerValue.SetSprite(0, images[i]);
            lstImage.ApplyAddValue(scrollerValue);
        }

        PageManager.Instance.ChangePage(pagDetail);

        onDisplayed.Invoke(new long[2] {taleFull.PostId, taleFull.Id});
    }
}