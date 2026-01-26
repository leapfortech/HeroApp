using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using System.Collections.Generic;
using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class TreatmentService : MonoBehaviour
{
    [Serializable]
    public class TreatmentFullEvent : UnityEvent<TreatmentFull> { }

    [Serializable]
    public class TreatmentFullsEvent : UnityEvent<List<TreatmentFull>> { }

    [SerializeField]
    private TreatmentFullEvent onFullRetreived = null;

    [SerializeField]
    private TreatmentFullsEvent onFullsRetreived = null;

    [SerializeField]
    private UnityLongEvent onRegistered = null;

    [SerializeField]
    private UnityBoolEvent onUpdated = null;


    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;


    // GET
    public void GetFull(long id)
    {
        TreatmentGetFullOperation treatmentFullGetOp = new TreatmentGetFullOperation();
        try
        {
            treatmentFullGetOp.id = id;
            treatmentFullGetOp["on-complete"] = (Action<TreatmentGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.treatmentFull);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            treatmentFullGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFullByPostId(long postId)
    {
        TreatmentFullByPostIdGetFullOperation treatmentFullByPostIdGetOp = new TreatmentFullByPostIdGetFullOperation();
        try
        {
            treatmentFullByPostIdGetOp.postId = postId;
            treatmentFullByPostIdGetOp["on-complete"] = (Action<TreatmentFullByPostIdGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.treatmentFull);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            treatmentFullByPostIdGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFulls(int status)
    {
        TreatmentGetFullsOperation treatmentFullsGetOp = new TreatmentGetFullsOperation();
        try
        {
            treatmentFullsGetOp.status = status;
            treatmentFullsGetOp["on-complete"] = (Action<TreatmentGetFullsOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullsRetreived.Invoke(op.treatmentFulls);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            treatmentFullsGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void Register(RegisterTreatmentRequest registerTreatmentRequest)
    {
        TreatmentRegisterOperation referredRegisterOp = new TreatmentRegisterOperation();
        try
        {
            referredRegisterOp.registerTreatmentRequest = registerTreatmentRequest;
            referredRegisterOp["on-complete"] = (Action<TreatmentRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt64(op.id));
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
    public void UpdateTreatment(RegisterTreatmentRequest registerTreatmentRequest)
    {
        TreatmentPutOperation referredPutOp = new TreatmentPutOperation();
        try
        {
            referredPutOp.registerTreatmentRequest = registerTreatmentRequest;
            referredPutOp["on-complete"] = (Action<TreatmentPutOperation, HttpResponse>)((op, response) =>
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

    public void Accept(PostModerationRequest postModerationRequest)
    {
        TreatmentAcceptPutOperation acceptPutOp = new TreatmentAcceptPutOperation();
        try
        {
            acceptPutOp.postModerationRequest = postModerationRequest;
            acceptPutOp["on-complete"] = (Action<TreatmentAcceptPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(op.response);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            acceptPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void Reject(PostModerationRequest postModerationRequest)
    {
        TreatmentRejectPutOperation rejectPutOp = new TreatmentRejectPutOperation();
        try
        {
            rejectPutOp.postModerationRequest = postModerationRequest;
            rejectPutOp["on-complete"] = (Action<TreatmentRejectPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(op.response);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            rejectPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}