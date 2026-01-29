using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Core.Tools;

using Sirenix.OdinInspector;


public class HappeningDisplayAction : MonoBehaviour
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

    HappeningService happeningService;

    long postId = -1, happeningId = -1;

    private void Awake()
    {
        happeningService = GetComponent<HappeningService>();
    }

    public void Clear()
    {
        
    }

    // Display

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

        lstImage.Clear();

        for (int i = 0; i < images.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(1, true);
            scrollerValue.SetSprite(0, images[i]);
            lstImage.ApplyAddValue(scrollerValue);
        }

        PageManager.Instance.ChangePage(pagDetail);

        onDisplayed.Invoke(new long[2] {happeningFull.PostId, happeningFull.Id});
    }
}