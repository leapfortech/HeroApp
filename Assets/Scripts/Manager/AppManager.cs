using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.Core.Tools;
using Leap.Core.Debug;
using Leap.Core.Security;
using Leap.Data.Collections;
using Leap.Data.Web;
using Leap.Finance.Payment;
using Leap.UI.Page;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;

public class AppManager : SingletonBehaviour<AppManager>
{
    [Title("ValueLists Filtered")]
    [SerializeField, LabelText(" ")]
    private ValueList[] vllFiltered = null;

    [Title("Events")]
    [SerializeField]
    private UnityEvent onLoadData = null;
    [SerializeField]
    private UnityEvent onStart = null;

    [Title("Message")]
    [SerializeField, TextArea(2, 4)]
    String accessDeniedError = "Acceso Denegado.";

    Dictionary<String, String> dicAppParams = new Dictionary<String, String>();

    StartService startService = null;
    StartResponse startResponse = null;

    private void Awake()
    {
        startService = GetComponent<StartService>();
    }
    void Start()
    {
        if (!EmulatorDetector.IsValid("Error", "No se pudo Iniciar la App.",         // IsEmulator
                                               "No fue posible Iniciar la App.",     // HasFakeAcc
                                               "No se puede Iniciar la App."))       // IsRooted
            return;
    }

    public void StartApp(String publicKey)
    {
        startService.StartApp(new StartRequest(publicKey, Application.version));
    }

    public void ApplyStart(StartResponse response)
    {
        startResponse = response;

        if (response.Message != null)
        {
            String[] message = response.Message.Split('|');
            if (message[0] == "1")
                ChoiceDialog.Instance.Info(message[1], message[2], response.Granted == 1 ? StartLink : GoToLink);
            else if (message[0] == "2")
                ChoiceDialog.Instance.Warning(message[1], message[2], response.Granted == 1 ? StartLink : GoToLink);
            else
                ChoiceDialog.Instance.Error(message[1], message[2], response.Granted == 1 ? StartLink : GoToLink);
        }
        else if (response.Granted == 0)
            ChoiceDialog.Instance.Error("Start", accessDeniedError, response.Link != null ? GoToLink : (UnityAction)null);
        else
            ContinueStart();
    }

    private void GoToLink()
    {
        if (startResponse.Link == null)
            return;

        String[] links = startResponse.Link.Split('|');

#if UNITY_IOS
        if (links.Length > 1)
        {
            if (links[1] == "<None>")
                return;
            Application.OpenURL(links[1]);
        }
        else
        {
            if (links[0] == "<None>")
                return;
            Application.OpenURL(links[0]);
        }
#else
        if (links[0] == "<None>")
            return;
        Application.OpenURL(links[0]);
#endif
    }

    private void StartLink()
    {
        GoToLink();
        ContinueStart();
    }

    private void ContinueStart()
    {
        CertificateManager.Instance.SetCertificate(startResponse.Certificates);
    }

    public void LoadData()
    {
        if (CertificateManager.Instance.Breach)
        {
            CertificateManager.Instance.DisplayBreach();
            return;
        }

        onLoadData.Invoke();
    }

    // Params
    public void SetAppParams(AppParam[] appParams)
    {
        for (int i = 0; i < appParams.Length; i++)
            dicAppParams.Add(appParams[i].Name, appParams[i].Value);

#if UNITY_EDITOR
        if (PageManager.Instance.TimeoutDelay == 0)
#endif
            PageManager.Instance.TimeoutDelay = Convert.ToInt32(dicAppParams["LogoutDelay"]);
    }

    public String GetParamValue(String key)
    {
        return dicAppParams[key];
    }

    // Start
    public void StartApp()
    {
        for (int i = 0; i < vllFiltered.Length; i++)
            vllFiltered[i].RefreshRecords();

        onStart.Invoke();

#if UNITY_EDITOR || !UNITY_WEBGL
        WebCamTexture webCamTexture = new WebCamTexture(1280, 720);
        webCamTexture?.Stop();
#else
        StartCoroutine(AskPermission());
#endif
    }

#if !UNITY_EDITOR && UNITY_WEBGL
    IEnumerator AskPermission()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        //if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        //    Initialize();
    }
#endif

    // Debug

    public void DisplayInfo(String message)
    {
        Debug.Log(message);
    }

    public void DisplayWarning(String message)
    {
        Debug.LogWarning(message);
    }

    public void DisplayError(String message)
    {
        Debug.LogError(message);
    }

    public void DisplayErrorA(String message)
    {
        Debug.LogError("A : " + message);
    }

    public void DisplayErrorB(String message)
    {
        Debug.LogError("B : " + message);
    }

    public void DisplayErrorC(String message)
    {
        Debug.LogError("C : " + message);
    }

    public void DisplayErrorD(String message)
    {
        Debug.LogError("D : " + message);
    }

    public void DisplayTokenK(String message)
    {
        if (DebugManager.Instance.DebugEnabled)
            Debug.Log("TokenK : " + (message.Length < 20 ? message : message[..20]));
    }

    public void DisplayInt(int value)
    {
        Debug.Log(value.ToString());
    }
}
