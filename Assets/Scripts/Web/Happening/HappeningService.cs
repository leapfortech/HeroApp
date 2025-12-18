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
    public class HappeningFullsEvent : UnityEvent<List<HappeningFull>> { }

    [SerializeField]
    private HappeningFullsEvent onRetreived = null;

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
        HappeningGetFullsOperation happeningFullsGetOp = new HappeningGetFullsOperation();
        try
        {
            happeningFullsGetOp.status = status;
            happeningFullsGetOp["on-complete"] = (Action<HappeningGetFullsOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRetreived.Invoke(op.happeningFulls);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
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
        HappeningRegisterOperation referredRegisterOp = new HappeningRegisterOperation();
        try
        {
            referredRegisterOp.registerHappeningRequest = registerHappeningRequest;
            referredRegisterOp["on-complete"] = (Action<HappeningRegisterOperation, HttpResponse>)((op, response) =>
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
    public void UpdateHappening(Happening happening)
    {
        HappeningPutOperation referredPutOp = new HappeningPutOperation();
        try
        {
            referredPutOp.happening = happening;
            referredPutOp["on-complete"] = (Action<HappeningPutOperation, HttpResponse>)((op, response) =>
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