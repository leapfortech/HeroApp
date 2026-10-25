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
    [SerializeField]
    GameObject gobEmpty = null;
    [SerializeField]
    Text txtMax = null;

    [Space, Title("Images")]
    [SerializeField]
    int maxCount = 4;
    [SerializeField]
    String spriteName = "None";
    [SerializeField]
    ListScroller lstImage = null;

    [Title("Data")]
    [SerializeField]
    ValueList vllImages = null;

    [Title("Action")]
    [SerializeField]
    Button btnAddFirst = null;
    [SerializeField]
    Button btnAdd = null;

    private int currentIdx = 0;

    public void RefreshImages()
    {
        lstImage.Clear();

        for (int i = 0; i < vllImages.RecordCount; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(lstImage.ListItem, true);
            scrollerValue.SetSprite(0, vllImages.GetRecordCellSprite(i, "Image"));
            scrollerValue.SetActive(1, false);
            lstImage.ApplyAddValue(scrollerValue);
        }

        bool hasImages = vllImages.RecordCount > 0;
        
        gobEmpty.gameObject.SetActive(!hasImages);
        btnAddFirst.gameObject.SetActive(!hasImages);

        imgDisplay.gameObject.SetActive(hasImages);

        btnAdd.gameObject.SetActive(vllImages.RecordCount < maxCount);

        txtMax.TextValue = $"Solo puedes agregar hasta {maxCount} imágenes.";
        txtMax.gameObject.SetActive(vllImages.RecordCount == maxCount);

        if (vllImages.RecordCount > 0)
            SelectImage(vllImages.RecordCount - 1);
    }

    public void SelectImage(int idx)
    {
        currentIdx = idx;

        imgDisplay.Sprite = vllImages.GetRecordCellSprite(currentIdx, "Image");

        for (int i = 0; i < vllImages.RecordCount; i++)
            lstImage[i].SetActive(1, idx == i);

        lstImage.RefreshVisibleValues();
    }

    public void AddImage(Texture2D image)
    {
        if (image == null)
            return;

        Sprite newSprite = image.CreateSprite($"Edt_{spriteName}_{vllImages.RecordCount + 1}");

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
