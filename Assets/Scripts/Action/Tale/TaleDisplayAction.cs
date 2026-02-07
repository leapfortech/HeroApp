using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Core.Tools;

using Sirenix.OdinInspector;


public class TaleDisplayAction : MonoBehaviour
{
    [Serializable]
    public class ImagesEvent : UnityEvent<List<Sprite>> { }
    [Space]
    [Title("Details")]
    [SerializeField]
    Text txtTitle = null;
    [SerializeField]
    Text txtDescription = null;

    [Title("Event")]
    [SerializeField]
    ImagesEvent onImagesDisplay = null;
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

    public void Display(long postId)
    {
        this.postId = postId;

        TaleFull taleFull = StateManager.Instance.GetTaleFullByPostId(postId);
        if (taleFull != null)
        {
            taleId = taleFull.Id;
            Display(taleFull);
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
        Display(taleFull);
    }

    private void Display(TaleFull taleFull)
    {       
        if (taleFull == null)
            return;

        txtTitle.TextValue = taleFull.Title;
        txtDescription.TextValue = taleFull.Description;

        List<Sprite> images = StateManager.Instance.GetTaleImagesById(taleId);
        
        onImagesDisplay.Invoke(images);
        onDisplayed.Invoke(new long[2] {taleFull.PostId, taleFull.Id});

        PageManager.Instance.ChangePage(pagDetail);
    }
}