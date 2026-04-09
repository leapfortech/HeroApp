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

    [SerializeField]
    private IdentityEvent onIdentityRetreived = null;

    [SerializeField]
    private UnityLongEvent onRegistered = null;

    [SerializeField]
    private UnityLongEvent onUpdated = null;

    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;

    // GET
    public void GetIdentity(long id)
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

    public void GetIdentityByAppUser(long appUserId, int status = 1)
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

    // REGISTER
    public void Register(Identity identity)
    {
        IdentityRegisterPostOperation identityRegisterPostOp = new IdentityRegisterPostOperation();
        try
        {
            identityRegisterPostOp.identity = identity;
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
    public void UpdatePersonal(IdentityPersonal identityPersonal)
    {
        PersonalPutOperation personalPutOp = new PersonalPutOperation();
        try
        {
            personalPutOp.identityPersonal = identityPersonal;
            personalPutOp["on-complete"] = (Action<PersonalPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(Convert.ToInt64(op.id));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            personalPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void UpdatePlace(IdentityPlace identityPlace)
    {
        PlacePutOperation placePutOp = new PlacePutOperation();
        try
        {
            placePutOp.identityPlace = identityPlace;
            placePutOp["on-complete"] = (Action<PlacePutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(Convert.ToInt64(op.id));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            placePutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void UpdateIdentity(long appUserId, Identity identity)
    {
        IdentityPutOperation identityPutOp = new IdentityPutOperation();
        try
        {
            identityPutOp.appUserId = appUserId;
            identityPutOp.identity = identity;
            identityPutOp["on-complete"] = (Action<IdentityPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(Convert.ToInt64(op.id));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            identityPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}
