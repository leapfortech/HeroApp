using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using System.Collections.Generic;
using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class RadioService : MonoBehaviour
{
    [Serializable]
    public class RadioFullsEvent : UnityEvent<List<RadioFull>> { }

    [SerializeField]
    private RadioFullsEvent onRetreived = null;

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
        RadioGetFullsOperation radioFullsGetOp = new RadioGetFullsOperation();
        try
        {
            radioFullsGetOp.status = status;
            radioFullsGetOp["on-complete"] = (Action<RadioGetFullsOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRetreived.Invoke(op.radioFulls);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            radioFullsGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void Register(RegisterRadioRequest registerRadioRequest)
    {
        RadioRegisterOperation referredRegisterOp = new RadioRegisterOperation();
        try
        {
            referredRegisterOp.registerRadioRequest = registerRadioRequest;
            referredRegisterOp["on-complete"] = (Action<RadioRegisterOperation, HttpResponse>)((op, response) =>
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

    public void RegisterRadioListen(RadioListen radioListen)
    {
        RadioListenRegisterOperation radioListenRegisterOp = new RadioListenRegisterOperation();
        try
        {
            radioListenRegisterOp.radioListen = radioListen;
            radioListenRegisterOp["on-complete"] = (Action<RadioListenRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt64(op.radioListenId));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            radioListenRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // UPDATE
    public void UpdateRadio(RegisterRadioRequest registerRadioRequest)
    {
        RadioPutOperation referredPutOp = new RadioPutOperation();
        try
        {
            referredPutOp.registerRadioRequest = registerRadioRequest;
            referredPutOp["on-complete"] = (Action<RadioPutOperation, HttpResponse>)((op, response) =>
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