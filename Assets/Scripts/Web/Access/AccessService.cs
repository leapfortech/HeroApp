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

    [Space]
    [SerializeField]
    private UnityLoginEvent onLogged = null;

    [SerializeField]
    private UnityLoginAppDataEvent onLoginAppInfoRetreived = null;

    [SerializeField]
    private UnityStringEvent onRegisteredApp = null;

    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;

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
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
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
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
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
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            registerAppPostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}
