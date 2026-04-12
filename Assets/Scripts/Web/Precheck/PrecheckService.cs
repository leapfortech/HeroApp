using System;
using UnityEngine;
using UnityEngine.Events;

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
    private UnityStringEvent onPhoneCodeSmsValidated = null;

    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;

    // REGISTER
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
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            registerPhoneSmsPostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // VALIDATION
    public void ValidatePhoneCodeSms(PhoneCodeRequest phoneCodeRequest)
    {
        ValidatePhoneCodeSmsPostOperation validatePhoneCodeSmsPostOp = new ValidatePhoneCodeSmsPostOperation();
        try
        {
            validatePhoneCodeSmsPostOp.phoneCodeRequest = phoneCodeRequest;

            validatePhoneCodeSmsPostOp["on-complete"] = (Action<ValidatePhoneCodeSmsPostOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onPhoneCodeSmsValidated.Invoke(validatePhoneCodeSmsPostOp.result[1..^1]);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            validatePhoneCodeSmsPostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}
