using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.UI.Extensions;
using Leap.Data.Web;

public class RegisterAction : MonoBehaviour
{
    [Header("Fields")]
    [SerializeField]
    InputField ifdEmail = null;

    [SerializeField]
    ComboAdapter cmbPhonePrefix = null;

    [SerializeField]
    InputField ifdPhone = null;

    [SerializeField]
    InputField ifdPassword = null;

    [SerializeField]
    InputField ifdConfirm = null;

    [SerializeField]
    InputField ifdReferredCode = null;

    [SerializeField]
    Toggle chkTerms = null;

    [Header("Action")]
    [SerializeField]
    Button btnRegister = null;

    [SerializeField]
    Button btnResendLink = null;

    [Header("Result")]
    [SerializeField]
    Text txtEmail = null;
    [SerializeField]
    Page pagMailLink = null;
    [SerializeField]
    Page pagDone = null;

    [Space]
    [SerializeField, TextArea(2, 5)]
    String verifyError = "Unable to send the activation email. Please try again.";
    [SerializeField, TextArea(2, 5)]
    String passwordError = "The password fields do not match. Please enter them again.";

    AccessService accessService;
    WebSysUserService webSysUserService;
    ElementValue[] elementValues = null;

    private void Awake()
    {
        accessService = GetComponent<AccessService>();
        webSysUserService = GetComponent<WebSysUserService>();
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (elementValues != null)
            return;

        elementValues = new ElementValue[4];
        elementValues[0] = ifdEmail;
        elementValues[1] = ifdPassword;
        elementValues[2] = ifdConfirm;
        elementValues[3] = chkTerms;

        btnRegister?.AddAction(Register);
        btnResendLink?.AddAction(ResendMailLink);
    }

    public void Init()
    {
        Clear();
        if (cmbPhonePrefix.Combo.IsEmpty())
            cmbPhonePrefix.Select(2);
    }

    public void Clear()
    {
        Initialize();
        for (int i = 0; i < elementValues.Length; i++)
            elementValues[i].Clear();
    }

    // Register

    public void Register()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        if (ifdPassword.Text != ifdConfirm.Text)
        {
            ifdPassword.DisplayValidity(false);
            ifdConfirm.DisplayValidity(false);
            ChoiceDialog.Instance.Error(PageManager.Instance.CurrentPage.HeaderTitle, passwordError);
            return;
        }

        ScreenDialog.Instance.Display();

        FirebaseManager.Instance.LoginStartToken(DoRegister, null);
    }

    private void DoRegister(String _)
    {
        accessService.RegisterApp(new RegisterAppRequest(ifdEmail.Text, ifdPassword.Text, cmbPhonePrefix.GetSelectedRecord().Id, ifdPhone.Text, Convert.ToInt64(ifdReferredCode.Text)));
    }

    // Send Mail Link
    public void ResendMailLink()
    {
        ScreenDialog.Instance.Display();

        FirebaseManager.Instance.LoginStartToken(OnResendMailLink, null);
    }

    private void OnResendMailLink(String _)
    {
        SendMailLink();
    }

    public void ApplyRegistered(String registerResponse)  // registerResponse : $"{appUserId}|{isMailVerified}"
    {
        if (registerResponse[^1] == '0')
            SendMailLink();  // First time
        else
            PageManager.Instance.ChangePage(pagDone);
    }

    private void SendMailLink()
    {
        FirebaseManager.Instance.Login(ifdEmail.Text, ifdPassword.Text, OnLoginMailLinkDone, null, false);
    }

    private void OnLoginMailLinkDone(String eMail)
    {
        webSysUserService.SendMailLink(eMail);
    }

    public void ChangePageMailLink()
    {
        txtEmail.TextValue = ifdEmail.Text;

        PageManager.Instance.ChangePage(pagMailLink);
    }

    // Messages

    public void VerifyErrorMessage()
    {
        ChoiceDialog.Instance.Info("Registro", verifyError);
    }
}
