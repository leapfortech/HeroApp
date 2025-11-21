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
    public class ReferredEvent : UnityEvent<List<Referred>> { }


    [SerializeField]
    private ReferredEvent onRetreived = null;

    [SerializeField]
    private UnityIntEvent onValidated = null;

    [SerializeField]
    private UnityStringEvent onRegistered = null;

    [SerializeField]
    private UnityBoolEvent onUpdated = null;


    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;


    // GET
    public void GetReferrals()
    {
        ReferredGetOperation referredGetOp = new ReferredGetOperation();
        try
        {
            referredGetOp.appUserId = StateManager.Instance.AppUser.Id;
            referredGetOp["on-complete"] = (Action<ReferredGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRetreived.Invoke(op.referreds);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            referredGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetByPeriod(DateTime startDate, DateTime endDate)
    {
        ByPeriodGetOperation byPeriodGetOp = new ByPeriodGetOperation();
        try
        {
            byPeriodGetOp.referredHistoryRequest = new ReferredHistoryRequest(StateManager.Instance.AppUser.Id, startDate, endDate);

            byPeriodGetOp["on-complete"] = (Action<ByPeriodGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRetreived.Invoke(op.referreds);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            byPeriodGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void Validate(long id)
    {
        ValidateGetOperation validateGetOp = new ValidateGetOperation();
        try
        {
            validateGetOp.id = id;

            validateGetOp["on-complete"] = (Action<ValidateGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onValidated.Invoke(Convert.ToInt32(op.response));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            validateGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void Register(Referred referred)
    {
        ReferredRegisterOperation referredRegisterOp = new ReferredRegisterOperation();
        try
        {
            referredRegisterOp.referred = referred;
            referredRegisterOp["on-complete"] = (Action<ReferredRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(op.referredIds);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
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
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            referredPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}
