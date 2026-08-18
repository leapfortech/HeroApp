using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit;
using hg.ApiWebKit.core.http;

using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class AppParamService : MonoBehaviour
{
    [Serializable]
    public class UnityAppParamsEvent : UnityEvent<AppParam[]> { }

    [SerializeField]
    private UnityAppParamsEvent onAppParams = null;

    [Title("Errors")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;

    [SerializeField]
    private UnityStringEvent onTimeoutError = null;

    // Get
    public void GetParams()
    {
        AppParamGetOperation appParamOp = new AppParamGetOperation();
        try
        {
            appParamOp["on-complete"] = (Action<AppParamGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && response.StatusCode == HttpStatusCode.OK)
                    onAppParams.Invoke(op.appParams);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            appParamOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}
