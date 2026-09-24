using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using System.Collections.Generic;
using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class TaleService : MonoBehaviour
{
    [Serializable]
    public class TaleFeedResponseEvent : UnityEvent<TaleFeedResponse> { }
    [Serializable]
    public class TaleFullEvent : UnityEvent<TaleFull> { }

    [Serializable]
    public class TaleFullsEvent : UnityEvent<List<TaleFull>> { }

    [SerializeField]
    private TaleFeedResponseEvent onFeedRetreived = null;

    [SerializeField]
    private TaleFullEvent onFullRetreived = null;

    [SerializeField]
    private TaleFullsEvent onFullsRetreived = null;

    [SerializeField]
    private UnityLongEvent onRegistered = null;

    [SerializeField]
    private UnityBoolEvent onReactionChanged = null;

    [SerializeField]
    private UnityBoolEvent onUpdated = null;


    [Title("Errors")]
    [SerializeField]
    private UnityStringEvent onSendError = null;

    [SerializeField]
    private UnityStringEvent onResponseError = null;

    [SerializeField]
    private UnityStringEvent onTimeoutError = null;


    // GET
    public void GetFeed(TaleFeedRequest request)
    {
        TaleGetFeedOperation taleFeedGetOp = new TaleGetFeedOperation();
        try
        {
            taleFeedGetOp.request = request;
            taleFeedGetOp["on-complete"] = (Action<TaleGetFeedOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFeedRetreived.Invoke(op.response);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            taleFeedGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message, onSendError);
        }
    }

    public void GetFull(long id, long likeAppUserId)
    {
        TaleGetFullOperation taleFullGetOp = new TaleGetFullOperation();
        try
        {
            taleFullGetOp.id = id;
            taleFullGetOp.likeAppUserId = likeAppUserId;
            taleFullGetOp["on-complete"] = (Action<TaleGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.taleFull);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            taleFullGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFullByPostId(long postId, long likeAppUserId)
    {
        TaleFullByPostIdGetFullOperation taleFullByPostIdGetOp = new TaleFullByPostIdGetFullOperation();
        try
        {
            taleFullByPostIdGetOp.postId = postId;
            taleFullByPostIdGetOp.likeAppUserId = likeAppUserId;
            taleFullByPostIdGetOp["on-complete"] = (Action<TaleFullByPostIdGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.taleFull);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            taleFullByPostIdGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFulls(int status)
    {
        TaleGetFullsOperation taleFullsGetOp = new TaleGetFullsOperation();
        try
        {
            taleFullsGetOp.status = status;
            taleFullsGetOp["on-complete"] = (Action<TaleGetFullsOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullsRetreived.Invoke(op.taleFulls);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            taleFullsGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void Register(RegisterTaleRequest registerTaleRequest)
    {
        TaleRegisterOperation taleRegisterOp = new TaleRegisterOperation();
        try
        {
            taleRegisterOp.registerTaleRequest = registerTaleRequest;
            taleRegisterOp["on-complete"] = (Action<TaleRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt64(op.id));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            taleRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void RegisterReaction(Reaction reaction)
    {
        ReactionRegisterOperation reactionRegisterOp = new ReactionRegisterOperation();
        try
        {
            reactionRegisterOp.reaction = reaction;
            reactionRegisterOp["on-complete"] = (Action<ReactionRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onReactionChanged.Invoke(Convert.ToInt64(op.reactionId) != -1);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            reactionRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void DeleteReaction(Reaction reaction)
    {
        ReactionDeleteOperation reactionDeleteOp = new ReactionDeleteOperation();
        try
        {
            reactionDeleteOp.reaction = reaction;
            reactionDeleteOp["on-complete"] = (Action<ReactionDeleteOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onReactionChanged.Invoke(Convert.ToBoolean(op.done));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            reactionDeleteOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // UPDATE
    public void UpdateTale(RegisterTaleRequest registerTaleRequest)
    {
        TalePutOperation talePutOp = new TalePutOperation();
        try
        {
            talePutOp.registerTaleRequest = registerTaleRequest;
            talePutOp["on-complete"] = (Action<TalePutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(bool.Parse(op.response));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            talePutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void Accept(PostModerationRequest postModerationRequest)
    {
        TaleAcceptPutOperation acceptPutOp = new TaleAcceptPutOperation();
        try
        {
            acceptPutOp.postModerationRequest = postModerationRequest;
            acceptPutOp["on-complete"] = (Action<TaleAcceptPutOperation, HttpResponse>)((op, response) =>
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
        TaleRejectPutOperation rejectPutOp = new TaleRejectPutOperation();
        try
        {
            rejectPutOp.postModerationRequest = postModerationRequest;
            rejectPutOp["on-complete"] = (Action<TaleRejectPutOperation, HttpResponse>)((op, response) =>
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