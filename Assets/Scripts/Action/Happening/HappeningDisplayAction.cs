using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Core.Tools;

using Sirenix.OdinInspector;


public class HappeningDisplayAction : MonoBehaviour
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

    HappeningService happeningService;

    long postId = -1, happeningId = -1;

    private void Awake()
    {
        happeningService = GetComponent<HappeningService>();
    }

    public void Display(long postId)
    {
        this.postId = postId;

        HappeningFull happeningFull = StateManager.Instance.GetHappeningFullByPostId(postId);
        if (happeningFull != null)
        {
            happeningId = happeningFull.Id;
            Display(happeningFull);
            return;
        }

        ScreenDialog.Instance.Display();
        happeningService.GetFullByPostId(postId);
    }

    public void ApplyFull(HappeningFull happeningFull)
    {
        happeningId = happeningFull.Id;
        StateManager.Instance.AddHappeningFull(happeningFull);
        StateManager.Instance.AddHappeningImages(happeningFull.Id, happeningFull.Images);
        Display(happeningFull);
    }

    private void Display(HappeningFull happeningFull)
    {       
        if (happeningFull == null)
            return;

        txtTitle.TextValue = happeningFull.Title;

        List<Sprite> images = StateManager.Instance.GetHappeningImagesById(happeningId);

        onImagesDisplay.Invoke(images);
        onDisplayed.Invoke(new long[2] {happeningFull.PostId, happeningFull.Id});

        PageManager.Instance.ChangePage(pagDetail);
    }
}