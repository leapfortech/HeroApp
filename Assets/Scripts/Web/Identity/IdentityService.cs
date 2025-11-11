using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class IdentityService : MonoBehaviour
{
    [Serializable]
    public class IdentityEvent : UnityEvent<Identity> { }

    [Serializable]
    public class IdentityInfoEvent : UnityEvent<IdentityInfo> { }

    [Serializable]
    public class DpiPhotoEvent : UnityEvent<DpiPhoto> { }

    [SerializeField]
    private IdentityEvent onIdentityRetreived = null;

    [SerializeField]
    private IdentityInfoEvent onIdentityInfoRetreived = null;

    [SerializeField]
    private DpiPhotoEvent onDpiPhotoRetreived = null;

    [SerializeField]
    private UnityStringEvent onPortraitRetreived = null;

    [SerializeField]
    private UnityStringEvent onSignatureRetreived = null;

    [SerializeField]
    private UnityIntEvent onRegistered = null;

    [SerializeField]
    private UnityEvent onDpiFrontUpdated = null;

    [SerializeField]
    private UnityEvent onDpiBackUpdated = null;

    [SerializeField]
    private UnityIntEvent onIdentityInfoUpdated = null;

    [SerializeField]
    private UnityEvent onPortraitUpdated = null;

    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;

    // GET
    public void GetIdentity(int id)
    {
        IdentityGetOperation identityGetOp = new IdentityGetOperation();
        try
        {
            identityGetOp.id = id;
            identityGetOp["on-complete"] = (Action<IdentityGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onIdentityRetreived.Invoke(op.identity);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            identityGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetIdentityByAppUser(int appUserId, int status = 1)
    {
        IdentityAppUserGetOperation identityAppUserGetOp = new IdentityAppUserGetOperation();
        try
        {
            identityAppUserGetOp.appUserId = appUserId;
            identityAppUserGetOp.status = status;
            identityAppUserGetOp["on-complete"] = (Action<IdentityAppUserGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onIdentityRetreived.Invoke(op.identity);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            identityAppUserGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetIdentityInfoByAppUser(int appUserId, int status = 1)
    {
        IdentityInfoAppUserGetOperation identityFullAppUserGetOp = new IdentityInfoAppUserGetOperation();
        try
        {
            identityFullAppUserGetOp.appUserId = appUserId;
            identityFullAppUserGetOp.status = status;
            identityFullAppUserGetOp["on-complete"] = (Action<IdentityInfoAppUserGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onIdentityInfoRetreived.Invoke(op.identityInfo);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            identityFullAppUserGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetDpiPhotoByAppUser(int appUserId)
    {
        DpiPhotoAppUserGetOperation dpiPhotoAppUserGetOp = new DpiPhotoAppUserGetOperation();
        try
        {
            dpiPhotoAppUserGetOp.appUserId = appUserId;
            dpiPhotoAppUserGetOp["on-complete"] = (Action<DpiPhotoAppUserGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onDpiPhotoRetreived.Invoke(op.dpiPhoto);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            dpiPhotoAppUserGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetPortraitByAppUser(int appUserId)
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

    public void GetSignature(int signatureId)
    {
        SignatureGetOperation signatureGetOp = new SignatureGetOperation();
        try
        {
            signatureGetOp.signatureId = signatureId;
            signatureGetOp["on-complete"] = (Action<SignatureGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onSignatureRetreived.Invoke(op.strokes);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            signatureGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void Register(IdentityRegister identityRegister)
    {
        IdentityRegisterPostOperation identityRegisterPostOp = new IdentityRegisterPostOperation();
        try
        {
            identityRegisterPostOp.identityRegister = identityRegister;
            identityRegisterPostOp["on-complete"] = (Action<IdentityRegisterPostOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt32(op.id));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            identityRegisterPostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // UPDATE
    public void UpdateDpiFront(int appUserId, String dpiPhotos)
    {
        DpiFrontPutOperation dpiFrontPutOp = new DpiFrontPutOperation();
        try
        {
            dpiFrontPutOp.appUserId = appUserId;
            dpiFrontPutOp.dpiPhotos = "\"" + dpiPhotos + "\"";
            dpiFrontPutOp["on-complete"] = (Action<DpiFrontPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onDpiFrontUpdated.Invoke();
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            dpiFrontPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void UpdateDpiBack(int appUserId, String dpiBack)
    {
        DpiBackPutOperation dpiBackPutOp = new DpiBackPutOperation();
        try
        {
            dpiBackPutOp.appUserId = appUserId;
            dpiBackPutOp.dpiBack = "\"" + dpiBack + "\"";
            dpiBackPutOp["on-complete"] = (Action<DpiBackPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onDpiBackUpdated.Invoke();
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            dpiBackPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void UpdateIdentityInfo(IdentityInfo identityInfo)
    {
        IdentityInfoPutOperation identityInfoPutOp = new IdentityInfoPutOperation();
        try
        {
            identityInfoPutOp.identityFull = identityInfo;
            identityInfoPutOp["on-complete"] = (Action<IdentityInfoPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onIdentityInfoUpdated.Invoke(Convert.ToInt32(op.id));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            identityInfoPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void UpdatePortrait(int appUserId, String portrait)
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
