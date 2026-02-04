using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Core.Tools;

using Sirenix.OdinInspector;
using MPUIKIT;


public class TaleDisplayAction : MonoBehaviour
{
    [Space]
    [Title("Details")]
    [SerializeField]
    Text txtTitle = null;

    [Space]
    [Title("Images")]
    [SerializeField]
    ListScroller lstImage = null;

    [Header("Indicator")]
    [SerializeField]
    private GameObject indicatorPrefab;
    [SerializeField]
    private Transform indicatorParent;
    [SerializeField]
    private Color colorOn = Color.white;
    [SerializeField]
    private Color colorOff = Color.gray;

    [Title("Event")]
    [SerializeField]
    UnityLongsEvent onDisplayed = null;

    [Title("Page")]
    [SerializeField]
    Page pagDetail;

    TaleService taleService;

    private GameObject[] indicators;
    long postId = -1, taleId = -1;

    private void Awake()
    {
        taleService = GetComponent<TaleService>();
    }

    public void Clear()
    {
        lstImage.Clear();
        foreach (Transform child in indicatorParent)
            Destroy(child.gameObject);
    }

    // Display

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

        List<Sprite> images = StateManager.Instance.GetTaleImagesById(taleId);

        lstImage.Clear();

        CreateIndicators(images.Count);

        for (int i = 0; i < images.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(1, true);
            scrollerValue.SetSprite(0, images[i]);
            lstImage.ApplyAddValue(scrollerValue);
        }

        PageManager.Instance.ChangePage(pagDetail);

        UpdateIndicator(0);
        onDisplayed.Invoke(new long[2] {taleFull.PostId, taleFull.Id});
    }

    public void UpdateIndicator(int currentIndex)
    {
        for (int i = 0; i < indicators.Length; i++)
        {
            MPImage indicatorImage = indicators[i].GetComponent<MPImage>();

            if (indicatorImage != null)
                indicatorImage.color = (i == currentIndex) ? colorOn : colorOff;
        }
    }

    private void CreateIndicators(int count)
    {
        foreach (Transform child in indicatorParent)
            Destroy(child.gameObject);

        indicators = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            GameObject indicator = Instantiate(indicatorPrefab, indicatorParent);
            indicators[i] = indicator;
        }
    }
}