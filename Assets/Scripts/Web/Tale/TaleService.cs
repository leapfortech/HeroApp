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
    public class TaleFullsEvent : UnityEvent<List<TaleFull>> { }

    [SerializeField]
    private TaleFullsEvent onRetreived = null;

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
        TaleGetFullsOperation taleFullsGetOp = new TaleGetFullsOperation();
        try
        {
            taleFullsGetOp.status = status;
            taleFullsGetOp["on-complete"] = (Action<TaleGetFullsOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRetreived.Invoke(op.taleFulls);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
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
        TaleRegisterOperation referredRegisterOp = new TaleRegisterOperation();
        try
        {
            referredRegisterOp.registerTaleRequest = registerTaleRequest;
            referredRegisterOp["on-complete"] = (Action<TaleRegisterOperation, HttpResponse>)((op, response) =>
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
    public void UpdateTale(Tale tale)
    {
        TalePutOperation referredPutOp = new TalePutOperation();
        try
        {
            referredPutOp.tale = tale;
            referredPutOp["on-complete"] = (Action<TalePutOperation, HttpResponse>)((op, response) =>
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