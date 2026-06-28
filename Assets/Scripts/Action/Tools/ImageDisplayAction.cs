using System.Collections.Generic;
using UnityEngine;

using Leap.Graphics.Tools;
using Leap.UI.Elements;
using MPUIKIT;

using Sirenix.OdinInspector;

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

    public void Clear()
    {
        testImages = null;
        for (int i = 0; i < images.Count; i++)
            images[i].Destroy();
        images.Clear();
        lstImage.Clear();
        ClearIndicators();
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
        imgDisplay.Sprite = images[idx];

        UpdateIndicator(idx);
    }

    public void UpdateIndicator(int idx)
    {
        for (int i = 0; i < indicators.Length; i++)
        {
            MPImage indicatorImage = indicators[i].GetComponent<MPImage>();

            if (indicatorImage != null)
                indicatorImage.color = (i == idx) ? colorOn : colorOff;
        }
    }

    private void CreateIndicators(int count)
    {
        ClearIndicators();

        indicators = new GameObject[count];
        for (int i = 0; i < count; i++)
            indicators[i] = Instantiate(indicatorPrefab, indicatorParent);
    }

    private void ClearIndicators()
    {
        foreach (Transform child in indicatorParent)
            Destroy(child.gameObject);
    }
}