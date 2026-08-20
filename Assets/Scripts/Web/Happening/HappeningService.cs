using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using System.Collections.Generic;
using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class HappeningService : MonoBehaviour
{
    [Serializable]
    public class HappeningFullEvent : UnityEvent<HappeningFull> { }

    [Serializable]
    public class HappeningFullsEvent : UnityEvent<List<HappeningFull>> { }

    [SerializeField]
    private HappeningFullEvent onFullRetreived = null;

    [SerializeField]
    private HappeningFullsEvent onFullsRetreived = null;

    [SerializeField]
    private UnityLongEvent onRegistered = null;

    [SerializeField]
    private UnityBoolEvent onUpdated = null;


    [Title("Errors")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;

    [SerializeField]
    private UnityStringEvent onTimeoutError = null;


    // GET
    public void GetFull(long id, long likeAppUserId)
    {
        HappeningGetFullOperation happeningFullGetOp = new HappeningGetFullOperation();
        try
        {
            happeningFullGetOp.id = id;
            happeningFullGetOp.likeAppUserId = likeAppUserId;
            happeningFullGetOp["on-complete"] = (Action<HappeningGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.happeningFull);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            happeningFullGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFullByPostId(long postId, long likeAppUserId)
    {
        HappeningFullByPostIdGetFullOperation happeningFullByPostIdGetOp = new HappeningFullByPostIdGetFullOperation();
        try
        {
            happeningFullByPostIdGetOp.postId = postId;
            happeningFullByPostIdGetOp.likeAppUserId=likeAppUserId;
            happeningFullByPostIdGetOp["on-complete"] = (Action<HappeningFullByPostIdGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.happeningFull);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            happeningFullByPostIdGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFulls(int status)
    {
        HappeningGetFullsOperation happeningFullsGetOp = new HappeningGetFullsOperation();
        try
        {
            happeningFullsGetOp.status = status;
            happeningFullsGetOp["on-complete"] = (Action<HappeningGetFullsOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullsRetreived.Invoke(op.happeningFulls);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            happeningFullsGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void Register(RegisterHappeningRequest registerHappeningRequest)
    {
        HappeningRegisterOperation referredRegisterOp = new HappeningRegisterOperation();
        try
        {
            referredRegisterOp.registerHappeningRequest = registerHappeningRequest;
            referredRegisterOp["on-complete"] = (Action<HappeningRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt64(op.id));
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
    public void UpdateHappening(RegisterHappeningRequest registerHappeningRequest)
    {
        HappeningPutOperation referredPutOp = new HappeningPutOperation();
        try
        {
            referredPutOp.registerHappeningRequest = registerHappeningRequest;
            referredPutOp["on-complete"] = (Action<HappeningPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(bool.Parse(op.response));
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

    public void Accept(PostModerationRequest postModerationRequest)
    {
        HappeningAcceptPutOperation acceptPutOp = new HappeningAcceptPutOperation();
        try
        {
            acceptPutOp.postModerationRequest = postModerationRequest;
            acceptPutOp["on-complete"] = (Action<HappeningAcceptPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(bool.Parse(op.response));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
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
        HappeningRejectPutOperation rejectPutOp = new HappeningRejectPutOperation();
        try
        {
            rejectPutOp.postModerationRequest = postModerationRequest;
            rejectPutOp["on-complete"] = (Action<HappeningRejectPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(bool.Parse(op.response));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            rejectPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}