using System;

using UnityEngine;
using UnityEngine.Events;

using Leap.Core.Tools;
using Leap.Core.Debug;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class NotificationReceiver: MonoBehaviour
{
    [Title("AppUser")]
    [SerializeField]
    UnityStringsEvent onRemoteLogin = null;

    [SerializeField]
    UnityEvent onAppUserLocked = null;

    [Title("Onboarding")]
    [SerializeField]
    UnityIntEvent onObdRequest = null;

    [SerializeField]
    UnityIntEvent onObdFinalize = null;

    [Title("InvestmentPayment")]
    [SerializeField]
    UnityIntEvent onPaymentAccepted = null;

    [SerializeField]
    UnityIntEvent onPaymentRejected = null;

    [SerializeField]
    UnityIntEvent onPaymentReceiptRejected = null;

    [Title("Investment")]
    [SerializeField]
    UnityIntEvent onInvRequest = null;

    [SerializeField]
    UnityIntEvent onInvFinalize = null;

    [Title("Notification")]
    [SerializeField]
    UnityIntEvent onNotification = null;

    public void SetLastNotification(LoginAppInfo loginData)
    {
        NotificationManager.Instance.SetLastNotification(loginData.Notifications);
    }

    public void OnNotificationData(FirebaseData data)
    {
        if (DebugManager.Instance.DebugEnabled)
        {
            Debug.Log("-------------------------------------------------------");
            Debug.Log("WebSysUserId : " + data.WebSysUserId);
            Debug.Log("Action : " + data.Action);
            Debug.Log("Information : " + data.Information);
            Debug.Log("Parameter : " + data.Parameter);
            Debug.Log("DisplayMode : " + data.DisplayMode);
            Debug.Log("-------------------------------------------------------");
        }

        int webSysUserId = int.Parse(data.WebSysUserId);

        bool bValid = data.DisplayMode == "1";

        if (data.Action == "RemoteLogin")
        {
            onRemoteLogin.Invoke(data.Information.Split('^'));  // with Title
        }
        else if (data.Action == "PersonLocking")
        {
            if (!bValid)
                onAppUserLocked.Invoke();
            //else
            //    onPersonUnblocked.Invoke();
        }
        else if (data.Action == "Onboarding")
        {
            if (data.Information == "Request")
            {
                onObdRequest.Invoke(Convert.ToInt32(data.Parameter));
            }
            else if (data.Information == "Finalize")
            {
                onObdFinalize.Invoke(Convert.ToInt32(data.Parameter));    // 2:OK    4:NOK
            }
        }
        else if (data.Action == "InvestmentPayment")
        {
            if (data.Information == "Accept")
            {
                onPaymentAccepted.Invoke(Convert.ToInt32(data.Parameter));
            }
            else if (data.Information == "Reject")
            {
                onPaymentRejected.Invoke(Convert.ToInt32(data.Parameter));
            }
            else if (data.Information == "RejectReceipt")
            {
                onPaymentReceiptRejected.Invoke(Convert.ToInt32(data.Parameter));
            }
        }
        else if (data.Action == "Investment")
        {
            if (data.Information == "Request")
            {
                onInvRequest.Invoke(Convert.ToInt32(data.Parameter)); //{investmentId}
            }
            else if (data.Information == "Finalize")
            {
                onInvFinalize.Invoke(Convert.ToInt32(data.Parameter)); // {investmentId}
            }
        }

        onNotification.Invoke(webSysUserId);
    }

    private int[] ConvertStringtoIntArray(String data)
    {
        String[] parameters = data.Split('^');
        int[] paramIds = new int[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
            paramIds[i] = Convert.ToInt32(parameters[i]);

        return paramIds;
    }
}