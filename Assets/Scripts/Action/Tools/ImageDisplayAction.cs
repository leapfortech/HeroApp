using System.Collections.Generic;
using UnityEngine;

using Leap.Graphics.Tools;
using Leap.UI.Elements;

using Sirenix.OdinInspector;

public class ImageDisplayAction : MonoBehaviour
{
    [Space, Title("Display")]
    [SerializeField]
    Image imgDisplay = null;

    [SerializeField]
    GameObject gobEmpty = null;

    [SerializeField]
    ListScroller lstImage = null;


    private List<Sprite> images = new List<Sprite>();

    public void Clear()
    {
        for (int i = 0; i < images.Count; i++)
            images[i].Destroy();
        images.Clear();
        lstImage.Clear();
    }



    public void Display(List<Sprite> imgs)
    {
        Clear();

        if (imgs != null)
            images.AddRange(imgs);

        bool hasImages = images.Count > 0;

        gobEmpty.gameObject.SetActive(!hasImages);

        if (!hasImages)
        {
            imgDisplay.Sprite = null;
            return;
        }

        for (int i = 0; i < images.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(lstImage.ListItem, true);
            scrollerValue.SetSprite(0, images[i]);
            scrollerValue.SetActive(1, false);
            lstImage.ApplyAddValue(scrollerValue);
        }

        SelectImage(0);
    }

    public void SelectImage(int idx)
    {
        imgDisplay.Sprite = images[idx];

        for (int i = 0; i < images.Count; i++)
            lstImage[i].SetActive(1, idx == i);

        lstImage.RefreshVisibleValues();
    }
}