using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class AppUserService : MonoBehaviour
{
    [Serializable]
    public class AppUserEvent : UnityEvent<AppUser> { }

    [Serializable]
    public class AliasResponseEvent : UnityEvent<AliasResponse> { }

    [SerializeField]
    private AppUserEvent onAppUserRetreived = null;

    [SerializeField]
    private UnityStringEvent onPortraitRetreived = null;

    [SerializeField]
    private UnityEvent onUpdated = null;

    [SerializeField]
    private UnityEvent onPhoneUpdated = null;

    [SerializeField]
    private UnityEvent onOptionsUpdated = null;

    [SerializeField]
    private UnityEvent onStatusUpdated = null;

    [SerializeField]
    private UnityLongEvent onReferredUpdated = null;

    [SerializeField]
    private AliasResponseEvent onAliasValidated = null;

    [SerializeField]
    private UnityEvent onPortraitUpdated = null;

    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;

    // GET
    public void GetAppUser(long appUserId)
    {
        AppUserGetOperation appUserGetOp = new AppUserGetOperation();
        try
        {
            appUserGetOp.id = appUserId;
            appUserGetOp["on-complete"] = (Action<AppUserGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onAppUserRetreived.Invoke(op.appUser);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            appUserGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void ValidateAlias(AliasRequest aliasRequest)
    {
        ValidateAliasPostOperation validateAliasPostOp = new ValidateAliasPostOperation();
        try
        {
            validateAliasPostOp.aliasRequest = aliasRequest;
            validateAliasPostOp["on-complete"] = (Action<ValidateAliasPostOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onAliasValidated.Invoke(op.aliasResponse);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            validateAliasPostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetPortrait(long appUserId)
    {
        PortraitAppUserGetOperation portraitAppUserGetOp = new PortraitAppUserGetOperation();
        try
        {
            portraitAppUserGetOp.appUserId = appUserId;
            portraitAppUserGetOp["on-complete"] = (Action<PortraitAppUserGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onPortraitRetreived.Invoke(op.portrait);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            portraitAppUserGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // UPDATE
    public void UpdatePerson(AppUser appUser)
    {
        AppUserPutOperation updateAppUserPutOp = new AppUserPutOperation();
        try
        {
            updateAppUserPutOp.appUser = appUser;
            updateAppUserPutOp["on-complete"] = (Action<AppUserPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke();
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            updateAppUserPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void UpdatePhone(PhoneRequest phoneRequest)
    {
        AppUserPhonePutOperation phonePutOp = new AppUserPhonePutOperation();
        try
        {
            phonePutOp.phoneRequest = phoneRequest;
            phonePutOp["on-complete"] = (Action<AppUserPhonePutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onPhoneUpdated.Invoke();
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            phonePutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void UpdateOptions(long options)
    {
        AppUserOptionsPutOperation optionsPutOp = new AppUserOptionsPutOperation();
        try
        {
            optionsPutOp.id = StateManager.Instance.AppUser.Id;
            optionsPutOp.options = options;
            optionsPutOp["on-complete"] = (Action<AppUserOptionsPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onOptionsUpdated.Invoke();
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            optionsPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void UpdateStatus(int status)
    {
        AppUserStatusPutOperation statusPutOp = new AppUserStatusPutOperation();
        try
        {
            statusPutOp.id = StateManager.Instance.AppUser.Id;
            statusPutOp.status = status;
            statusPutOp["on-complete"] = (Action<AppUserStatusPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onStatusUpdated.Invoke();
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            statusPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void UpdateReferred(String referredCode)
    {
        AppUserReferredPutOperation referredPutOp = new AppUserReferredPutOperation();
        try
        {
            referredPutOp.id = StateManager.Instance.AppUser.Id;
            referredPutOp.referredCode = referredCode;
            referredPutOp["on-complete"] = (Action<AppUserReferredPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onReferredUpdated.Invoke(Convert.ToInt64(op.referredAppUserId));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            referredPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void UpdatePortrait(long appUserId, String portrait)
    {
        PortraitPutOperation portraitPutOp = new PortraitPutOperation();
        try
        {
            portraitPutOp.appUserId = appUserId;
            portraitPutOp.portrait = "\"" + portrait + "\"";
            portraitPutOp["on-complete"] = (Action<PortraitPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onPortraitUpdated.Invoke();
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            portraitPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}
