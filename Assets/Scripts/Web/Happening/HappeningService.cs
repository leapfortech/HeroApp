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
    public class HappeningFeedResponseEvent : UnityEvent<HappeningFeedResponse> { }

    [Serializable]
    public class HappeningFullEvent : UnityEvent<HappeningFull> { }

    [Serializable]
    public class HappeningFullsEvent : UnityEvent<List<HappeningFull>> { }

    [SerializeField]
    private HappeningFeedResponseEvent onFeedRetreived = null;

    [SerializeField]
    private HappeningFullEvent onFullRetreived = null;

    [SerializeField]
    private HappeningFullsEvent onFullsRetreived = null;

    [SerializeField]
    private UnityLongEvent onRegistered = null;

    [SerializeField]
    private UnityBoolEvent onFavoriteChanged = null;

    [SerializeField]
    private UnityBoolEvent onSelectedChanged = null;

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
    public void GetFeed(HappeningFeedRequest request)
    {
        HappeningGetFeedOperation happeningFeedGetOp = new HappeningGetFeedOperation();
        try
        {
            happeningFeedGetOp.request = request;
            happeningFeedGetOp["on-complete"] = (Action<HappeningGetFeedOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFeedRetreived.Invoke(op.response);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            happeningFeedGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message, onSendError);
        }
    }

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
        HappeningRegisterOperation happeningRegisterOp = new HappeningRegisterOperation();
        try
        {
            happeningRegisterOp.registerHappeningRequest = registerHappeningRequest;
            happeningRegisterOp["on-complete"] = (Action<HappeningRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt64(op.id));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            happeningRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
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
            WebManager.Instance.OnSendError(ex.Message);
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
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void RegisterSelected(Selected selected)
    {
        SelectedRegisterOperation selectedRegisterOp = new SelectedRegisterOperation();
        try
        {
            selectedRegisterOp.selected = selected;
            selectedRegisterOp["on-complete"] = (Action<SelectedRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onSelectedChanged.Invoke(Convert.ToInt64(op.selectedId) != -1);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            selectedRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void DeleteSelected(Selected selected)
    {
        SelectedDeleteOperation selectedDeleteOp = new SelectedDeleteOperation();
        try
        {
            selectedDeleteOp.selected = selected;
            selectedDeleteOp["on-complete"] = (Action<SelectedDeleteOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onSelectedChanged.Invoke(Convert.ToBoolean(op.done));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            selectedDeleteOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // UPDATE
    public void UpdateHappening(RegisterHappeningRequest registerHappeningRequest)
    {
        HappeningPutOperation happeningPutOp = new HappeningPutOperation();
        try
        {
            happeningPutOp.registerHappeningRequest = registerHappeningRequest;
            happeningPutOp["on-complete"] = (Action<HappeningPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(bool.Parse(op.response));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            happeningPutOp.Send();
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