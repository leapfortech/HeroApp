using System;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_ANDROID
using Leap.Core.Security;
#endif
using Leap.UI.Elements;
using Leap.UI.Dialog;
using Leap.Data.Web;
using Leap.UI.Extensions;
using Leap.Data.Collections;
using Leap.UI.Page;

using Sirenix.OdinInspector;

public class PasswordResetAction : MonoBehaviour
{
    [Serializable]
    public class UnityPasswordChangedEvent : UnityEvent { }

    [Title("Method")]
    [SerializeField]
    ToggleGroup tggMethod = null;
    [SerializeField]
    GameObject pnlPhone = null;
    [SerializeField]
    GameObject pnlEmail = null;

    [Title("Elements")]
    [SerializeField]
    ToggleGroup tggPhoneChannel = null;

    [SerializeField]
    ComboAdapter cmbPhoneCountry = null;

    [SerializeField]
    InputField ifdPhoneNumber = null;

    [SerializeField]
    InputField ifdEmail = null;

    [Title("Code")]
    [SerializeField]
    Text txtResult = null;

    [SerializeField]
    InputField ifdCode = null;

    [SerializeField]
    InputField ifdNewPassword = null;

    [SerializeField]
    InputField ifdConfirmPassword = null;


    [Header("Page")]
    [SerializeField]
    Page pagValidate = null;

    [SerializeField]
    Page pagNewPassword = null;

    [SerializeField]
    Page pagDone = null;

    [Header("Action")]
    [SerializeField]
    Button btnReset = null;

    [SerializeField]
    Button btnResendCode = null;

    [SerializeField]
    Button btnValidateCode = null;

    [SerializeField]
    Button btnUpdatePassword = null;

    [Header("Event")]
    [SerializeField]
    UnityPasswordChangedEvent onPasswordChanged = null;

    [Title("Messages")]
    [SerializeField]
    String notExistError = "The phone number does not exist.";

    [SerializeField]
    String resendTitle = "Verification Code";

    [SerializeField]
    String resendMessage = "The verification code was sent.";

    [Space]
    [SerializeField]
    String notRegisteredError = "The phone number is not registered.";

    [SerializeField]
    String expiredError = "The code has expired. You can send another one.";

    [SerializeField]
    String badCodeError = "The code is invalid.";

    [SerializeField]
    String maxAttemptsError = "Max send attempts reached.";

    AccessService accessService;
    PrecheckService precheckService = null;

    bool isResend = false;
    long webSysUserId = -1;
    private void Start()
    {
        Initialize();

        btnReset?.AddAction(Login);
        btnValidateCode?.AddAction(ValidateCode);
        btnResendCode?.AddAction(ResendCode);
        btnUpdatePassword?.AddAction(UpdatePassword);
    }

    private void Initialize()
    {
        isResend = false;

        if (precheckService != null)
            return;

        accessService = GetComponent<AccessService>();
        precheckService = GetComponent<PrecheckService>();
    }

    public void Clear()
    {
        Initialize();

        ifdEmail.Clear();
        cmbPhoneCountry.Clear();
        ifdPhoneNumber.Clear();
    }

    public void DisplayMethod()
    {
        pnlPhone.SetActive(tggMethod.Value == "P");
        pnlEmail.SetActive(tggMethod.Value != "P");
    }


    // Reset Password
    private void Login()
    {
        Initialize();

        if (tggMethod.Value == "P")
        {
            if (!ElementHelper.Validate(cmbPhoneCountry.Combo) && !ElementHelper.Validate(ifdPhoneNumber))
                return;
        }
        else
        {
            if (!ElementHelper.Validate(ifdEmail))
                return;
        }

        ScreenDialog.Instance.Display();

        FirebaseManager.Instance.LoginStartToken(ResetPassword, null);
    }

    private void ResetPassword(String _)
    {
        String email = null;
        long phoneCountryId = -1;
        String phone = null;

        if (tggMethod.Value == "P")
        {
            phoneCountryId = cmbPhoneCountry.GetSelectedId();
            phone = ifdPhoneNumber.Text;

            email = "hm." + cmbPhoneCountry.GetSelectedRecord().Id.ToString() + "."
                    + ifdPhoneNumber.Text.Replace("-", "")
                    + "@heroesmigrantes.com";
        }
        else
            email = ifdEmail.Text;

        ResetPasswordRequest request = new ResetPasswordRequest(tggMethod.Value == "P" ? 1 : 2,
                                                                tggPhoneChannel.Value == "W" ? 1 : 2,
                                                                phoneCountryId,
                                                                phone,
                                                                email);

        accessService.ResetPassword(request);
    }

    
    public void ApplyResetPasswordSent(long webSysUserId)
    {
        this.webSysUserId = webSysUserId;

        FirebaseManager.Instance.AuthLogOut();
#if !UNITY_EDITOR && UNITY_ANDROID
        if (NativeAuthManager.Instance.IsRegistered(ifdEmail.Text))
            NativeAuthManager.Instance.Unregister();
#endif
        if (tggMethod.Value == "P")
            txtResult.TextValue = "Ingresa el código que recibiste al número de celular <b> "
                               + cmbPhoneCountry.GetSelectedCellString("PhonePrefix")
                               + " "
                               + ifdPhoneNumber.Text
                               + "</b>";
        else
            txtResult.TextValue = "Ingresa el código que recibiste al correo <b>" + ifdEmail.Text + "</b>";

        if (!isResend)
            PageManager.Instance.ChangePage(pagValidate);
        else
            ChoiceDialog.Instance.Info(resendTitle, resendMessage);
    }

    // Validate
    private void ResendCode()
    {
        ScreenDialog.Instance.Display();
        isResend = true;

        Login();
    }

    private void ValidateCode()
    {
        if (!ElementHelper.Validate(ifdCode))
            return;

        ScreenDialog.Instance.Display();

        switch (tggMethod.Value)
        {
            case "P":
                switch (tggPhoneChannel.Value)
                {
                    case "S":
                        FirebaseManager.Instance.LoginStartToken(ValidatePhoneSmsCode, null);
                        break;

                    case "W":
                        FirebaseManager.Instance.LoginStartToken(ValidatePhoneWACode, null);
                        break;
                }
                break;

            case "E":
                FirebaseManager.Instance.LoginStartToken(ValidateEmailCode, null);
                break;
        }
    }

    private void ValidatePhoneSmsCode(String _)
    {
        PhoneCodeRequest phoneCodeRequest = new PhoneCodeRequest(cmbPhoneCountry.GetSelectedId(), ifdPhoneNumber.Text, ifdCode.Text);
        precheckService.ValidatePhoneSmsCode(phoneCodeRequest);
    }

    private void ValidatePhoneWACode(String _)
    {
        PhoneCodeRequest phoneCodeRequest = new PhoneCodeRequest(cmbPhoneCountry.GetSelectedId(), ifdPhoneNumber.Text, ifdCode.Text);
        precheckService.ValidatePhoneWACode(phoneCodeRequest);
    }

    private void ValidateEmailCode(String _)
    {
        EmailCodeRequest emailCodeRequest = new EmailCodeRequest(ifdEmail.Text, ifdCode.Text);
        precheckService.ValidateEmailCode(emailCodeRequest);
    }           

    public void ApplyValidateCode(String result)
    {
        if (result == "NOT_FOUND")
        {
            ChoiceDialog.Instance.Error(notRegisteredError);
            return;
        }

        if (result == "EXPIRED")
        {
            ChoiceDialog.Instance.Error(expiredError);
            return;
        }

        if (result == "BAD_CODE")
        {
            ChoiceDialog.Instance.Error(badCodeError);
            return;
        }

        PageManager.Instance.ChangePage(pagNewPassword);
    }

    // Update Password

    private void UpdatePassword()
    {
        if (!ElementHelper.Validate(ifdNewPassword) && !ElementHelper.Validate(ifdConfirmPassword))
            return;

        if (ifdNewPassword.Text != ifdConfirmPassword.Text)
        {
            ifdNewPassword.DisplayValidity(false);
            ifdConfirmPassword.DisplayValidity(false);
            ChoiceDialog.Instance.Error(PageManager.Instance.CurrentPage.HeaderTitle, "Los campos <b>" + ifdNewPassword.Title + "</b> y <b>" + ifdConfirmPassword.Title + "</b> son diferentes. Por favor, revisa e intenta de nuevo.");
            return;
        }

        ScreenDialog.Instance.Display();

        accessService.UpdatePassword(new UpdatePasswordRequest(webSysUserId, ifdConfirmPassword.Text));
    }

    public void ApplyUpdatePassword()
    {
        Clear();
        onPasswordChanged?.Invoke();
        PageManager.Instance.ChangePage(pagDone);
    }


    public void DisplayErrorMessage(String error)
    {
        if (error.Contains("was not found"))
        {
            ChoiceDialog.Instance.Error(notExistError);
            return;
        }

        if (error.Contains("Max send attempts reached"))
        {
            ChoiceDialog.Instance.Error(maxAttemptsError);
            return;
        }

        ChoiceDialog.Instance.Error(error);
    }

}
