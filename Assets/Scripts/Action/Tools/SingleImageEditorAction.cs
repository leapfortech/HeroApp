using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.Data.Collections;
using Leap.Graphics.Tools;

using Sirenix.OdinInspector;

public class SingleImageEditorAction : MonoBehaviour
{
    [Space, Title("Display")]
    [SerializeField]
    Image imgDisplay = null;

    [Space, Title("Images")]
    [SerializeField]
    String spriteName = "None";

    [Title("Data")]
    [SerializeField]
    ValueList vllImages = null;


    public void Clear()
    {
        vllImages.ClearRecords();
        imgDisplay.ClearValue();
    }

    public void AddImage(Texture2D image)
    {
        Clear();

        Sprite newSprite = image.CreateSprite($"Edt_{spriteName}");

        vllImages.AddRecord(newSprite);
        imgDisplay.Sprite = newSprite;
    }
}
