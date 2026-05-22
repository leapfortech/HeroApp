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
    public class NewsFullEvent : UnityEvent<NewsFull> { }

    [Serializable]
    public class NewsFullsEvent : UnityEvent<List<NewsFull>> { }

    [SerializeField]
    private NewsFullEvent onFullRetreived = null;

    [SerializeField]
    private NewsFullsEvent onFullsRetreived = null;

    [SerializeField]
    private UnityLongEvent onRegistered = null;

    [SerializeField]
    private UnityBoolEvent onUpdated = null;


    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;


    // GET
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
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
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
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
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
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
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
    public void UpdateNews(RegisterNewsRequest registerNewsRequest)
    {
        NewsPutOperation referredPutOp = new NewsPutOperation();
        try
        {
            referredPutOp.registerNewsRequest = registerNewsRequest;
            referredPutOp["on-complete"] = (Action<NewsPutOperation, HttpResponse>)((op, response) =>
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
        NewsAcceptPutOperation acceptPutOp = new NewsAcceptPutOperation();
        try
        {
            acceptPutOp.postModerationRequest = postModerationRequest;
            acceptPutOp["on-complete"] = (Action<NewsAcceptPutOperation, HttpResponse>)((op, response) =>
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
        NewsRejectPutOperation rejectPutOp = new NewsRejectPutOperation();
        try
        {
            rejectPutOp.postModerationRequest = postModerationRequest;
            rejectPutOp["on-complete"] = (Action<NewsRejectPutOperation, HttpResponse>)((op, response) =>
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