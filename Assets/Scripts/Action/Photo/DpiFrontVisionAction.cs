using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.Core.Tools;
using Leap.Core.Debug;
using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Data.Collections;
using Leap.Vision.Tools;
using Leap.Identity.Vision;

using Sirenix.OdinInspector;

public class DpiFrontVisionAction : MonoBehaviour
{
    [Serializable]
    public class UnityDpiFrontEvent : UnityEvent<DpiFront> { }

    [Title("Camera")]
    [SerializeField]
    WebCamera webCamera = null;
    [SerializeField]
    int maxRetries = 4;

    [Title("Value")]
    [SerializeField]
    ValueList vllGender = null;
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
    String msgTooFar = null;
    [SerializeField, TextArea(2, 5)]
    String msgBadDetection = null;
    [SerializeField, TextArea(2, 5)]
    String msgNoPortrait = null;

    [Title("Portrait")]
    [SerializeField]
    bool extractPortrait = false;
    [Space]
    [SerializeField]
    Image[] photoPortraits = null;

    [Title("Result")]
    [SerializeField]
    Image[] dpiPhotos = null;
    [Space]
    [SerializeField]
    DataMapper dtmDpiFront = null;
    [Space]
    [SerializeField]
    UnityDpiFrontEvent onDpiFront = null;
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
    Page identityPage = null;
#pragma warning restore 414

    [Title("Success")]
    [SerializeField]
    String successTitle = "";
    [SerializeField, TextArea(2, 5)]
    String successMessage = "";
    [SerializeField]
    Sprite successSprite = null;
    [SerializeField]
    String successBtnOK = "OK";

    [Title("Error")]
    [SerializeField]
    String errorTitle = "";
    [SerializeField, TextArea(2, 5)]
    String errorMessage = "";
    [SerializeField]
    Sprite errorSprite = null;
    [SerializeField]
    String errorBtnOK = "OK";
    [SerializeField]
    String errorBtnKO = "Cancelar";

    IdentityVisionService identityVisionService;
    readonly String[] monthNames = { "ENE", "FEB", "MAR", "ABR", "MAY", "JUN", "JUL", "AGO", "SEP", "OCT", "NOV", "DIC" };
    Dictionary<String, int> months;

    Texture2D dpiPhoto = null;
    VisionDpiFrontResponse visionDpiFrontResponse = null;
    
    public int Count { get; set; } = 0;

    private void Awake()
    {
        identityVisionService = GetComponent<IdentityVisionService>();
    }

    private void Start()
    {
        btnVision?.AddAction(Do);
    }

    public bool GoToIdentityPage()
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        PageManager.Instance.ChangePage(nextPage);
#else
        if (useCamera)
            PageManager.Instance.ChangePage(nextPage);
        else
            PageManager.Instance.ChangePage(identityPage);
#endif
        return false;
    }

    // Take

    public void Do()
    {
        ScreenDialog.Instance.Display(2);
        Invoke(nameof(Take), 0.2f);
    }

    private void Take()
    {
        dpiPhoto = webCamera.TakePause();
        VisionRequest visionRequet = new VisionRequest(dpiPhoto.ToStrBase64(ImageType.JPG), extractPortrait);

        if (DebugManager.Instance.DebugEnabled)
        {
            DebugManager.Instance.DebugClear();
            Debug.Log("DpiFront64 : " + visionRequet.Image.Length);
        }

        identityVisionService.DpiFrontVision(visionRequet);
    }

    public void ApplyDpiFront(VisionDpiFrontResponse visionDpiFrontResponse)
    {
        int res = visionDpiFrontResponse.Code;

        if (res != 0)
        {
            Count++;
            if (Count == maxRetries)
            {
                Count = 0;
                if (nextPageResult != null)
                    ChoiceDialog.Instance.ErrorH(errorTitle, errorMessage, errorSprite, () => webCamera.Resume(), () => PageManager.Instance.ChangePage(nextPageResult), errorBtnOK, errorBtnKO);
                else
                    ChoiceDialog.Instance.ErrorH(errorTitle, errorMessage, errorSprite, () => webCamera.Resume(), null);
                return;
            }
        }

        if (res == 0x01010103 || res == 0x01040101)
        {
            ChoiceDialog.Instance.ErrorH(errorTitle, msgNoDpi, errorSprite, WebCamRestart);
            return;
        }
        if (res == 0x01040201)
        {
            ChoiceDialog.Instance.ErrorH(errorTitle, msgBadPhoto, errorSprite, WebCamRestart);
            return;
        }
        if (res == 0x01010207)
        {
            ChoiceDialog.Instance.ErrorH(errorTitle, msgTooClose, errorSprite, WebCamRestart);
            return;
        }
        if (res == 0x01010206)
        {
            ChoiceDialog.Instance.ErrorH(errorTitle, msgTooFar, errorSprite, WebCamRestart);
            return;
        }
        if (res == 0x01010401)
        {
            ChoiceDialog.Instance.ErrorH(errorTitle, msgNoPortrait, WebCamRestart);
            return;
        }
        if (res != 0)
        {
            ChoiceDialog.Instance.ErrorH(errorTitle, msgBadDetection, errorSprite, WebCamRestart);
            return;
        }

        this.visionDpiFrontResponse = visionDpiFrontResponse;

        ChangeNextPage();
    }

    private void ChangeNextPage()
    {
        ScreenDialog.Instance.Hide();
        Count = 0;
        webCamera.Stop();

        // Populate
        Sprite sprite = dpiPhoto.CreateSprite(true);
        for (int i = 0; i < dpiPhotos.Length; i++)
        {
            dpiPhotos[i].Sprite = sprite;
            dpiPhotos[i].name = "DpiFrontCamera" + i;
        }
        
        int genderId = -1;
        int maritalStatusId = -1;

        if (visionDpiFrontResponse.Result.Gender != null)
            genderId = vllGender.FindRecordId("Code", visionDpiFrontResponse.Result.Gender[0].ToString());

        if (visionDpiFrontResponse.Result.MaritalStatus != null)
            maritalStatusId = vllMaritalStatus.FindRecordId("Code", visionDpiFrontResponse.Result.MaritalStatus[0].ToString());
        
        DpiFront dpiFront = new DpiFront(this.visionDpiFrontResponse, genderId, maritalStatusId);

        ApplyPhotoPortraits(dpiFront.Portrait);

        dtmDpiFront?.PopulateClass(dpiFront);

        onDpiFront?.Invoke(dpiFront);

        //PageManager.Instance.ChangePage(nextPage);
        if (nextPageResult != null)
            ChoiceDialog.Instance.InfoH(successTitle, successMessage, successSprite, () => PageManager.Instance.ChangePage(nextPageResult), successBtnOK);
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
        dtmDpiFront.ClearElements();
    }

    public void ApplyPhotoPortraits(Texture2D portrait)
    {
        if (photoPortraits.Length == 0)
            return;

        for (int i = 0; i < photoPortraits.Length; i++)
            photoPortraits[i].Sprite = portrait?.CreateSprite(photoPortraits[i].name);
    }
}