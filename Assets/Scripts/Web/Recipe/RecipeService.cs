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


    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;


    // GET
    public void GetFull(long id)
    {
        RecipeGetFullOperation recipeFullGetOp = new RecipeGetFullOperation();
        try
        {
            recipeFullGetOp.id = id;
            recipeFullGetOp["on-complete"] = (Action<RecipeGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.recipeFull);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            recipeFullGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFullByPostId(long postId)
    {
        RecipeFullByPostIdGetFullOperation recipeFullByPostIdGetOp = new RecipeFullByPostIdGetFullOperation();
        try
        {
            recipeFullByPostIdGetOp.postId = postId;
            recipeFullByPostIdGetOp["on-complete"] = (Action<RecipeFullByPostIdGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.recipeFull);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
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
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            recipeFullsGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void Register(RegisterRecipeRequest registerRecipeRequest)
    {
        RecipeRegisterOperation referredRegisterOp = new RecipeRegisterOperation();
        try
        {
            referredRegisterOp.registerRecipeRequest = registerRecipeRequest;
            referredRegisterOp["on-complete"] = (Action<RecipeRegisterOperation, HttpResponse>)((op, response) =>
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
    public void UpdateRecipe(RegisterRecipeRequest registerRecipeRequest)
    {
        RecipePutOperation referredPutOp = new RecipePutOperation();
        try
        {
            referredPutOp.registerRecipeRequest = registerRecipeRequest;
            referredPutOp["on-complete"] = (Action<RecipePutOperation, HttpResponse>)((op, response) =>
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
        RecipeAcceptPutOperation acceptPutOp = new RecipeAcceptPutOperation();
        try
        {
            acceptPutOp.postModerationRequest = postModerationRequest;
            acceptPutOp["on-complete"] = (Action<RecipeAcceptPutOperation, HttpResponse>)((op, response) =>
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
        RecipeRejectPutOperation rejectPutOp = new RecipeRejectPutOperation();
        try
        {
            rejectPutOp.postModerationRequest = postModerationRequest;
            rejectPutOp["on-complete"] = (Action<RecipeRejectPutOperation, HttpResponse>)((op, response) =>
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