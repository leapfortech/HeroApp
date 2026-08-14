using System;
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
    Button btnAddFirst = null;
    [SerializeField]
    Button btnAdd = null;
    [SerializeField]
    Button btnDelete = null;

    private int currentIdx = 0;

    public void Clear()
    {
        lstImage.Clear();
        for (int i = 0; i < vllImages.RecordCount; i++)
            vllImages.GetRecordCellSprite(i, "Image").Destroy();
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

        bool hasImages = vllImages.RecordCount > 0;
        btnDelete.gameObject.SetActive(hasImages);
        imgDisplay.gameObject.SetActive(hasImages);
        btnAddFirst.gameObject.SetActive(!hasImages);

        btnAdd.gameObject.SetActive(vllImages.RecordCount < maxCount);

        if (vllImages.RecordCount > 0)
            SelectImage(vllImages.RecordCount - 1);
    }

    public void SelectImage(int idx)
    {
        currentIdx = idx;

        imgDisplay.Sprite = vllImages.GetRecordCellSprite(currentIdx, "Image");
    }

    public void AddImage(Texture2D image)
    {
        Sprite newSprite = image.CreateSprite($"{spriteName}_{vllImages.RecordCount + 1}");

        vllImages.AddRecord(newSprite);

        RefreshImages();
    }

    public void RemoveImage()
    {
        vllImages.GetRecordCellSprite(currentIdx, "Image").Destroy();
        vllImages.RemoveRecord(currentIdx);

        RefreshImages();
    }
}
