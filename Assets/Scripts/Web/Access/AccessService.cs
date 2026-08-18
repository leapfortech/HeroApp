using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit;
using hg.ApiWebKit.core.http;

using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class AccessService : MonoBehaviour
{
    [Serializable]
    public class UnityLoginEvent : UnityEvent<LoginAppResponse> { }

    [Serializable]
    public class UnityLoginAppDataEvent : UnityEvent<LoginAppInfo> { }

    [Serializable]
    public class OnboardingEvent : UnityEvent<OnboardingResponse> { }

    [Space]
    [SerializeField]
    private UnityLoginEvent onLogged = null;

    [SerializeField]
    private UnityLoginAppDataEvent onLoginAppInfoRetreived = null;

    [SerializeField]
    private UnityStringEvent onRegisteredApp = null;

    [SerializeField]
    private OnboardingEvent onOnboardingRegistered = null;

    [SerializeField]
    private UnityLongEvent onResetPasswordSent = null;

    [SerializeField]
    private UnityEvent onPasswordUpdated = null;

    [SerializeField]
    private UnityEvent onResetAccountSent = null;

    [SerializeField]
    private UnityEvent onAccountUpdated = null;

    [Title("Errors")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;

    [SerializeField]
    private UnityStringEvent onTimeoutError = null;

    // Login
    public void LoginApp(String email, String version)
    {
        AccessLoginAppPostOperation loginAppPostOp = new AccessLoginAppPostOperation();
        try
        {
            loginAppPostOp.loginRequest = new LoginRequest(email, version);
            loginAppPostOp["on-complete"] = (Action<AccessLoginAppPostOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onLogged.Invoke(op.loginResponse);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            loginAppPostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetLoginAppInfo()
    {
        LoginAppInfoGetOperation loginAppInfoGetOp = new LoginAppInfoGetOperation();
        try
        {
            loginAppInfoGetOp.appUserId = StateManager.Instance.AppUser.Id;
            loginAppInfoGetOp.webSysUserId = WebManager.Instance.WebSysUser.Id;
            loginAppInfoGetOp["on-complete"] = (Action<LoginAppInfoGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onLoginAppInfoRetreived.Invoke(op.loginAppInfo);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            loginAppInfoGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // Register
    public void RegisterApp(RegisterAppRequest registerAppRequest)
    {
        AccessRegisterAppPostOperation registerAppPostOp = new AccessRegisterAppPostOperation();
        try
        {
            registerAppPostOp.registerAppRequest = registerAppRequest;
            registerAppPostOp["on-complete"] = (Action<AccessRegisterAppPostOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegisteredApp.Invoke(op.registerResponse);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            registerAppPostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void Onboarding(OnboardingRequest onboardingRequest)
    {
        OnboardingPostOperation onboardingPostOp = new OnboardingPostOperation();
        try
        {
            onboardingPostOp.onboardingRequest = onboardingRequest;
            onboardingPostOp["on-complete"] = (Action<OnboardingPostOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onOnboardingRegistered.Invoke(op.onboardingResponse);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            onboardingPostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void ResetPassword(PasswordRequest resetPasswordRequest)
    {
        ResetPasswordPostOperation resetPasswordPostOp = new ResetPasswordPostOperation();
        try
        {
            resetPasswordPostOp.resetPasswordRequest = resetPasswordRequest;
            resetPasswordPostOp["on-complete"] = (Action<ResetPasswordPostOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onResetPasswordSent.Invoke(Convert.ToInt64(op.webSysUserId));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            resetPasswordPostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void UpdatePassword(UpdatePasswordRequest updatePasswordRequest)
    {
        UpdatePasswordPutOperation updatePasswordPutOp = new UpdatePasswordPutOperation();
        try
        {
            updatePasswordPutOp.updatePasswordRequest = updatePasswordRequest;
            updatePasswordPutOp["on-complete"] = (Action<UpdatePasswordPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onPasswordUpdated.Invoke();
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            updatePasswordPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void ResetAccount(AccountRequest accountRequest)
    {
        ResetAccountPostOperation resetAccountPostOp = new ResetAccountPostOperation();
        try
        {
            resetAccountPostOp.accountRequest = accountRequest;
            resetAccountPostOp["on-complete"] = (Action<ResetAccountPostOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onResetAccountSent.Invoke();
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            resetAccountPostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void UpdateAccount(UpdateAccountRequest updateAccountRequest)
    {
        UpdateAccountPutOperation updateAccountPutOp = new UpdateAccountPutOperation();
        try
        {
            updateAccountPutOp.updateAccountRequest = updateAccountRequest;
            updateAccountPutOp["on-complete"] = (Action<UpdateAccountPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onAccountUpdated.Invoke();
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            updateAccountPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}
