using UnityEngine;
using UnityEngine.Events;

using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Dialog.Gallery;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;
using System;

public class PhotoVisionAction : MonoBehaviour
{
    [Serializable]
    public class PhotoEvent : UnityEvent<Texture2D> { }

    [Title("Camera")]
    [SerializeField]
    WebCamera webCamera = null;

    [Title("Gallery")]
    [SerializeField]
    Vector2Int gallerySize = new Vector2Int(794, 560);

    [Title("Action")]
    [SerializeField]
    Button btnVision = null;

    [Space]
    [SerializeField]
    private PhotoEvent onPhotoTaken = null;

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
        Invoke(nameof(Take), 0.2f);
    }

    private void Take()
    {
        ApplyPhoto(webCamera.TakePause());
    }

    // Apply

    private void ApplyPhoto(Texture2D photo)
    {
        ScreenDialog.Instance.Display();
        onPhotoTaken.Invoke(photo);
    }
}
