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
    private UnityBoolEvent onFavoriteChanged = null;

    [SerializeField]
    private UnityBoolEvent onReactionChanged = null;

    [SerializeField]
    private UnityLongEvent onPlaintRegistered = null;

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
    public void GetFull(long id, long reactionAppUserId)
    {
        TreatmentGetFullOperation treatmentFullGetOp = new TreatmentGetFullOperation();
        try
        {
            treatmentFullGetOp.id = id;
            treatmentFullGetOp.reactionAppUserId = reactionAppUserId;
            treatmentFullGetOp["on-complete"] = (Action<TreatmentGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.treatmentFull);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            treatmentFullGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message, onSendError);
        }
    }

    public void GetFullByPostId(long postId, long reactionAppUserId)
    {
        TreatmentFullByPostIdGetFullOperation treatmentFullByPostIdGetOp = new TreatmentFullByPostIdGetFullOperation();
        try
        {
            treatmentFullByPostIdGetOp.postId = postId;
            treatmentFullByPostIdGetOp.reactionAppUserId=reactionAppUserId;
            treatmentFullByPostIdGetOp["on-complete"] = (Action<TreatmentFullByPostIdGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.treatmentFull);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            treatmentFullByPostIdGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message, onSendError);
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
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            treatmentFullsGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message, onSendError);
        }
    }

    // REGISTER
    public void Register(TreatmentFull treatmentFull)
    {
        TreatmentRegisterOperation referredRegisterOp = new TreatmentRegisterOperation();
        try
        {
            referredRegisterOp.treatmentFull = treatmentFull;
            referredRegisterOp["on-complete"] = (Action<TreatmentRegisterOperation, HttpResponse>)((op, response) =>
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
            WebManager.Instance.OnSendError(ex.Message, onSendError);
        }
    }

    public void RegisterFavorite(Favorite favorite)
    {
        FavoriteRegisterOperation favoriteRegisterOp = new FavoriteRegisterOperation();
        try
        {
            favoriteRegisterOp.favorite = favorite;
            favoriteRegisterOp["on-complete"] = (Action<FavoriteRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFavoriteChanged.Invoke(Convert.ToInt64(op.favoriteId) != -1);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            favoriteRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message, onSendError);
        }
    }

    public void DeleteFavorite(Favorite favorite)
    {
        FavoriteDeleteOperation favoriteDeleteOp = new FavoriteDeleteOperation();
        try
        {
            favoriteDeleteOp.favorite = favorite;
            favoriteDeleteOp["on-complete"] = (Action<FavoriteDeleteOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFavoriteChanged.Invoke(Convert.ToBoolean(op.done));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            favoriteDeleteOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message, onSendError);
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
            WebManager.Instance.OnSendError(ex.Message, onSendError);
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
            WebManager.Instance.OnSendError(ex.Message, onSendError);
        }
    }

    public void RegisterPostPlaint(PostPlaint postPlaint)
    {
        PostPlaintRegisterOperation postPlaintRegisterOp = new PostPlaintRegisterOperation();
        try
        {
            postPlaintRegisterOp.postPlaint = postPlaint;
            postPlaintRegisterOp["on-complete"] = (Action<PostPlaintRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onPlaintRegistered.Invoke(Convert.ToInt64(op.postPlaintId));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            postPlaintRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message, onSendError);
        }
    }

    // UPDATE
    public void UpdateTreatment(TreatmentFull treatmentFull)
    {
        TreatmentPutOperation referredPutOp = new TreatmentPutOperation();
        try
        {
            referredPutOp.treatmentFull = treatmentFull;
            referredPutOp["on-complete"] = (Action<TreatmentPutOperation, HttpResponse>)((op, response) =>
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
            WebManager.Instance.OnSendError(ex.Message, onSendError);
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
                    onUpdated.Invoke(bool.Parse(op.response));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            acceptPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message, onSendError);
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
                    onUpdated.Invoke(bool.Parse(op.response));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            rejectPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message, onSendError);
        }
    }

    public void Delete(PostModerationRequest postModerationRequest)
    {
        TreatmentDeletePutOperation deletePutOp = new TreatmentDeletePutOperation();
        try
        {
            deletePutOp.postModerationRequest = postModerationRequest;
            deletePutOp["on-complete"] = (Action<TreatmentDeletePutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(bool.Parse(op.response));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            deletePutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message, onSendError);
        }
    }
}