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

    [Title("Notification")]
    [SerializeField]
    UnityLongEvent onNotification = null;

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

        long webSysUserId = long.Parse(data.WebSysUserId);

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