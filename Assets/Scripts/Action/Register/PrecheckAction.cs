using System;

using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Dialog;
using Leap.UI.Extensions;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class PrecheckAction : MonoBehaviour
{
    [Title("Method")]
    [SerializeField]
    ToggleGroup tggMethod = null;
    [SerializeField]
    GameObject pnlPhone = null;
    [SerializeField]
    GameObject pnlEmail = null;

    [Title("Phone")]
    [SerializeField]
    ToggleGroup tggPhoneChannel = null;

    [SerializeField]
    ComboAdapter cmbPhoneCountry = null;

    [SerializeField]
    InputField ifdPhoneNumber = null;

    [Title("Email")]
    [SerializeField]
    InputField ifdEmail = null;

    [Title("Code")]
    [SerializeField]
    Text txtResult = null;

    [SerializeField]
    InputField ifdCode = null;

    [Title("Action")]
    [SerializeField]
    Button btnRegister = null;

    [SerializeField]
    Button btnResendCode = null;

    [SerializeField]
    Button btnValidateCode = null;

    [SerializeField]
    bool isSandwich = false;

    [Title("Messages")]
    [SerializeField]
    String resendTitle = "Verification Code";

    [SerializeField]
    String resendMessage = "The verification code was sent.";

    [SerializeField]
    String notExistError = "The phone number does not exist.";

    [SerializeField]
    String countryError = "The phone number must be an US one.";

    [SerializeField]
    String mobileError = "The phone number must be a mobile one.";

    [Space]
    [SerializeField]
    String notRegisteredError = "The phone number is not registered.";

    [SerializeField]
    String expiredError = "The code has expired. You can send another one.";

    [SerializeField]
    String badCodeError = "The code is invalid.";

    [SerializeField]
    String maxAttemptsError = "Max send attempts reached.";
    

    [Space]
    [Title("Events")]
    [SerializeField]
    UnityEvent onRegistered = null;

    [SerializeField]
    UnityEvent onValidateCode = null;

    PrecheckService precheckService = null;
    bool isResend = false;

    private void Start()
    {
        Initialize();

        btnRegister?.AddAction(Register);
        btnValidateCode?.AddAction(ValidateCode);
        btnResendCode?.AddAction(ResendCode);
    }

    private void Initialize()
    {
        isResend = false;

        if (precheckService != null)
            return;

        precheckService = GetComponent<PrecheckService>();
    }

    public void Clear()
    {
        Initialize();
        cmbPhoneCountry.Clear();
        ifdPhoneNumber.Clear();
        ifdEmail.Clear();
    }

    public void DisplayMethod()
    {
        pnlPhone.SetActive(tggMethod.Value == "P");
        pnlEmail.SetActive(tggMethod.Value != "P");
    }

    public void SelectCountryId(int countryId)
    {
        cmbPhoneCountry.Select(countryId);
    }

    public void SelectCountryId()
    {
        long countryId = WebManager.Instance.WebSysUser.PhoneCountryId;
        cmbPhoneCountry.Select(countryId);
    }

    private void Register()
    {
        Initialize();

        switch (tggMethod.Value)
        {
            case "P":
                switch (tggPhoneChannel.Value)
                {
                    case "S":
                        RegisterPhoneSms();
                        break;

                    case "W":
                        RegisterPhoneWA();
                        break;
                }
                break;

            case "E":
                RegisterEmail();
                break;
        }
    }


    // REGISTER SMS
    private void RegisterPhoneSms()
    {
        if (!ElementHelper.Validate(cmbPhoneCountry.Combo) && !ElementHelper.Validate(ifdPhoneNumber))
            return;

        ScreenDialog.Instance.Display();

        if (isSandwich)
            DoRegisterPhoneSms(null);
        else
            FirebaseManager.Instance.LoginStartToken(DoRegisterPhoneSms, null);
    }

    private void DoRegisterPhoneSms(String _)
    {
        precheckService.RegisterPhoneSms(cmbPhoneCountry.GetSelectedId(), ifdPhoneNumber.Text);
    }

    public void ApplyRegisterPhoneSms(String result)
    {
        if (result == "COUNTRY")
        {
            ChoiceDialog.Instance.Error(countryError);
            return;
        }

        if (result == "MOBILE")
        {
            ChoiceDialog.Instance.Error(mobileError);
            return;
        }

        txtResult.TextValue = cmbPhoneCountry.GetSelectedCellString("PhonePrefix") + " " + ifdPhoneNumber.Text;

        if(!isResend)
            onRegistered.Invoke();
        else
            ChoiceDialog.Instance.Info(resendTitle, resendMessage); 
    }

    // REGISTER WA
    private void RegisterPhoneWA()
    {
        if (!ElementHelper.Validate(cmbPhoneCountry.Combo) && !ElementHelper.Validate(ifdPhoneNumber))
            return;

        ScreenDialog.Instance.Display();

        if (isSandwich)
            DoRegisterPhoneWA(null);
        else
            FirebaseManager.Instance.LoginStartToken(DoRegisterPhoneWA, null);
    }

    private void DoRegisterPhoneWA(String _)
    {
        precheckService.RegisterPhoneWA(cmbPhoneCountry.GetSelectedId(), ifdPhoneNumber.Text);
    }

    public void ApplyRegisterPhoneWA(String result)
    {
        txtResult.TextValue = cmbPhoneCountry.GetSelectedCellString("PhonePrefix") + " " + ifdPhoneNumber.Text;

        if (!isResend)
            onRegistered.Invoke();
        else
            ChoiceDialog.Instance.Info(resendTitle, resendMessage);
    }

    // REGISTER EMAIL
    private void RegisterEmail()
    {
        if (!ElementHelper.Validate(ifdEmail))
            return;

        ScreenDialog.Instance.Display();

        if (isSandwich)
            DoRegisterEmail(null);
        else
            FirebaseManager.Instance.LoginStartToken(DoRegisterEmail, null);
    }

    private void DoRegisterEmail(String _)
    {
        precheckService.RegisterEmail(ifdEmail.Text);
    }

    public void ApplyRegisterEmail(String result)
    {
        txtResult.TextValue = ifdEmail.Text;

        if (!isResend)
            onRegistered.Invoke();
        else
            ChoiceDialog.Instance.Info(resendTitle, resendMessage);
    }

    // VALIDATE
    private void ResendCode()
    {
        ScreenDialog.Instance.Display();
        isResend = true;

        switch (tggMethod.Value)
        {
            case "P":
                switch (tggPhoneChannel.Value)
                {
                    case "S":
                        DoRegisterPhoneSms(null);
                        break;

                    case "W":
                        DoRegisterPhoneWA(null);
                        break;
                }
                break;

            case "E":
                DoRegisterEmail(null);
                break;
        }
    }

    private void ValidateCode()
    {
        if (!ElementHelper.Validate(ifdCode))
            return;

        ScreenDialog.Instance.Display();

        switch (tggMethod.Value)
        {
            case "P":
                PhoneCodeRequest phoneCodeRequest = new PhoneCodeRequest(cmbPhoneCountry.GetSelectedId(),ifdPhoneNumber.Text, ifdCode.Text);
                
                switch (tggPhoneChannel.Value)
                {
                    case "S":
                        precheckService.ValidatePhoneSmsCode(phoneCodeRequest);
                        break;

                    case "W":
                        precheckService.ValidatePhoneWACode(phoneCodeRequest);
                        break;
                }
                break;

            case "E":
                EmailCodeRequest emailCodeRequest = new EmailCodeRequest(ifdEmail.Text,ifdCode.Text);

                precheckService.ValidateEmailCode(emailCodeRequest);
                break;
        }
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

        onValidateCode.Invoke();
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