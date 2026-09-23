using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using System.Collections.Generic;
using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class NewsService : MonoBehaviour
{
    [Serializable]
    public class NewsFeedResponseEvent : UnityEvent<NewsFeedResponse> { }
    [Serializable]
    public class NewsFullEvent : UnityEvent<NewsFull> { }

    [Serializable]
    public class NewsFullsEvent : UnityEvent<List<NewsFull>> { }

    [SerializeField]
    private NewsFeedResponseEvent onFeedRetreived = null;

    [SerializeField]
    private NewsFullEvent onFullRetreived = null;

    [SerializeField]
    private NewsFullsEvent onFullsRetreived = null;

    [SerializeField]
    private UnityLongEvent onRegistered = null;

    [SerializeField]
    private UnityBoolEvent onFavoriteChanged = null;

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
    public void GetFeed(NewsFeedRequest request)
    {
        NewsGetFeedOperation newsFeedGetOp = new NewsGetFeedOperation();
        try
        {
            newsFeedGetOp.request = request;
            newsFeedGetOp["on-complete"] = (Action<NewsGetFeedOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFeedRetreived.Invoke(op.response);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            newsFeedGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message, onSendError);
        }
    }

    public void GetFull(long id, long likeAppUserId)
    {
        NewsGetFullOperation newsFullGetOp = new NewsGetFullOperation();
        try
        {
            newsFullGetOp.id = id;
            newsFullGetOp.likeAppUserId = likeAppUserId;
            newsFullGetOp["on-complete"] = (Action<NewsGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.newsFull);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            newsFullGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFullByPostId(long postId, long likeAppUserId)
    {
        NewsFullByPostIdGetFullOperation newsFullByPostIdGetOp = new NewsFullByPostIdGetFullOperation();
        try
        {
            newsFullByPostIdGetOp.postId = postId;
            newsFullByPostIdGetOp.likeAppUserId = likeAppUserId;
            newsFullByPostIdGetOp["on-complete"] = (Action<NewsFullByPostIdGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.newsFull);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            newsFullByPostIdGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFulls(int status)
    {
        NewsGetFullsOperation newsFullsGetOp = new NewsGetFullsOperation();
        try
        {
            newsFullsGetOp.status = status;
            newsFullsGetOp["on-complete"] = (Action<NewsGetFullsOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullsRetreived.Invoke(op.newsFulls);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            newsFullsGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void Register(RegisterNewsRequest registerNewsRequest)
    {
        NewsRegisterOperation referredRegisterOp = new NewsRegisterOperation();
        try
        {
            referredRegisterOp.registerNewsRequest = registerNewsRequest;
            referredRegisterOp["on-complete"] = (Action<NewsRegisterOperation, HttpResponse>)((op, response) =>
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

    // UPDATE
    public void UpdateNews(RegisterNewsRequest registerNewsRequest)
    {
        NewsPutOperation referredPutOp = new NewsPutOperation();
        try
        {
            referredPutOp.registerNewsRequest = registerNewsRequest;
            referredPutOp["on-complete"] = (Action<NewsPutOperation, HttpResponse>)((op, response) =>
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
        NewsAcceptPutOperation acceptPutOp = new NewsAcceptPutOperation();
        try
        {
            acceptPutOp.postModerationRequest = postModerationRequest;
            acceptPutOp["on-complete"] = (Action<NewsAcceptPutOperation, HttpResponse>)((op, response) =>
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
        NewsRejectPutOperation rejectPutOp = new NewsRejectPutOperation();
        try
        {
            rejectPutOp.postModerationRequest = postModerationRequest;
            rejectPutOp["on-complete"] = (Action<NewsRejectPutOperation, HttpResponse>)((op, response) =>
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