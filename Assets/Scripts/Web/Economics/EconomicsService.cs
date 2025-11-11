using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class EconomicsService : MonoBehaviour
{
    [Serializable]
    public class EconomicsInfoEvent : UnityEvent<EconomicsInfo> { }


    [SerializeField]
    private EconomicsInfoEvent onInfoRetreived = null;

    [SerializeField]
    private UnityIntEvent onAdded = null;

    [SerializeField]
    private UnityIntsEvent onRegistered = null;

    [SerializeField]
    private UnityIntsEvent onUpdated = null;

    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;


    // GET
    public void GetEconomicsInfo(int investmentId)
    {
        EconomicsInfoGetOperation economicsInfoGetOp = new EconomicsInfoGetOperation();
        try
        {
            economicsInfoGetOp.investmentId = investmentId;
            economicsInfoGetOp["on-complete"] = (Action<EconomicsInfoGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onInfoRetreived.Invoke(op.economicsInfo);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            economicsInfoGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // ADD
    public void Add(Income income)
    {
        IncomePostOperation incomePostOp = new IncomePostOperation();
        try
        {
            incomePostOp.income = income;
            incomePostOp["on-complete"] = (Action<IncomePostOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onAdded.Invoke(Convert.ToInt32(op.id));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            incomePostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void Register(EconomicsInfo economicsInfo)
    {
        EconomicsRegisterOperation incomeRegisterPostOp = new EconomicsRegisterOperation();
        try
        {
            incomeRegisterPostOp.economicsInfo = economicsInfo;
            incomeRegisterPostOp["on-complete"] = (Action<EconomicsRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(op.infoIds);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            incomeRegisterPostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // UPDATE
    public void UpdateEconomics(EconomicsInfo economicsInfo)
    {
        EconomicsUpdateOperation incomeUpdatePostOp = new EconomicsUpdateOperation();
        try
        {
            incomeUpdatePostOp.economicsInfo = economicsInfo;
            incomeUpdatePostOp["on-complete"] = (Action<EconomicsUpdateOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(op.infoIds);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            incomeUpdatePostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}
