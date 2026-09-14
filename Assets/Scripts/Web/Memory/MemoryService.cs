using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using System.Collections.Generic;
using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class MemoryService : MonoBehaviour
{
    [Serializable]
    public class MemoryFullEvent : UnityEvent<MemoryFull> { }

    [Serializable]
    public class MemoryFullsEvent : UnityEvent<List<MemoryFull>> { }

    [SerializeField]
    private MemoryFullEvent onFullRetreived = null;

    [SerializeField]
    private MemoryFullsEvent onFullsRetreived = null;

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
        MemoryGetFullOperation memoryFullGetOp = new MemoryGetFullOperation();
        try
        {
            memoryFullGetOp.id = id;
            memoryFullGetOp.likeAppUserId = likeAppUserId;
            memoryFullGetOp["on-complete"] = (Action<MemoryGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.memoryFull);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            memoryFullGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFullByPostId(long postId, long likeAppUserId)
    {
        MemoryFullByPostIdGetFullOperation memoryFullByPostIdGetOp = new MemoryFullByPostIdGetFullOperation();
        try
        {
            memoryFullByPostIdGetOp.postId = postId;
            memoryFullByPostIdGetOp.likeAppUserId=likeAppUserId;
            memoryFullByPostIdGetOp["on-complete"] = (Action<MemoryFullByPostIdGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.memoryFull);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            memoryFullByPostIdGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFulls(int status)
    {
        MemoryGetFullsOperation memoryFullsGetOp = new MemoryGetFullsOperation();
        try
        {
            memoryFullsGetOp.status = status;
            memoryFullsGetOp["on-complete"] = (Action<MemoryGetFullsOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullsRetreived.Invoke(op.memoryFulls);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            memoryFullsGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void Register(RegisterMemoryRequest registerMemoryRequest)
    {
        MemoryRegisterOperation memoryRegisterOp = new MemoryRegisterOperation();
        try
        {
            memoryRegisterOp.registerMemoryRequest = registerMemoryRequest;
            memoryRegisterOp["on-complete"] = (Action<MemoryRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt64(op.id));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            memoryRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // UPDATE
    public void UpdateMemory(RegisterMemoryRequest registerMemoryRequest)
    {
        MemoryPutOperation memoryPutOp = new MemoryPutOperation();
        try
        {
            memoryPutOp.registerMemoryRequest = registerMemoryRequest;
            memoryPutOp["on-complete"] = (Action<MemoryPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(bool.Parse(op.response));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            memoryPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void Accept(PostModerationRequest postModerationRequest)
    {
        MemoryAcceptPutOperation acceptPutOp = new MemoryAcceptPutOperation();
        try
        {
            acceptPutOp.postModerationRequest = postModerationRequest;
            acceptPutOp["on-complete"] = (Action<MemoryAcceptPutOperation, HttpResponse>)((op, response) =>
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
        MemoryRejectPutOperation rejectPutOp = new MemoryRejectPutOperation();
        try
        {
            rejectPutOp.postModerationRequest = postModerationRequest;
            rejectPutOp["on-complete"] = (Action<MemoryRejectPutOperation, HttpResponse>)((op, response) =>
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