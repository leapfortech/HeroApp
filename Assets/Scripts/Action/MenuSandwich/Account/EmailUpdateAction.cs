using System;

using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class EmailUpdateAction : MonoBehaviour
{
    [Serializable]
    public class UnityAccountChangedEvent : UnityEvent { }

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
    String updatedTitle = "Correo electrónico";

    [SerializeField]
    String updatedMessage = "Correo electrónico actualizado exitosamente.";

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

        ifdEmail.Clear();
    }

    private void ResetAccount()
    {
        if (!ElementHelper.Validate(ifdEmail))
            return;

        ScreenDialog.Instance.Display();

        AccountRequest request = new AccountRequest(WebManager.Instance.WebSysUser.Id, 2, -1, -1, null, ifdEmail.Text);
        accessService.ResetAccount(request);
    }

    public void ApplyResetAccount()
    {
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

        ResetAccount();
    }

    private void ValidateCode()
    {
        if (!ElementHelper.Validate(ifdCode))
            return;

        ScreenDialog.Instance.Display();

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

        UpdateAccountRequest request = new UpdateAccountRequest(WebManager.Instance.WebSysUser.Id,
                                                                -1, null, ifdEmail.Text);
        accessService.UpdateAccount(request);
    }


    public void ApplyUpdateEmail()
    {
        WebManager.Instance.WebSysUser.Email = ifdEmail.Text;
        Clear();
        onAccountChanged?.Invoke();
        ChoiceDialog.Instance.Info(updatedTitle, updatedMessage, () => PageManager.Instance.ChangePage(nextPageUpdate));
    }
}