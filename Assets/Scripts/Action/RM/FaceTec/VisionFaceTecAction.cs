using System;
using UnityEngine;

using Leap.Core.Tools;
using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Vision.FaceTec;

using Sirenix.OdinInspector;

public class VisionFaceTecAction : SingletonBehaviour<VisionFaceTecAction>
{
    [Space]
    [SerializeField]
    VisionFaceTecController visionFaceTecController = null;

    [Title("Start")]
    [SerializeField]
    Page pagStart = null;

#pragma warning disable 414
    [SerializeField]
    Page pagFaceTec = null;
#pragma warning restore 414

    [Title("Result")]
    [SerializeField]
    Text txtSuccess = null;
    [SerializeField]
    Text txtFail = null;

    [Space]
    [SerializeField]
    Button btnNext = null;
    [SerializeField]
    Button btnRetry = null;

    [Space]
    [SerializeField]
    Image[] imgPhotos = null;

#pragma warning disable 414
    [Space]
    [SerializeField]
    Sprite dummy = null;
#pragma warning restore 414

    [Title("Page")]
    [SerializeField]
    Page pagResult = null;

    Color white = new Color32(255, 255, 255, 255);
    Color grey = new Color32(200, 200, 200, 255);

    private void Awake()
    {
        for (int i = 0; i < imgPhotos.Length; i++)
        {
            imgPhotos[i].Sprite = null;
            imgPhotos[i].Color = grey;
        }
    }

    public void GoToPortraitPage()
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        ChangeFaceTecPage();
#else
        for (int i = 0; i < imgPhotos.Length; i++)
        {
            imgPhotos[i].Sprite = dummy;
            imgPhotos[i].Color = imgPhotos[i].Sprite != null ? white : grey;
        }
        txtSuccess.gameObject.SetActive(true);
        btnNext.gameObject.SetActive(true);
        PageManager.Instance.ChangePage(pagResult);
#endif
    }

    public void GoBack()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        PageManager.Instance.ChangePage(pagFaceTec);
        ChangeFaceTecPage();
#elif !UNITY_EDITOR && UNITY_IOS
        Invoke(nameof(ChangeFaceTecPage), 1f);
#else
        PageManager.Instance.ChangePage(pagStart);
#endif
    }

    public void ChangeFaceTecPage()
    {
        PageManager.Instance.ChangePage(pagFaceTec);
    }

    // FaceScan
    public void StartFaceScan()
    {
        ScreenDialog.Instance.Display();
        visionFaceTecController.SetEvents(OnFaceScanImage, OnFaceScanDone, OnFaceScanRetry, OnRestart);
        visionFaceTecController.GetLiveness3dKeys();
    }

    public void OnFaceScanImage(String faceImage)
    {
        for (int i = 0; i < imgPhotos.Length; i++)
        {
            imgPhotos[i].Sprite = faceImage.CreateSprite("Liveness3d", new Rect(0, 40, 180, 240));
            imgPhotos[i].Color = imgPhotos[i].Sprite != null ? white : grey;
        }
    }

    public void OnFaceScanDone()
    {
        txtSuccess.gameObject.SetActive(true);
        txtFail.gameObject.SetActive(false);
        if (btnNext != null)
            btnNext.gameObject.SetActive(true);
        if (btnRetry != null)
            btnRetry.gameObject.SetActive(false);

        PageManager.Instance.ChangePage(pagResult);
    }

    public void OnFaceScanRetry()
    {
        txtSuccess.gameObject.SetActive(false);
        txtFail.gameObject.SetActive(true);
        if (btnNext != null)
            btnNext.gameObject.SetActive(false);
        if (btnRetry != null)
            btnRetry.gameObject.SetActive(true);

        PageManager.Instance.ChangePage(pagResult);
    }

    // Restart
    public void OnRestart()
    {
        PageManager.Instance.ChangePage(pagStart);
    }
}