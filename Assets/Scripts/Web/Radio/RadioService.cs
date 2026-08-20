using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using System.Collections.Generic;
using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class RadioService : MonoBehaviour
{
    [Serializable]
    public class RadioFullEvent : UnityEvent<RadioFull> { }

    [Serializable]
    public class RadioFullsEvent : UnityEvent<List<RadioFull>> { }

    [SerializeField]
    private RadioFullEvent onFullRetreived = null;

    [SerializeField]
    private RadioFullsEvent onFullsRetreived = null;

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
        RadioGetFullOperation radioFullGetOp = new RadioGetFullOperation();
        try
        {
            radioFullGetOp.id = id;
            radioFullGetOp.likeAppUserId = likeAppUserId;
            radioFullGetOp["on-complete"] = (Action<RadioGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.radioFull);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            radioFullGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFullByPostId(long postId, long likeAppUserId)
    {
        RadioFullByPostIdGetFullOperation radioFullByPostIdGetOp = new RadioFullByPostIdGetFullOperation();
        try
        {
            radioFullByPostIdGetOp.postId = postId;
            radioFullByPostIdGetOp.likeAppUserId= likeAppUserId;
            radioFullByPostIdGetOp["on-complete"] = (Action<RadioFullByPostIdGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.radioFull);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            radioFullByPostIdGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFulls(int status)
    {
        RadioGetFullsOperation radioFullsGetOp = new RadioGetFullsOperation();
        try
        {
            radioFullsGetOp.status = status;
            radioFullsGetOp["on-complete"] = (Action<RadioGetFullsOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullsRetreived.Invoke(op.radioFulls);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            radioFullsGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void Register(RegisterRadioRequest registerRadioRequest)
    {
        RadioRegisterOperation referredRegisterOp = new RadioRegisterOperation();
        try
        {
            referredRegisterOp.registerRadioRequest = registerRadioRequest;
            referredRegisterOp["on-complete"] = (Action<RadioRegisterOperation, HttpResponse>)((op, response) =>
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

    public void RegisterRadioListen(RadioListen radioListen)
    {
        RadioListenRegisterOperation radioListenRegisterOp = new RadioListenRegisterOperation();
        try
        {
            radioListenRegisterOp.radioListen = radioListen;
            radioListenRegisterOp["on-complete"] = (Action<RadioListenRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt64(op.radioListenId));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            radioListenRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // UPDATE
    public void UpdateRadio(RegisterRadioRequest registerRadioRequest)
    {
        RadioPutOperation referredPutOp = new RadioPutOperation();
        try
        {
            referredPutOp.registerRadioRequest = registerRadioRequest;
            referredPutOp["on-complete"] = (Action<RadioPutOperation, HttpResponse>)((op, response) =>
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
        RadioAcceptPutOperation acceptPutOp = new RadioAcceptPutOperation();
        try
        {
            acceptPutOp.postModerationRequest = postModerationRequest;
            acceptPutOp["on-complete"] = (Action<RadioAcceptPutOperation, HttpResponse>)((op, response) =>
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
        RadioRejectPutOperation rejectPutOp = new RadioRejectPutOperation();
        try
        {
            rejectPutOp.postModerationRequest = postModerationRequest;
            rejectPutOp["on-complete"] = (Action<RadioRejectPutOperation, HttpResponse>)((op, response) =>
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