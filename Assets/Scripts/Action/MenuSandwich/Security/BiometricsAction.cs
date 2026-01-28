using System;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

#if UNITY_IOS
using AppleAuth.Interfaces;
#endif

using Leap.Core.Tools;
//using Leap.Core.Debug;
using Leap.Core.Security;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class BiometricsAction : MonoBehaviour
{
    [Title("Fields")]
    [SerializeField]
    InputField ifdEmail;

    [SerializeField]
    InputField ifdPassword;

    [SerializeField]
    GameObject imgCardBiometrics;

#pragma warning disable 0414
    [Title("Authentication")]
    [SerializeField]
    String authPromptTitle = "Autenticación Requerida";

    [SerializeField]
    String authPromptSubtitle = "";

    [SerializeField]
    String authPromptDescription = "Logueate con tu biometría";

    [Title("Registration")]
    [SerializeField]
    String regPromptTitle = "Autenticación Requerida";

    [SerializeField]
    String regPromptSubtitle = "";

    [SerializeField]
    String regPromptDescription = "Logueate con tu biometría";

    [Title("Security Menu")]
    [SerializeField]
    String regAndroid = "Activar la biometría";

    [SerializeField]
    String unregAndroid = "Desactivar la biometría";

    [SerializeField]
    String regApple = "Activar Apple Sign In";

    [SerializeField]
    String unregApple = "Desactivar Apple Sign In";

    [Title("Activation Messages")]
    [SerializeField]
    String msgTitle = "Biometría";

    [Space]
    [SerializeField]
    String msgChoiceOk = "Sí";

    [SerializeField]
    String msgChoiceKo = "No";

    [Space]
    [SerializeField, TextArea(2, 4)]
    String msgAndroidActivated = "Biometría activada.\r\n\r\nPuedes ingresar con tu biometría.";

    [SerializeField]
    String msgAndroidChoice = "¿Quieres desactivar tu biometría?";

    [Space]
    [SerializeField, TextArea(2, 4)]
    String msgAppleActivated = "Apple Sign In activado.\r\n\r\nPuedes iniciar sesión con Apple.";

    [SerializeField]
    String msgAppleChoice = "¿Quieres desactivar Apple Sign In?";
#pragma warning restore 0414

    [Title("Action")]
    [SerializeField]
    Button btnRegBiometrics;

    [SerializeField]
    Button btnUnregBiometrics;

    [SerializeField]
    Button btnBiometrics;

    [Title("Events")]
    [SerializeField]
    UnityStringsEvent onAuthenticated;

    [SerializeField]
    UnityEvent onRegistered;

    [Title("Page")]
    [SerializeField]
    Page pagSecurity;

    [SerializeField]
    Page pagBiometrics;

    Page pagEndRegister = null;

    String eMail = null;
    String password = null;
    AppUser appUser = null;
    WebSysUser webSysUser = null;
    ElementValue[] elementValues = null;

#if !UNITY_EDITOR && UNITY_IOS
    bool appleRegister = false;
#endif

    private void Awake()
    {
        elementValues = new ElementValue[1];
        elementValues[0] = ifdPassword;

#if UNITY_ANDROID
        btnRegBiometrics.Title = regAndroid;
        btnUnregBiometrics.Title = unregAndroid;
        btnBiometrics.Title = regAndroid;
#elif UNITY_IOS
        btnRegBiometrics.Title = regApple;
        btnUnregBiometrics.Title = unregApple;
        btnBiometrics.Title = regApple;
#endif
    }

    public void Clear()
    {
        for (int i = 0; i < elementValues.Length; i++)
            elementValues[i].Clear();
    }

    // Display

    public void Display()
    {
        if (NativeAuthManager.Instance.IsSupported() && (!NativeAuthManager.Instance.IsRegistered(null) || NativeAuthManager.Instance.IsRegistered(WebManager.Instance.WebSysUser?.Email)))
        {
            imgCardBiometrics.SetActive(true);
            DisplayButtons();
        }
        else
        {
            imgCardBiometrics.SetActive(false);
        }
    }

    private void DisplayButtons()
    {
        bool isRegistered = NativeAuthManager.Instance.IsRegistered(WebManager.Instance.WebSysUser?.Email);
        btnRegBiometrics.gameObject.SetActive(!isRegistered);
        btnUnregBiometrics.gameObject.SetActive(isRegistered);
    }

    public void DisplayEmail()
    {
        ifdEmail.Text = WebManager.Instance.WebSysUser.Email;
    }

    // Authenticate

    public void Authenticate()
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        AuthAndroid();
#elif UNITY_IOS
        AuthApple();
#endif
    }

#if UNITY_EDITOR

#elif UNITY_ANDROID
    private void AuthAndroid()
    {
        NativeAuthManager.Instance.SetTitle(authPromptTitle, authPromptSubtitle, authPromptDescription);
        NativeAuthManager.Instance.AuthenticateCrypto();
    }
#elif UNITY_IOS
    private void AuthApple()
    {
        appleRegister = false;
        NativeAuthManager.Instance.Authenticate();
    }        
#endif

    public void OnAuthenticated(String[] credentials)
    {
        //DebugManager.Instance.DebugInfo("Biometrics : OnAuthenticated");

        if (credentials != null && credentials[0] == null)
        {
            ChoiceDialog.Instance.Error(credentials[1]);
            return;
        }

#if !UNITY_EDITOR && UNITY_IOS
        if (NativeAuthManager.Instance.AppleCredential == null)
        {
            //DebugManager.Instance.DebugInfo("Biometrics : Cancelled");
            ScreenDialog.Instance.Hide();
            return;
        }

        //DebugManager.Instance.DebugInfo("Biometrics : appleRegister = " + appleRegister.ToString());
        if (appleRegister)
        {
            OnAppleAuthenticated();
            return;
        }
#endif
        onAuthenticated.Invoke(credentials);
    }

    // Register

    public void Register()   // From Security Page
    {
        PageManager.Instance.ChangePage(pagBiometrics);
    }

    public void RegisterLogin(String[] credentials)   // From Login Page
    {
        //DebugManager.Instance.DebugInfo("Biometrics : RegisterLogin");

        pagEndRegister = null;
        eMail = credentials[0];
        password = credentials[1];

        Login();
    }

    public void RegisterPage()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        pagEndRegister = pagSecurity;
        eMail = ifdEmail.Text;
        password = ifdPassword.Text;

        Login();
    }

    private void Login()
    {
        //DebugManager.Instance.DebugInfo("Biometrics : Login");

        appUser = StateManager.Instance.AppUser;
        webSysUser = WebManager.Instance.WebSysUser;

        ScreenDialog.Instance.Display();
        FirebaseManager.Instance.Login(eMail, password, OnLoginDone, EndLogin);
    }

    bool registering = false;

    private void OnLoginDone(String credentialEmail)
    {
#if UNITY_EDITOR
        EndLogin();
#elif UNITY_ANDROID
        registering = true;
        NativeAuthManager.Instance.SetTitle(regPromptTitle, regPromptSubtitle, regPromptDescription);
        NativeAuthManager.Instance.RegisterCrypto(eMail, password);
        EndLogin();
#elif UNITY_IOS
        ResetAppUser();
        FirebaseManager.Instance.CheckAppleProvider(eMail, OnAppleProvider, null);
#endif
    }

    private void ResetAppUser()
    {
        StateManager.Instance.AppUser = appUser;
        WebManager.Instance.WebSysUser = webSysUser;
    }

    private void EndLogin()
    {
        ResetAppUser();
        ScreenDialog.Instance.Hide();
    }

#if !UNITY_EDITOR && UNITY_IOS
    private void OnAppleProvider(bool hasAppleProvider)
    {
        //DebugManager.Instance.DebugInfo("Biometrics : OnAppleProvider");
        if (hasAppleProvider)
        {
            //DebugManager.Instance.DebugInfo("Biometrics : hasAppleProvider");
            OnAppleLinked();
            return;
        }

        appleRegister = true;
        //DebugManager.Instance.DebugInfo("Biometrics : Authenticate");
        NativeAuthManager.Instance.Authenticate();
    }

    private void OnAppleAuthenticated()
    {
        //DebugManager.Instance.DebugInfo("Biometrics : OnAppleAuthenticated");

        //DebugManager.Instance.DebugInfo("Biometrics : LinkAppleProvider");
        FirebaseManager.Instance.LinkAppleProvider(NativeAuthManager.Instance.AppleCredential, NativeAuthManager.Instance.AppleRawNonce, OnAppleLinked, null);
    }

    private void OnAppleLinked()
    {
        //DebugManager.Instance.DebugInfo("Biometrics : OnAppleLinked");

        NativeAuthManager.Instance.Register(eMail);

        DisplayButtons();
        ScreenDialog.Instance.Hide();

        onRegistered.Invoke();
        ChoiceDialog.Instance.Info(msgTitle, msgAppleActivated, EndRegister);
    }
#endif

    // Register

    public void RegisterResume()
    {
        if (!registering)
            PageManager.Instance.ChangePage(pagSecurity);
    }

    public void OnRegistered()
    {
        onRegistered.Invoke();
        ChoiceDialog.Instance.Info(msgTitle, msgAndroidActivated, EndRegister);
    }

    public void OnCancelled()
    {
        registering = false;
    }

    private void EndRegister()
    {
        registering = false;
        if (pagEndRegister != null)
            PageManager.Instance.ChangePage(pagEndRegister);
    }

    public void Unregister()
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        ChoiceDialog.Instance.Info(msgTitle, msgAndroidChoice, DoUnregister, (UnityAction)null, msgChoiceOk, msgChoiceKo);
#elif UNITY_IOS
        ChoiceDialog.Instance.Info(msgTitle, msgAppleChoice, CheckAppleProvider, (UnityAction)null, msgChoiceOk, msgChoiceKo);
#endif
    }

#if !UNITY_EDITOR && UNITY_IOS
    private void CheckAppleProvider()
    {
        ScreenDialog.Instance.Display();
        FirebaseManager.Instance.CheckAppleProvider(WebManager.Instance.WebSysUser.Email, DoAppleUnlinked, null);
    }

    private void DoAppleUnlinked(bool hasAppleProvider)
    {
        if (!hasAppleProvider)
        {
            DoUnregister();
            return;
        }

        try
        {
            FirebaseManager.Instance.UnlinkAppleProvider(DoUnregister, null);
        }
        catch
        {
            DoUnregister();
        }
    }
#endif

    public void DoUnregister()
    {
        NativeAuthManager.Instance.Unregister();
        DisplayButtons();
        ScreenDialog.Instance.Hide();
    }

    // Fail

    public void OnFailed(String message)
    {
        if (String.IsNullOrEmpty(message))
            ScreenDialog.Instance.Hide();
        else
            ChoiceDialog.Instance.Error(message);
    }
}
