using System;
using UnityEngine;

using hg.ApiWebKit.core.http;

using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class ServiceWishService : MonoBehaviour
{
    [SerializeField]
    private UnityLongEvent onRegistered = null;

    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;

    // REGISTER
    public void Register(ServiceWish serviceWish)
    {
        ServiceWishRegisterOperation serviceWishRegisterOp = new ServiceWishRegisterOperation();
        try
        {
            serviceWishRegisterOp.serviceWish = serviceWish;
            serviceWishRegisterOp["on-complete"] = (Action<ServiceWishRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt64(op.id));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            serviceWishRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}