using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if !UNITY_EDITOR && UNITY_ANDROID
using UnityEngine.Android;
#endif

using Leap.Core.Tools;
using Leap.Core.Security;
using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Collections;
using Leap.Data.Web;
using Leap.UI.Extensions;

using Sirenix.OdinInspector;

public class LoginAction : MonoBehaviour
{
    [Title("Version")]
    [SerializeField]
    Text txtStartVersion = null;

    [SerializeField]
    Text txtLoginVersion = null;

    [SerializeField]
    Text txtSupportVersion = null;

    [Title("Fields")]
    [SerializeField]
    ToggleGroup tggMethod = null;

    [SerializeField]
    GameObject pnlPhone = null;

    [SerializeField]
    ComboAdapter cmbPhonePrefix = null;

    [SerializeField]
    InputField ifdPhone = null;

    [SerializeField]
    InputField ifdEmail = null;

    [SerializeField]
    InputField ifdPassword = null;

    [SerializeField]
    Toggle chkRemember = null;

    [Header("Data")]
    [SerializeField]
    ValueList vllCountry = null;

    [Title("Authenticate")]
    [SerializeField]
    Button btnAndroidAuth;

    [SerializeField]
    Button btnAppleAuth;

    [SerializeField]
    Button btnRegister;

    [SerializeField]
    bool moveBtnRegister = false;

    [Title("Action")]
    [SerializeField]
    Button btnLogin = null;

    [Title("Pages")]
    [SerializeField]
    Page loginPage = null;

    [SerializeField]
    Page homePage = null;

    [SerializeField]
    Page obdPage = null;

#pragma warning disable 0414
    [Title("Biometrics")]
    [SerializeField]
    String txtAuthAndroid = "Ingresar con tu biometría";

    [SerializeField]
    String txtRegAndroid = "Activar tu biometría";

    [SerializeField]
    String txtAuthApple = "Iniciar sesión con Apple";

    [SerializeField]
    String txtRegApple = "Activar Apple Sign In";

    [Space]
    [SerializeField]
    UnityEvent onAuthenticate = null;

    [SerializeField]
    UnityStringsEvent onRegister = null;
#pragma warning restore 0414

    [Title("Messages")]
    [SerializeField, TextArea(2, 4)]
    String accessDeniedError = "Acceso Denegado.";

    [Space]
    [SerializeField, LabelWidth(130)]
    String resetTitle = "Reiniciar";
    [SerializeField, LabelWidth(130)]
    String resetMsg = "¿Estás seguro de reiniciar tu información?";

    [Space]
    [SerializeField, LabelWidth(130)]
    String resetDoneTitle = "Reinicio satisfactorio";
    [SerializeField, LabelWidth(130)]
    String resetDoneMsg = "Tu información fue reiniciada.";

    [Space]
    [SerializeField, LabelWidth(130)]
    String logoutTitle = "Cerrar sesión";
    [SerializeField, LabelWidth(130)]
    String logoutMsg = "¿Estás seguro de cerrar sesión?";

    [Space]
    [SerializeField, TextArea(2, 6)]
    String resendMailLinkMsg = "Se mandó de nuevo el enlace de confimación a tu correo:\r\n{0}\r\n" +
                               "Si no lo encuentras, busca en tu carpeta de Spams o envíalo de nuevo.\r\nConfirma tu correo y prueba de nuevo.";

    readonly String[] envVersion = { " Dev", " QA", "" };

    AccessService accessService = null;
    WebSysUserService webSysUserService = null;
    WebSysTokenService webSysTokenService = null;
    AppUserService appUserService = null;

    //ElementValue[] elementValues = null;
    LoginAppResponse loginResponse = null;

    RectTransform registerTrf;
#if !UNITY_EDITOR
    Vector3 btnRegisterPos;
#endif
    Vector3 btnAndroidPos;

    private void Awake()
    {
        accessService = GetComponent<AccessService>();
        appUserService = GetComponent<AppUserService>();
        webSysUserService = GetComponent<WebSysUserService>();
        webSysTokenService = GetComponent<WebSysTokenService>();

        //elementValues = new ElementValue[2];
        //elementValues[0] = ifdEmail;
        //elementValues[1] = ifdPassword;

        registerTrf = btnRegister.GetComponent<RectTransform>();
#if !UNITY_EDITOR
        btnRegisterPos = registerTrf.anchoredPosition;
#endif
        btnAndroidPos = btnAndroidAuth.GetComponent<RectTransform>().anchoredPosition;
    }

    public void DisplayVersion()
    {
        txtStartVersion.TextValue = txtSupportVersion.TextValue = txtLoginVersion.TextValue = "v " + Application.version + envVersion[WebManager.Instance.EnvironmentId];
    }

    public void DisplayMethod()
    {
        pnlPhone.SetActive(tggMethod.Value == "P");
        ifdEmail.gameObject.SetActive(tggMethod.Value != "P");
    }

    private void Start()
    {
        // RM REVIEW
        if (!PlayerPrefs.HasKey("Email"))
            PlayerPrefs.SetString("Email", "");

        btnLogin?.AddAction(DoLogin);

#if !UNITY_EDITOR && UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            Permission.RequestUserPermission(Permission.Camera);
#endif
    }

    public void Clear()
    {
        //for (int i = 0; i < elementValues.Length; i++)
        //    elementValues[i].Clear();

        ifdEmail.Clear();
        ifdPassword.Clear();
        cmbPhonePrefix.Clear();
        ifdPhone.Clear();
    }

    public void Display()
    {
        String email = PlayerPrefs.GetString("Email", "");
        int phoneCountryId = PlayerPrefs.GetInt("PhoneCountryId", -1);
        String phone = PlayerPrefs.GetString("Phone", "");

        if (!String.IsNullOrWhiteSpace(email))
        {
            ifdEmail.Text = email;
            ifdEmail.Revalidate(true);
            chkRemember.Checked = true;
            tggMethod.Value = "E";
        }
        else if (phoneCountryId != -1 && !String.IsNullOrWhiteSpace(phone))
        {
            cmbPhonePrefix.Select(Convert.ToInt32(phoneCountryId));
            ifdPhone.Text = phone;
            ifdPhone.Revalidate(true);
            
            chkRemember.Checked = true;
            tggMethod.Value = "P";
        }
        else
        {
            chkRemember.Checked = false;
        }


        DisplayBiometrics();
    }

    // Biometrics
    public void DisplayBiometrics()
    {
#if !UNITY_EDITOR
        if (NativeAuthManager.Instance.IsSupported())
        {
#if UNITY_ANDROID
            if (moveBtnRegister)
                registerTrf.anchoredPosition = btnRegisterPos;
            btnAndroidAuth.gameObject.SetActive(true);
            btnAppleAuth.gameObject.SetActive(false);

            if (NativeAuthManager.Instance.IsRegistered(null))
                btnAndroidAuth.Title = txtAuthAndroid;
            else
                btnAndroidAuth.Title = txtRegAndroid;
#elif UNITY_IOS
            if (moveBtnRegister)
                registerTrf.anchoredPosition = btnRegisterPos;
            btnAndroidAuth.gameObject.SetActive(false);
            btnAppleAuth.gameObject.SetActive(true);

            if (NativeAuthManager.Instance.IsRegistered(null))
                btnAppleAuth.Title = txtAuthApple;
            else
                btnAppleAuth.Title = txtRegApple;
#endif
        }
        else
        {
#endif
        if (moveBtnRegister)
            registerTrf.anchoredPosition = btnAndroidPos;

        btnAndroidAuth.gameObject.SetActive(false);
        btnAppleAuth.gameObject.SetActive(false);


#if !UNITY_EDITOR
        }
#endif
        btnAndroidAuth.SetStyle();
        btnAppleAuth.SetStyle();
    }

    public void DoBiometrics()
    {
#if !UNITY_EDITOR
        if (NativeAuthManager.Instance.IsRegistered(null))
        {
            onAuthenticate.Invoke();
        }
        else
        {
#if UNITY_ANDROID
            ChoiceDialog.Instance.Info("Biometría", "¿Quieres activar tu biometría?", Register, (UnityAction)null, "Sí", "No");
#elif UNITY_IOS
            ChoiceDialog.Instance.Info("Biometría", "¿Quieres activar el Apple Sign In?", Register, (UnityAction)null, "Sí", "No");
#endif
        }
#endif
    }

    public void Authenticate()
    {
#if !UNITY_EDITOR
        if (NativeAuthManager.Instance.IsRegistered(null))
        {
            onAuthenticate.Invoke();
        }
#endif
    }

    private void Register()
    {
        // RM REVIEW
        if (!ElementHelper.Validate(ifdEmail) && !ElementHelper.Validate(ifdPassword))
            return;

        onRegister.Invoke(new String[] { ifdEmail.Text, ifdPassword.Text });
    }

    // Login

    private void DoLogin()  // Manual Login
    {
        String email = null;

        if (tggMethod.Value == "P")
        {
            if (!ElementHelper.Validate(cmbPhonePrefix.Combo) && !ElementHelper.Validate(ifdPhone) && !ElementHelper.Validate(ifdPassword))
                return;

            email = "hm." + vllCountry.FindRecordCellString(cmbPhonePrefix.GetSelectedRecord().Id, "PhonePrefix").Replace("+", "")
                    + ifdPhone.Text.Replace("-", "")
                    + "@heroesmigrantes.com";
        }
        else
        {
            if (!ElementHelper.Validate(ifdEmail) && !ElementHelper.Validate(ifdPassword))
                return;

            email = ifdEmail.Text;
        }

        Login(email, ifdPassword.Text);
    }

    public void Login(String[] credentials)  // Biometric Login
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        Login(credentials[0], credentials[1]);
#elif UNITY_IOS
        LoginApple();
#endif
    }

    private void Login(String eMail, String password)
    {
        ScreenDialog.Instance.Display();
        PageManager.Instance.ResetTimer();

        FirebaseManager.Instance.Login(eMail, password, OnLoginDone, null);
    }

#if !UNITY_EDITOR && UNITY_IOS
    private void LoginApple()
    {
        ScreenDialog.Instance.Display();
        PageManager.Instance.ResetTimer();

        FirebaseManager.Instance.Login(NativeAuthManager.Instance.AppleCredential, NativeAuthManager.Instance.AppleRawNonce, OnLoginDone, null);
    }
#endif

    private void OnLoginDone(String eMail)
    {
        if (chkRemember.Checked)
        {
            if (tggMethod.Value == "P")
            {
                PlayerPrefs.SetInt("PhoneCountryId", (int)cmbPhonePrefix.GetSelectedRecord().Id);
                PlayerPrefs.SetString("Phone", ifdPhone.Text);
                PlayerPrefs.DeleteKey("Email");
            }
            else
            {
                PlayerPrefs.SetString("Email", eMail);
                PlayerPrefs.DeleteKey("PhoneCountryId");
                PlayerPrefs.DeleteKey("Phone");
            }
        }
        else
        {
            PlayerPrefs.DeleteKey("Email");
            PlayerPrefs.DeleteKey("PhoneCountryId");
            PlayerPrefs.DeleteKey("Phone");
        }

        PlayerPrefs.Save();

        accessService.LoginApp(eMail, Application.version);
    }

    public void ResendMailLink()
    {
        if (!ElementHelper.Validate(ifdEmail) && !ElementHelper.Validate(ifdPassword))
            return;

        ScreenDialog.Instance.Display();

        FirebaseManager.Instance.Login(ifdEmail.Text, ifdPassword.Text, OnLoginMailLinkDone, null, false);
    }

    private void OnLoginMailLinkDone(String eMail)
    {
        webSysUserService.SendMailLink(eMail);
    }

    public void ApplyMailLink()
    {
        ChoiceDialog.Instance.Info("Login", String.Format(resendMailLinkMsg, ifdEmail.Text));
    }

    // SetCustomer
    public void SetAppUser(LoginAppResponse response)
    {
        loginResponse = response;
        StateManager.Instance.AppUser = response.AppUser;
        WebManager.Instance.WebSysUser = response.WebSysUser;
    }

    // SystemToken
    public void AddSystemToken()
    {
        webSysTokenService.Add(new WebSysToken(-1, WebManager.Instance.WebSysUser.Id, NotificationManager.Instance.Token ?? "TOKEN_NULL", 1));
    }

    // Change Page
    public void ChangePage()
    {
        if (loginResponse.Message != null)
        {
            String[] message = loginResponse.Message.Split('|');
            if (message[0] == "1")
                ChoiceDialog.Instance.Info(message[1], message[2], loginResponse.Granted == 1 ? ChangePageLink : GoToLink);
            else if(message[0] == "2")
                ChoiceDialog.Instance.Warning(message[1], message[2], loginResponse.Granted == 1 ? ChangePageLink : GoToLink);
            else
                ChoiceDialog.Instance.Error(message[1], message[2], loginResponse.Granted == 1 ? ChangePageLink : GoToLink);
        }
        else if (loginResponse.Granted == 0)
            ChoiceDialog.Instance.Error("Login", accessDeniedError, loginResponse.Link != null ? GoToLink : (UnityAction)null);
        else
            ChangePageGranted();
    }

    private void GoToLink()
    {
        if (loginResponse.Link == null)
            return;

        String[] links = loginResponse.Link.Split('|');

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

    private void ChangePageLink()
    {
        GoToLink();
        ChangePageGranted();
    }

    private void ChangePageGranted()
    {
        Clear();
        GetLoginAppInfo();
    }

    public void ChangeToStartPage()
    {
        long obdStatus = StateManager.Instance.GetOption(0);

        if (obdStatus == 1)
            PageManager.Instance.ChangePage(obdPage);
        else
            ChangeToHomePage();
    }

    public void ChangeToHomePage()
    {
        Clear();

        PageManager.Instance.ChangePage(homePage);
    }

    // LoginAppInfo
    public void GetLoginAppInfo()
    {
        accessService.GetLoginAppInfo();
    }

    public void ApplyLoginAppInfo(LoginAppInfo loginAppInfo)
    {
        StateManager.Instance.ReferredCount = loginAppInfo.ReferredCount;
        StateManager.Instance.Identity = loginAppInfo.Identity;
        StateManager.Instance.Address = loginAppInfo.Address;
        StateManager.Instance.Portrait = loginAppInfo.Portrait != null ? loginAppInfo.Portrait.CreateSprite("Portrait") : null;
        StateManager.Instance.InterestLocality = loginAppInfo.InterestLocality;
        StateManager.Instance.CurrentLocality = loginAppInfo.CurrentLocality;
        StateManager.Instance.Card = loginAppInfo.Card;  // loginData.Card.Id == 0 ? null : loginData.Card;
    }

    // Remote Login
    public void RemoteLogin(String[] message)
    {
        ChoiceDialog.Instance.Info(message[0], message[1], ChangeToLoginPage);
    }

    public void ChangeToLoginPage()
    {
        PageManager.Instance.ChangePage(loginPage);
        StateManager.Instance.ClearAll();
    }

    // Reset
    public void ResetAppUser()
    {
        ChoiceDialog.Instance.Warning(resetTitle, resetMsg, () => { SandwichMenu.Instance.Close(); ScreenDialog.Instance.Display(); appUserService.UpdateStatus(0); }, null, "Sí", "No");
    }

    public void ResetAppUserDone()
    {
        ScreenDialog.Instance.Hide();
        ChoiceDialog.Instance.Info(resetDoneTitle, resetDoneMsg, ChangeToLoginPage);
    }

    // Logout
    public void Logout()
    {
        ChoiceDialog.Instance.Info(logoutTitle, logoutMsg, () => { SandwichMenu.Instance.Close(); ChangeToLoginPage(); }, null, "Sí", "No");
    }
}
