using UnityEngine;

using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.UI.Dialog.Gallery;

using Sirenix.OdinInspector;
using System;

public class PhotoVisionAction : MonoBehaviour
{
    [Title("Param")]
    [SerializeField]
    String spriteName = null;

    [Title("Camera")]
    [SerializeField]
    WebCamera webCamera = null;

    [Title("Gallery")]
    [SerializeField]
    Vector2Int gallerySize = new Vector2Int(794, 560);

    [Title("Action")]
    [SerializeField]
    Button btnVision = null;

    [Title("Result")]
    [SerializeField]
    Element[] photos = null;

    [Space]
    [SerializeField]
    Page nextPage = null;

    Texture2D photo = null;

    private void Start()
    {
        btnVision?.AddAction(Do);
    }

    // Gallery

    public void SearchPhoto()
    {
        GalleryDialog.Instance.Search(gallerySize, false, ApplyPhoto); // new Vector2(0.588f, 1.7f));  // new Vector2(0.625f, 1.6f));
    }

    // Clear

    public void ClearPhoto()
    {
        ApplyPhoto(null);
    }

    // Camera

    public void Do()
    {
        ScreenDialog.Instance.Display();
        Invoke(nameof(Take), 0.2f);
    }

    private void Take()
    {
        ApplyPhoto(webCamera.TakePause());

        PageManager.Instance.ChangePage(nextPage);
    }

    // Apply

    private void ApplyPhoto(Texture2D photo)
    {
        this.photo?.Destroy();
        this.photo = photo;
        this.photo.name = spriteName;

        CreateSprites();
    }

    private void CreateSprites()
    {
        Sprite sprite = photo?.CreateSprite(true);
        for (int i = 0; i < photos.Length; i++)
        {
            if (photos[i] is Image @image)
                @image.Sprite = sprite;
            if (photos[i] is Button @button)
                @button.Sprite = sprite;
        }
    }
}
