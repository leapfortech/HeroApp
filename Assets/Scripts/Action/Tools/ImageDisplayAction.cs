using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;

using Sirenix.OdinInspector;
using MPUIKIT;


public class ImageDisplayAction : MonoBehaviour
{
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

    private GameObject[] indicators;


    public void Clear()
    {
        lstImage.Clear();
        foreach (Transform child in indicatorParent)
            Destroy(child.gameObject);
    }


    public void Display(List<Sprite> images)
    {       
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

        UpdateIndicator(0);
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