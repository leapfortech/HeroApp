using System;
using UnityEngine;
using UnityEngine.Events;

using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.UI.Dialog.Gallery;

using Sirenix.OdinInspector;

public class DocVisionAction : MonoBehaviour
{
    [Serializable]
    public class DocEvent : UnityEvent<Texture2D> { }

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
    Page nextPage = null;

    [Space]
    [SerializeField]
    private DocEvent onDocTaken = null;
    Page pagBack = null;

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

    public void SetPagBack(Page pagBack)
    {
        this.pagBack = pagBack;
    }

    public void ChangeBackPage()
    {
        PageManager.Instance.ChangePage(pagBack);
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
        onDocTaken.Invoke(photo);

        PageManager.Instance.ChangePage(nextPage);
    }
}
