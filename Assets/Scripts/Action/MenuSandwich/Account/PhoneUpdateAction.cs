using System;

using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Web;
using Leap.UI.Extensions;

using Sirenix.OdinInspector;

public class PhoneUpdateAction : MonoBehaviour
{
    [Serializable]
    public class UnityAccountChangedEvent : UnityEvent { }

    [Title("Phone")]
    [SerializeField]
    ToggleGroup tggPhoneChannel = null;

    [SerializeField]
    ComboAdapter cmbPhoneCountry = null;

    [SerializeField]
    InputField ifdPhoneNumber = null;

    [Title("Code")]
    [SerializeField]
    Text txtResult = null;

    [SerializeField]
    InputField ifdCode = null;

    [Title("Action")]
    [SerializeField]
    Button btnReset = null;

    [SerializeField]
    Button btnResendCode = null;

    [SerializeField]
    Button btnValidateCode = null;

    [Title("Pages")]
    [SerializeField]
    Page pagValidate = null;

    [SerializeField]
    Page nextPageUpdate = null;

    [Header("Event")]
    [SerializeField]
    UnityAccountChangedEvent onAccountChanged = null;

    [Title("Messages")]
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

    [Space]
    [SerializeField]
    String updatedTitle = "Número de teléfono";

    [SerializeField]
    String updatedMessage = "Número de teléfono actualizado exitosamente.";

    AccessService accessService;
    PrecheckService precheckService = null;

    bool isResend = false;

    private void Initialize()
    {
        isResend = false;

        if (precheckService != null)
            return;

        accessService = GetComponent<AccessService>();
        precheckService = GetComponent<PrecheckService>();
    }

    private void Start()
    {
        Initialize();

        btnReset?.AddAction(ResetAccount);
        btnValidateCode?.AddAction(ValidateCode);
        btnResendCode?.AddAction(ResendCode);
    }

    public void Clear()
    {
        Initialize();

        cmbPhoneCountry.Clear();
        ifdPhoneNumber.Clear();
    }

    private void ResetAccount()
    {
        if (!ElementHelper.Validate(cmbPhoneCountry.Combo) || !ElementHelper.Validate(ifdPhoneNumber))
            return;

        ScreenDialog.Instance.Display();

        AccountRequest request = new AccountRequest(WebManager.Instance.WebSysUser.Id, 1, tggPhoneChannel.Value == "W" ? 1 : 2,
                                                    cmbPhoneCountry.GetSelectedId(), ifdPhoneNumber.Text, null);
        accessService.ResetAccount(request);
    }

    public void ApplyResetAccount()
    {
        txtResult.TextValue = "Ingresa el código que recibiste al número de celular <b> "
                               + cmbPhoneCountry.GetSelectedCellString("PhonePrefix")
                               + " "
                               + ifdPhoneNumber.Text
                               + "</b>";

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

        ResetAccount();
    }

    private void ValidateCode()
    {
        if (!ElementHelper.Validate(ifdCode))
            return;

        ScreenDialog.Instance.Display();

        PhoneCodeRequest phoneCodeRequest = new PhoneCodeRequest(cmbPhoneCountry.GetSelectedId(), ifdPhoneNumber.Text,
                                                                 ifdCode.Text);
        
        if (tggPhoneChannel.Value == "W")
            precheckService.ValidatePhoneWACode(phoneCodeRequest);
        else
            precheckService.ValidatePhoneSmsCode(phoneCodeRequest);
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

        String email = "hm." + cmbPhoneCountry.GetSelectedRecord().Id.ToString() + "."
                        + ifdPhoneNumber.Text.Replace("-", "")
                        + "@heroesmigrantes.com";

        UpdateAccountRequest request = new UpdateAccountRequest(WebManager.Instance.WebSysUser.Id,
                                                                cmbPhoneCountry.GetSelectedId(), ifdPhoneNumber.Text,
                                                                email);
        accessService.UpdateAccount(request);
    }


    public void ApplyUpdatePhone()
    {
        WebManager.Instance.WebSysUser.PhoneCountryId = cmbPhoneCountry.GetSelectedId();
        WebManager.Instance.WebSysUser.Phone = ifdPhoneNumber.Text;

        Clear();
        onAccountChanged?.Invoke();
        ChoiceDialog.Instance.Info(updatedTitle, updatedMessage, () => PageManager.Instance.ChangePage(nextPageUpdate));
    }
}