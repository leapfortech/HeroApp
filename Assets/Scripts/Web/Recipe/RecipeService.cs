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
    public class RecipeFullsEvent : UnityEvent<List<RecipeFull>> { }

    [SerializeField]
    private RecipeFullsEvent onRetreived = null;

    [SerializeField]
    private UnityLongEvent onRegistered = null;

    [SerializeField]
    private UnityBoolEvent onUpdated = null;


    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;


    // GET
    public void GetFulls(int status)
    {
        RecipeGetFullsOperation recipeFullsGetOp = new RecipeGetFullsOperation();
        try
        {
            recipeFullsGetOp.status = status;
            recipeFullsGetOp["on-complete"] = (Action<RecipeGetFullsOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRetreived.Invoke(op.recipeFulls);
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
    public void UpdateRecipe(Recipe recipe)
    {
        RecipePutOperation referredPutOp = new RecipePutOperation();
        try
        {
            referredPutOp.recipe = recipe;
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
}