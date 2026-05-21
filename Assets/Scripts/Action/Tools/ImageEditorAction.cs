using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.Data.Collections;
using Leap.Graphics.Tools;

using Sirenix.OdinInspector;

public class ImageEditorAction : MonoBehaviour
{
    [Space, Title("Display")]
    [SerializeField]
    Image imgDisplay = null;
    [SerializeField]
    Sprite sprEmpty = null;

    [Space, Title("Images")]
    [SerializeField]
    int maxCount = 4;
    [SerializeField]
    String spriteName = "Tale";
    [SerializeField]
    ListScroller lstImage = null;
    //[SerializeField]
    //Text txtEmpty;

    [Title("Data")]
    [SerializeField]
    ValueList vllImages = null;

    [Title("Action")]
    [SerializeField]
    Button btnAdd = null;
    [SerializeField]
    Button btnDelete = null;

    private int currentIdx = 0;

    public void Clear()
    {
        imgDisplay.Clear();
        lstImage.Clear();
        vllImages.ClearRecords();
    }

    public void RefreshImages()
    {
        lstImage.Clear();

        for (int i = 0; i < vllImages.RecordCount; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(1, true);
            scrollerValue.SetSprite(0, vllImages.GetRecordCellSprite(i, "Image"));
            lstImage.ApplyAddValue(scrollerValue);
        }

        if (vllImages.RecordCount > 0)
            btnDelete.gameObject.SetActive(true);
        else
        {
            imgDisplay.Sprite = sprEmpty;
            btnDelete.gameObject.SetActive(false);
        }

        if (vllImages.RecordCount < maxCount)
            btnAdd.gameObject.SetActive(true);
        else
            btnAdd.gameObject.SetActive(false);

        if (vllImages.RecordCount > 0)
            SelectImage(0);
    }

    public void SelectImage(int idx)
    {
        currentIdx = idx;

        imgDisplay.Sprite = vllImages.GetRecordCellSprite(currentIdx, "Image");
    }

    public void AddImage(Texture2D image)
    {
        Sprite newSprite = image.CreateSprite($"{spriteName}_{vllImages.RecordCount + 1}");

        List<Sprite> imgs = new List<Sprite>();
        imgs.Add(newSprite);

        for (int i = 0; i < vllImages.RecordCount; i++)
            imgs.Add(vllImages.GetRecordCellSprite(i, "Image"));

        vllImages.ClearRecords();

        for (int i = 0; i < imgs.Count; i++)
            vllImages.AddRecord(imgs[i]);

        RefreshImages();
    }

    public void RemoveImage()
    {
        vllImages.RemoveRecord(currentIdx);

        RefreshImages();
    }
}
