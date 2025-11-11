using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.Core.Debug;
using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Vision.Tools;
using Leap.Identity.Vision;
using Leap.Data.Collections;

using Sirenix.OdinInspector;


public class DpiBackVisionAction : MonoBehaviour
{
    [Serializable]
    public class UnityDpiBackEvent : UnityEvent<DpiBack> { }

    [Title("Camera")]
    [SerializeField]
    WebCamera webCamera = null;
    [SerializeField]
    int maxRetries = 4;
    [Title("Value")]
    [SerializeField]
    ValueList vllMaritalStatus = null;

    [Title("Action")]
    [SerializeField]
    Button btnVision = null;

    [Title("Messages")]
    [SerializeField, TextArea(2, 5)]
    String msgNoDpi = null;
    [SerializeField, TextArea(2, 5)]
    String msgBadPhoto = null;
    [SerializeField, TextArea(2, 5)]
    String msgTooClose = null;
    [SerializeField, TextArea(2, 5)]
    String msgBadDetection = null;
    [SerializeField, TextArea(2, 5)]
    String msgTooFar = null;

    [Title("Result")]
    [SerializeField]
    Image[] photos = null;
    [Space]
    [SerializeField]
    DataMapper dtmDpiBack = null;
    [Space]
    [SerializeField]
    UnityDpiBackEvent onDpiBack = null;
    [Space]
    [SerializeField]
    Page nextPage = null;

    [Title("Error")]
    [SerializeField]
    String errorTitle = "";
    [SerializeField, TextArea(3, 5)]
    String errorMessage = "";
    [SerializeField]
    Sprite errorSprite = null;
    [SerializeField]
    String errorBtnOK = "OK";
    [SerializeField]
    String errorBtnKO = "Cancel";

    IdentityVisionService identityVisionService;
    readonly String[] monthNames = { "ENE", "FEB", "MAR", "ABR", "MAY", "JUN", "JUL", "AGO", "SEP", "OCT", "NOV", "DIC" };
    Dictionary<String, int> months;

    Texture2D photo = null;
    public int Count { get; set; } = 0;

    private void Awake()
    {
        identityVisionService = GetComponent<IdentityVisionService>();
    }

    private void Start()
    {
        btnVision?.AddAction(Do);
    }

    // Take

    public void Do()
    {
        ScreenDialog.Instance.Display(3);
        Invoke(nameof(Take), 0.2f);
    }

    private void Take()
    {
        photo = webCamera.TakePause();
        VisionRequest visionRequet = new VisionRequest(photo.ToStrBase64(ImageType.JPG));

        if (DebugManager.Instance.DebugEnabled)
        {
            DebugManager.Instance.DebugClear();
            Debug.Log("DpiBack64 : " + visionRequet.Image.Length);
        }

        identityVisionService.DpiBackVision(visionRequet);
    }


    public void OnVisionError()
    {
        ApplyDpiBack(null);
    }

    public void ApplyDpiBack(VisionDpiBackResponse visionDpiBackResponse)
    {
        ScreenDialog.Instance.Hide();

        int res = -1;

        if (visionDpiBackResponse != null)
            res = visionDpiBackResponse.Code;

        if (res != 0)
        {
            Count++;
            if (Count == maxRetries)
            {
                Count = 0;
                if (nextPage != null)
                    ChoiceDialog.Instance.ErrorH(errorTitle, errorMessage, errorSprite, () => webCamera.Resume(), () => PageManager.Instance.ChangePage(nextPage), errorBtnOK, errorBtnKO);
                else
                    ChoiceDialog.Instance.ErrorH(errorTitle, errorMessage, errorSprite, () => webCamera.Resume(), null);
                return;
            }
        }

        if (res == 0x01020103 || res == 0x01040101)
        {
            ChoiceDialog.Instance.ErrorH(errorTitle, msgNoDpi, errorSprite, WebCamRestart);
            return;
        }
        if (res == 0x01040201)
        {
            ChoiceDialog.Instance.ErrorH(errorTitle, msgBadPhoto, errorSprite, WebCamRestart);
            return;
        }
        if (res == 0x01020203)
        {
            ChoiceDialog.Instance.ErrorH(errorTitle, msgTooClose, errorSprite, WebCamRestart);
            return;
        }
        if (res == 0x01020202)
        {
            ChoiceDialog.Instance.ErrorH(errorTitle, msgTooFar, errorSprite, WebCamRestart);
            return;
        }
        if (res != 0)
        {
            ChoiceDialog.Instance.ErrorH(errorTitle, msgBadDetection, errorSprite, WebCamRestart);
            return;
        }

        Count = 0;
        webCamera.Stop();

        // Populate
        Sprite sprite = photo.CreateSprite(true);
        for (int i = 0; i < photos.Length; i++)
        {
            photos[i].Sprite = sprite;
            photos[i].name = "DpiBackCamera" + i;
        }

        int maritalStatusId = -1;

        if (visionDpiBackResponse.Result.MaritalStatus != null)
            maritalStatusId = vllMaritalStatus.FindRecordId("Code", visionDpiBackResponse.Result.MaritalStatus[0].ToString());

        DpiBack dpiBack = new DpiBack(visionDpiBackResponse, maritalStatusId);

        dtmDpiBack?.PopulateClass(dpiBack);

        onDpiBack?.Invoke(dpiBack);

        // Change Page
        if (nextPage != null)
            PageManager.Instance.ChangePage(nextPage);
    }

    public void DisplayError(String errorMessage)
    {
        ChoiceDialog.Instance.ErrorH(errorMessage, WebCamRestart);
    }

    private void WebCamRestart()
    {
        webCamera.Resume();
    }

    public void ClearElements()
    {
        dtmDpiBack.ClearElements();
    }
}
