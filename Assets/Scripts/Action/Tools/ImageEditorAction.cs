using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.Data.Collections;
using Leap.Graphics.Tools;

using Sirenix.OdinInspector;

public class ImageEditorAction : MonoBehaviour
{
    [Title("Data")]
    [SerializeField]
    ValueList vllImages = null;

    [Space]
    [Title("Images")]
    [SerializeField]
    int maxCount = 4;
    [SerializeField]
    String spriteName = "Tale";
    [SerializeField]
    ListScroller lstImage = null;
    [SerializeField]
    Text txtEmpty;

    [Title("Action")]
    [SerializeField]
    Button btnAdd = null;

    public void Clear()
    {
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
            txtEmpty.gameObject.SetActive(false);
        else
            txtEmpty.gameObject.SetActive(true);

        if (vllImages.RecordCount < maxCount)
            btnAdd.gameObject.SetActive(true);
        else
            btnAdd.gameObject.SetActive(false);
    }

    public void AddImage(Texture2D image)
    {
        vllImages.AddRecord(image.CreateSprite($"{spriteName}_{vllImages.RecordCount + 1}"));
        RefreshImages();
    }

    public void RemoveImage(int idx)
    {
        vllImages.RemoveRecord(idx);
        RefreshImages();
    }
}
