using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using System.Collections.Generic;
using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class NewsService : MonoBehaviour
{
    [Serializable]
    public class NewsInfosEvent : UnityEvent<List<NewsInfo>> { }


    [SerializeField]
    private NewsInfosEvent onRetreived = null;


    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;


    // GET
    public void GetNewsInfos(int status)
    {
        NewsInfoGetOperation newsInfosGetOp = new NewsInfoGetOperation();
        try
        {
            newsInfosGetOp.status = status;
            newsInfosGetOp["on-complete"] = (Action<NewsInfoGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRetreived.Invoke(op.newsInfos);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            newsInfosGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}
