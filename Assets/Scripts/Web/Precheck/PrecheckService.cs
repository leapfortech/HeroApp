using System;
using UnityEngine;

using hg.ApiWebKit.core.http;

using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class PrecheckService : MonoBehaviour
{
    [Space]
    [SerializeField]
    private UnityStringEvent onPhoneSmsRegistered = null;

    [SerializeField]
    private UnityStringEvent onPhoneSmsCodeValidated = null;

    [Space]
    [SerializeField]
    private UnityStringEvent onPhoneWARegistered = null;

    [SerializeField]
    private UnityStringEvent onPhoneWACodeValidated = null;

    [Space]
    [SerializeField]
    private UnityStringEvent onEmailRegistered = null;

    [SerializeField]
    private UnityStringEvent onEmailCodeValidated = null;

    [Title("Errors")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;

    [SerializeField]
    private UnityStringEvent onTimeoutError = null;

    // SMS
    public void RegisterPhoneSms(long phoneCountryId, String phoneNumber)
    {
        RegisterPhoneSmsPostOperation registerPhoneSmsPostOp = new RegisterPhoneSmsPostOperation();
        try
        {
            registerPhoneSmsPostOp.phoneCountryId = phoneCountryId;
            registerPhoneSmsPostOp.phoneNumber = phoneNumber;

            registerPhoneSmsPostOp["on-complete"] = (Action<RegisterPhoneSmsPostOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onPhoneSmsRegistered.Invoke(registerPhoneSmsPostOp.result[1..^1]);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            registerPhoneSmsPostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void ValidatePhoneSmsCode(PhoneCodeRequest phoneCodeRequest)
    {
        ValidatePhoneSmsCodePostOperation validatePhoneSmsCodePostOp = new ValidatePhoneSmsCodePostOperation();
        try
        {
            validatePhoneSmsCodePostOp.phoneCodeRequest = phoneCodeRequest;
            validatePhoneSmsCodePostOp["on-complete"] = (Action<ValidatePhoneSmsCodePostOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onPhoneSmsCodeValidated.Invoke(validatePhoneSmsCodePostOp.result[1..^1]);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            validatePhoneSmsCodePostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // WHATSAPP
    public void RegisterPhoneWA(long phoneCountryId, String phoneNumber)
    {
        RegisterPhoneWAPostOperation registerPhoneWAPostOp = new RegisterPhoneWAPostOperation();
        try
        {
            registerPhoneWAPostOp.phoneCountryId = phoneCountryId;
            registerPhoneWAPostOp.phoneNumber = phoneNumber;

            registerPhoneWAPostOp["on-complete"] = (Action<RegisterPhoneWAPostOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onPhoneWARegistered.Invoke(registerPhoneWAPostOp.result[1..^1]);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            registerPhoneWAPostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void ValidatePhoneWACode(PhoneCodeRequest phoneCodeRequest)
    {
        ValidatePhoneWACodePostOperation validatePhoneWACodePostOp = new ValidatePhoneWACodePostOperation();
        try
        {
            validatePhoneWACodePostOp.phoneCodeRequest = phoneCodeRequest;
            validatePhoneWACodePostOp["on-complete"] = (Action<ValidatePhoneWACodePostOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onPhoneWACodeValidated.Invoke(validatePhoneWACodePostOp.result[1..^1]);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            validatePhoneWACodePostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // EMAIL
    public void RegisterEmail(String email)
    {
        RegisterEmailPostOperation registerEmailPostOp = new RegisterEmailPostOperation();
        try
        {
            registerEmailPostOp.email = email;
            registerEmailPostOp["on-complete"] = (Action<RegisterEmailPostOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onEmailRegistered.Invoke(registerEmailPostOp.result[1..^1]);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            registerEmailPostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void ValidateEmailCode(EmailCodeRequest emailCodeRequest)
    {
        ValidateEmailCodePostOperation validateEmailCodePostOp = new ValidateEmailCodePostOperation();
        try
        {
            validateEmailCodePostOp.emailCodeRequest = emailCodeRequest;
            validateEmailCodePostOp["on-complete"] = (Action<ValidateEmailCodePostOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onEmailCodeValidated.Invoke(validateEmailCodePostOp.result[1..^1]);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            validateEmailCodePostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}
