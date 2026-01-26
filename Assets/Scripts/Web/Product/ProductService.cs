using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using System.Collections.Generic;
using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class ProductService : MonoBehaviour
{
    [Serializable]
    public class ProductFullEvent : UnityEvent<ProductFull> { }

    [Serializable]
    public class ProductFullsEvent : UnityEvent<List<ProductFull>> { }

    [SerializeField]
    private ProductFullEvent onFullRetreived = null;

    [SerializeField]
    private ProductFullsEvent onFullsRetreived = null;

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
        ProductGetFullOperation productFullGetOp = new ProductGetFullOperation();
        try
        {
            productFullGetOp.id = id;
            productFullGetOp["on-complete"] = (Action<ProductGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.productFull);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            productFullGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFullByPostId(long postId)
    {
        ProductFullByPostIdGetFullOperation productFullByPostIdGetOp = new ProductFullByPostIdGetFullOperation();
        try
        {
            productFullByPostIdGetOp.postId = postId;
            productFullByPostIdGetOp["on-complete"] = (Action<ProductFullByPostIdGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.productFull);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            productFullByPostIdGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFulls(int status)
    {
        ProductGetFullsOperation productFullsGetOp = new ProductGetFullsOperation();
        try
        {
            productFullsGetOp.status = status;
            productFullsGetOp["on-complete"] = (Action<ProductGetFullsOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullsRetreived.Invoke(op.productFulls);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            productFullsGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void Register(RegisterProductRequest registerProductRequest)
    {
        ProductRegisterOperation productRegisterOp = new ProductRegisterOperation();
        try
        {
            productRegisterOp.registerProductRequest = registerProductRequest;
            productRegisterOp["on-complete"] = (Action<ProductRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt64(op.id));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            productRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void RegisterReview(ProductReview productReview)
    {
        ReviewRegisterOperation reviewRegisterOp = new ReviewRegisterOperation();
        try
        {
            reviewRegisterOp.productReview = productReview;
            reviewRegisterOp["on-complete"] = (Action<ReviewRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt64(op.id));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            reviewRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // UPDATE
    public void UpdateProduct(RegisterProductRequest registerProductRequest)
    {
        ProductPutOperation referredPutOp = new ProductPutOperation();
        try
        {
            referredPutOp.registerProductRequest = registerProductRequest;
            referredPutOp["on-complete"] = (Action<ProductPutOperation, HttpResponse>)((op, response) =>
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
        ProductAcceptPutOperation acceptPutOp = new ProductAcceptPutOperation();
        try
        {
            acceptPutOp.postModerationRequest = postModerationRequest;
            acceptPutOp["on-complete"] = (Action<ProductAcceptPutOperation, HttpResponse>)((op, response) =>
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
        ProductRejectPutOperation rejectPutOp = new ProductRejectPutOperation();
        try
        {
            rejectPutOp.postModerationRequest = postModerationRequest;
            rejectPutOp["on-complete"] = (Action<ProductRejectPutOperation, HttpResponse>)((op, response) =>
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