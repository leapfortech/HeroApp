using System;

using UnityEngine;

using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Core.Debug;
using Leap.Vision.Tools;
using Leap.Identity.Vision;

using Sirenix.OdinInspector;


public class PortraitVisionAction : MonoBehaviour
{
    [Title("Camera")]
    [SerializeField]
    WebCamera webCamera = null;

    [Title("Action")]
    [SerializeField]
    Button btnVision = null;

    [Title("Thresholds")]
    [SerializeField]
    float minScore = 0.6f;

    [Space]
    [SerializeField]
    int minSize = 380;
    [SerializeField]
    int maxSize = 520;

    [SerializeField]
    int minX = 330;
    [SerializeField]
    int maxX = 390;
    [SerializeField]
    int minY = 440;
    [SerializeField]
    int maxY = 500;

    [Space]
    [SerializeField]
    float maxAngle = 30f;  // 8f
    [SerializeField]
    float maxBlur = 0.3f;

    [Title("Messages")]
    [SerializeField, TextArea(2, 5)]
    String msgNoFace = null;

    [SerializeField, TextArea(2, 5)]
    String msgMinSize = null;
    [SerializeField, TextArea(2, 5)]
    String msgMaxSize = null;

    [SerializeField, TextArea(2, 5)]
    String msgMinX = null;
    [SerializeField, TextArea(2, 5)]
    String msgMaxX = null;
    [SerializeField, TextArea(2, 5)]
    String msgMinY = null;
    [SerializeField, TextArea(2, 5)]
    String msgMaxY = null;

    [SerializeField, TextArea(2, 5)]
    String msgBadPhoto = null;
    [SerializeField, TextArea(2, 5)]
    String msgBlurPhoto = null;
    [SerializeField, TextArea(2, 5)]
    String msgRotation = null;
    [SerializeField, TextArea(2, 5)]
    String msgOneFace = null;

    [Header("Result")]
    [SerializeField]
    Image[] imgPhotos = null;

    [Space]
#pragma warning disable 414
    [SerializeField]
    bool useCamera = true;
#pragma warning restore 414
    [SerializeField]
    Page nextPage = null;
    [SerializeField]
    Page nextPageResult = null;

#pragma warning disable 414
    [SerializeField]
    Sprite dummy = null;
#pragma warning restore 414

    Texture2D portrait = null;

    IdentityVisionService identityVisionService;

    Color white = new Color32(255, 255, 255, 255);
    Color grey = new Color32(200, 200, 200, 255);

    private void Awake()
    {
        for (int i = 0; i < imgPhotos.Length; i++)
        {
            imgPhotos[i].Sprite = null;
            imgPhotos[i].Color = grey;
        }

        identityVisionService = GetComponent<IdentityVisionService>();
    }

    private void Start()
    {
        btnVision?.AddAction(Do);
    }

    public void GoToPortraitPage()
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        PageManager.Instance.ChangePage(nextPage);
#else
        if (useCamera)
            PageManager.Instance.ChangePage(nextPage);
        else
        {
            for (int i = 0; i < imgPhotos.Length; i++)
            {
                imgPhotos[i].Sprite = dummy;
                imgPhotos[i].Color = imgPhotos[i].Sprite != null ? white : grey;
            }
            PageManager.Instance.ChangePage(nextPageResult);
        }
#endif
    }

    // Take

    public void Do()
    {
        ScreenDialog.Instance.Display(2);
        Invoke(nameof(Take), 0.2f);
    }

    private void Take()
    {
        portrait = webCamera.TakePause();

        VisionRequest visionRequet = new VisionRequest(portrait.ToStrBase64(ImageType.JPG), false);

        if (DebugManager.Instance.DebugEnabled)
        {
            DebugManager.Instance.DebugClear();
            Debug.Log("Portrait64 : " + visionRequet.Image.Length);
        }

        identityVisionService.DetectFacesVision(visionRequet);
    }

    // Vision

    public void PhotoVision(VisionFeaturesResponse visionFeaturesResponse)
    {
        ScreenDialog.Instance.Hide();

        if (visionFeaturesResponse == null)
        {
            ChoiceDialog.Instance.Error("No hay respuesta del servidor.", WebCamRestart);
            return;
        }

        if (visionFeaturesResponse.Result == null)
        {
            ChoiceDialog.Instance.Error(msgNoFace, WebCamRestart);
            return;
        }

        if (visionFeaturesResponse.Result.Features.Count == 0)
        {
            ChoiceDialog.Instance.Error(msgNoFace, WebCamRestart);
            return;
        }

        VisionFeatureResult[] features = visionFeaturesResponse.Result.Features.ToArray();

        if (features.Length > 1)
        {
            ChoiceDialog.Instance.Error(msgOneFace, WebCamRestart);
            return;
        }

        if (features[0].Scores[4] > maxBlur)
        {
            ChoiceDialog.Instance.Error(msgBlurPhoto, WebCamRestart);
            return;
        }

        if (Math.Abs(features[0].Scores[1]) > maxAngle || Math.Abs(features[0].Scores[2]) > maxAngle || Math.Abs(features[0].Scores[3]) > maxAngle)
        {
            ChoiceDialog.Instance.Error(msgRotation, WebCamRestart);
            return;
        }

        int leftIdx = features[0].Points.Count - 1;
        int rightIdx = features[0].Points.Count - 2;

        double size = features[0].Points[leftIdx].x - features[0].Points[rightIdx].x;
        //Debug.Log("Eye size : " + size);
        if (size < minSize)
        {
            ChoiceDialog.Instance.Error(msgMinSize, WebCamRestart);
            return;
        }
        if (size > maxSize)
        {
            ChoiceDialog.Instance.Error(msgMaxSize, WebCamRestart);
            return;
        }

        size = webCamera.VideoTexture.width - webCamera.TextureCropper.Rect.y - webCamera.TextureCropper.Rect.height + features[0].Points[0].y + (features[0].Points[rightIdx].x + features[0].Points[leftIdx].y) / 2f;
        //Debug.Log("Eye center Y : " + size);
        if (size < minY)
        {
            ChoiceDialog.Instance.Error(msgMinY, WebCamRestart);
            return;
        }
        if (size > maxY)
        {
            ChoiceDialog.Instance.Error(msgMaxY, WebCamRestart);
            return;
        }

        size = features[0].Points[0].x + (features[0].Points[rightIdx].x + features[0].Points[leftIdx].x) / 2f;  // photoHolder.Rect.x +
        //Debug.Log("Eye center X : " + size);
        if (size < minX)
        {
            ChoiceDialog.Instance.Error(msgMinX, WebCamRestart);
            return;
        }
        if (size > maxX)
        {
            ChoiceDialog.Instance.Error(msgMaxX, WebCamRestart);
            return;
        }

        if (features[0].Scores[0] < minScore)
        {
            ChoiceDialog.Instance.Error(msgBadPhoto, WebCamRestart);
            return;
        }

        webCamera.Stop();
        ApplyPhotos();
        PageManager.Instance.ChangePage(nextPageResult);
    }

    public void ApplyPhotos()
    {
        Sprite sprite = portrait.CreateSprite(true);
        for (int i = 0; i < imgPhotos.Length; i++)
            imgPhotos[i].Sprite = sprite;
    }

    private void WebCamRestart()
    {
        webCamera.Resume();
    }
}
