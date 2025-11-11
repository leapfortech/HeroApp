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

    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;

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
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            appParamOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}
