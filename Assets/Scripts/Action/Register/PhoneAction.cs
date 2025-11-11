using System;

using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.UI.Extensions;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class PhoneAction : MonoBehaviour
{
    [Title("Phone")]
    [SerializeField]
    ComboAdapter cmbPhoneCountry = null;

    [SerializeField]
    InputField ifdPhoneNumber = null;

    [SerializeField]
    Text txtPhoneNumber = null;

    [SerializeField]
    InputField ifdCode = null;

    [Title("Action")]
    [SerializeField]
    Button btnRegisterPhone = null;

    [SerializeField]
    Button btnResendCode = null;

    [SerializeField]
    Button btnValidateCode = null;

    [SerializeField]
    bool isSandwich = false;

    //[Title("Pages")]
    //[SerializeField]
    //Page nextPageRegister = null;

    //[SerializeField]
    //Page nextPageValidate = null;

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

    PhoneService phoneService = null;
    ElementValue[] elementValues = null;
    bool isResend = false;

    private void Start()
    {
        Initialize();

        btnRegisterPhone?.AddAction(RegisterPhone);
        btnValidateCode?.AddAction(ValidateCode);
        btnResendCode?.AddAction(ResendCode);
    }

    private void Initialize()
    {
        isResend = false;

        if (phoneService != null)
            return;

        elementValues = new ElementValue[2];
        elementValues[0] = cmbPhoneCountry.Combo;
        elementValues[1] = ifdPhoneNumber;

        phoneService = GetComponent<PhoneService>();
    }

    public void Clear()
    {
        Initialize();
        for (int i = 0; i < elementValues.Length; i++)
            elementValues[i].Clear();
    }

    public void SelectIdCountry(int countryId)
    {
        cmbPhoneCountry.Select(countryId);
    }

    private void RegisterPhone()
    {
        Initialize();

        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        if (isSandwich)
            DoRegisterPhone(null);
        else
            FirebaseManager.Instance.LoginStartToken(DoRegisterPhone, null);
    }

    private void ResendCode()
    {
        ScreenDialog.Instance.Display();
        isResend = true;
        DoRegisterPhone(null);
    }

    private void DoRegisterPhone(String _)
    {
        phoneService.RegisterPhone(cmbPhoneCountry.GetSelectedId(), ifdPhoneNumber.Text);
    }

    public void ApplyRegisterPhone(String result)
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

        txtPhoneNumber.TextValue = cmbPhoneCountry.GetSelectedCellString("PhonePrefix") + " " + ifdPhoneNumber.Text;

        if(!isResend)
            onRegistered.Invoke();
        else
            ChoiceDialog.Instance.Info(resendTitle, resendMessage); 
    }

    private void ValidateCode()
    {
        if (!ElementHelper.Validate(ifdCode))
            return;

        ScreenDialog.Instance.Display();
        PhoneCodeRequest phoneCodeRequest = new PhoneCodeRequest(cmbPhoneCountry.GetSelectedId(), ifdPhoneNumber.Text, ifdCode.Text);
        phoneService.ValidateCode(phoneCodeRequest);
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