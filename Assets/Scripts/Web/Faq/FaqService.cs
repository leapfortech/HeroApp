using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using System.Collections.Generic;
using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class FaqService : MonoBehaviour
{
    [Serializable]
    public class FaqEvent : UnityEvent<Faq> { }
    [Serializable]
    public class AllEvent : UnityEvent<List<Faq>> { }

    [SerializeField]
    private FaqEvent onRetreived = null;

    [SerializeField]
    private AllEvent onAllRetreived = null;

    [SerializeField]
    private UnityLongEvent onRegistered = null;

    [SerializeField]
    private UnityLongEvent onUpdated = null;

    [SerializeField]
    private UnityBoolEvent onStatusUpdated = null;

    [Title("Errors")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;

    [SerializeField]
    private UnityStringEvent onTimeoutError = null;

    public void GetById(long id)
    {
        FaqGetOperation faqGetOp = new FaqGetOperation();
        try
        {
            faqGetOp.id = id;
            faqGetOp["on-complete"] = (Action<FaqGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRetreived.Invoke(op.faq);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            faqGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetAllByType(long faqTypeId)
    {
        FaqAllByTypeGetOperation AllByTypeGetOp = new FaqAllByTypeGetOperation();
        try
        {
            AllByTypeGetOp.faqTypeId = faqTypeId;
            AllByTypeGetOp["on-complete"] = (Action<FaqAllByTypeGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onAllRetreived.Invoke(op.faqs);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            AllByTypeGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void Register(Faq faq)
    {
        FaqRegisterOperation faqRegisterOp = new FaqRegisterOperation();
        try
        {
            faqRegisterOp.faq = faq;
            faqRegisterOp["on-complete"] = (Action<FaqRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt64(op.id));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            faqRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // UPDATE
    public void UpdateFaq(Faq faq)
    {
        FaqPutOperation faqPutOp = new FaqPutOperation();
        try
        {
            faqPutOp.faq = faq;
            faqPutOp["on-complete"] = (Action<FaqPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(Convert.ToInt64(op.id));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            faqPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void UpdateStatus(long id, int status)
    {
        UpdateStatusPutOperation updateStatusPutOp = new UpdateStatusPutOperation();
        try
        {
            updateStatusPutOp.id = id;
            updateStatusPutOp.status = status;
            updateStatusPutOp["on-complete"] = (Action<UpdateStatusPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onStatusUpdated.Invoke(op.response);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            updateStatusPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}