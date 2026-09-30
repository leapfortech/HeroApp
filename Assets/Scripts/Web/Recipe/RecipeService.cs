using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using System.Collections.Generic;
using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class RecipeService : MonoBehaviour
{
    [Serializable]
    public class RecipeFullEvent : UnityEvent<RecipeFull> { }

    [Serializable]
    public class RecipeFullsEvent : UnityEvent<List<RecipeFull>> { }

    [SerializeField]
    private RecipeFullEvent onFullRetreived = null;

    [SerializeField]
    private RecipeFullsEvent onFullsRetreived = null;

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
    public void GetFull(long id, long reactionAppUserId)
    {
        RecipeGetFullOperation recipeFullGetOp = new RecipeGetFullOperation();
        try
        {
            recipeFullGetOp.id = id;
            recipeFullGetOp.reactionAppUserId = reactionAppUserId;
            recipeFullGetOp["on-complete"] = (Action<RecipeGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.recipeFull);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            recipeFullGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFullByPostId(long postId, long reactionAppUserId)
    {
        RecipeFullByPostIdGetFullOperation recipeFullByPostIdGetOp = new RecipeFullByPostIdGetFullOperation();
        try
        {
            recipeFullByPostIdGetOp.postId = postId;
            recipeFullByPostIdGetOp.reactionAppUserId=reactionAppUserId;
            recipeFullByPostIdGetOp["on-complete"] = (Action<RecipeFullByPostIdGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.recipeFull);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            recipeFullByPostIdGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFulls(int status)
    {
        RecipeGetFullsOperation recipeFullsGetOp = new RecipeGetFullsOperation();
        try
        {
            recipeFullsGetOp.status = status;
            recipeFullsGetOp["on-complete"] = (Action<RecipeGetFullsOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullsRetreived.Invoke(op.recipeFulls);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            recipeFullsGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void Register(RecipeFull recipeFull)
    {
        RecipeRegisterOperation recipeRegisterOp = new RecipeRegisterOperation();
        try
        {
            recipeRegisterOp.recipeFull = recipeFull;
            recipeRegisterOp["on-complete"] = (Action<RecipeRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt64(op.id));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            recipeRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // UPDATE
    public void UpdateRecipe(RecipeFull recipeFull)
    {
        RecipePutOperation referredPutOp = new RecipePutOperation();
        try
        {
            referredPutOp.recipeFull = recipeFull;
            referredPutOp["on-complete"] = (Action<RecipePutOperation, HttpResponse>)((op, response) =>
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
        RecipeAcceptPutOperation acceptPutOp = new RecipeAcceptPutOperation();
        try
        {
            acceptPutOp.postModerationRequest = postModerationRequest;
            acceptPutOp["on-complete"] = (Action<RecipeAcceptPutOperation, HttpResponse>)((op, response) =>
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
        RecipeRejectPutOperation rejectPutOp = new RecipeRejectPutOperation();
        try
        {
            rejectPutOp.postModerationRequest = postModerationRequest;
            rejectPutOp["on-complete"] = (Action<RecipeRejectPutOperation, HttpResponse>)((op, response) =>
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