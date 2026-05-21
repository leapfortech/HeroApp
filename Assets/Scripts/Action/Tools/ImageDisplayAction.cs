using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;

using Sirenix.OdinInspector;
using MPUIKIT;


public class ImageDisplayAction : MonoBehaviour
{
    [Space, Title("Test Images")]
    [SerializeField]
    List<Sprite> testImages = null;

    [Space, Title("Display")]
    [SerializeField]
    Image imgDisplay = null;
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

    private GameObject[] indicators;
    
    private List<Sprite> images = new List<Sprite>();

    private int currentIdx = 0;

    public void Clear()
    {
        testImages = null;
        images.Clear();
        lstImage.Clear();
        foreach (Transform child in indicatorParent)
            Destroy(child.gameObject);
    }

    public void DisplayTestImages()
    {
        Display(testImages);
    }


    public void Display(List<Sprite> imgs)
    {       
        this.images = imgs;
        
        if (images == null || images.Count == 0)
            return;

        lstImage.Clear();

        CreateIndicators(images.Count);

        for (int i = 0; i < images.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(1, true);
            scrollerValue.SetSprite(0, images[i]);
            lstImage.ApplyAddValue(scrollerValue);
        }

        SelectImage(0);
    }

    public void SelectImage(int idx)
    {
        currentIdx = idx;

        imgDisplay.Sprite = images[idx];

        UpdateIndicator(idx);
    }

    public void UpdateIndicator(int currentIdx)
    {
        for (int i = 0; i < indicators.Length; i++)
        {
            MPImage indicatorImage = indicators[i].GetComponent<MPImage>();

            if (indicatorImage != null)
                indicatorImage.color = (i == currentIdx) ? colorOn : colorOff;
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