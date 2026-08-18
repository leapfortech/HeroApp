using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using System.Collections.Generic;
using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class ReferredService : MonoBehaviour
{
    [Serializable]
    public class ReferredFullEvent : UnityEvent<List<ReferredFull>> { }


    [SerializeField]
    private ReferredFullEvent onRetreived = null;

    [SerializeField]
    private UnityIntEvent onValidated = null;

    [SerializeField]
    private UnityStringEvent onRegistered = null;

    [SerializeField]
    private UnityBoolEvent onUpdated = null;


    [Title("Errors")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;

    [SerializeField]
    private UnityStringEvent onTimeoutError = null;


    // GET
    public void GetByPeriod(DateTime startDate, DateTime endDate)
    {
        ByPeriodGetOperation byPeriodGetOp = new ByPeriodGetOperation();
        try
        {
            byPeriodGetOp.referredHistoryRequest = new ReferredHistoryRequest(StateManager.Instance.AppUser.Id, startDate, endDate);

            byPeriodGetOp["on-complete"] = (Action<ByPeriodGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRetreived.Invoke(op.referredFulls);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            byPeriodGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void Validate(String code)
    {
        ValidateGetOperation validateGetOp = new ValidateGetOperation();
        try
        {
            validateGetOp.code = code;

            validateGetOp["on-complete"] = (Action<ValidateGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onValidated.Invoke(Convert.ToInt32(op.response));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            validateGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void Register(RegisterReferredRequest registerReferredRequest)
    {
        ReferredRegisterOperation referredRegisterOp = new ReferredRegisterOperation();
        try
        {
            referredRegisterOp.registerReferredRequest = registerReferredRequest;
            referredRegisterOp["on-complete"] = (Action<ReferredRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(op.referredIds);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            referredRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // UPDATE
    public void UpdateReference(Referred referred)
    {
        ReferredPutOperation referredPutOp = new ReferredPutOperation();
        try
        {
            referredPutOp.referred = referred;
            referredPutOp["on-complete"] = (Action<ReferredPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(op.response);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            referredPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}
