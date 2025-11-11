using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;
using hg.LitJson;

using Leap.Core.Tools;
using Leap.Data.Web;
using Leap.Finance.Payment;

using Sirenix.OdinInspector;

public class CardService : MonoBehaviour
{
    [Serializable]
    public class BillToEvent : UnityEvent<CSBillTo> { }

    [Serializable]
    public class CardsEvent : UnityEvent<Card[]> { }

    [Serializable]
    public class CardEvent : UnityEvent<Card> { }

    [Serializable]
    public class CSErrorsEvent : UnityEvent<CSErrors> { }

    [SerializeField]
    private BillToEvent onBillToRetreived = null;

    [SerializeField]
    private CardEvent onCardRetreived = null;

    [SerializeField]
    private CardEvent onCardRegistered = null;

    [SerializeField]
    private UnityEvent onCardStatusChanged = null;

    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;

    [SerializeField]
    private CSErrorsEvent onCybersourceError = null;

    // BillTo
    public void GetBillTo(int appUserId)
    {
        BillToOperation billToOp = new BillToOperation();
        try
        {
            billToOp.appUserId = appUserId;
            billToOp["on-complete"] = (Action<BillToOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onBillToRetreived.Invoke(new CSBillTo(op.billToCreate));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            billToOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // Card
    public void GetCard()
    {
        CardCustomerOperation cardCustomerOp = new CardCustomerOperation();
        try
        {
            cardCustomerOp.appUserId = StateManager.Instance.AppUser.Id;
            cardCustomerOp["on-complete"] = (Action<CardCustomerOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onCardRetreived.Invoke(op.card);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            cardCustomerOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // Register
    public void RegisterCard(CardRegister cardRegister)
    {
        CardRegisterOperation cardRegisterOp = new CardRegisterOperation();
        try
        {
            cardRegisterOp.cardRegister = cardRegister;
            cardRegisterOp["on-complete"] = (Action<CardRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                {
                    onCardRegistered.Invoke(op.card);
                }
                else if (response.Text.StartsWith("\"CBSC|"))
                {
                    try
                    {
                        String errors = response.Text.Substring(response.Text.IndexOf('{'));
                        errors = Regex.Unescape(errors.Substring(0, errors.Length - 1));
                        onCybersourceError.Invoke(JsonMapper.ToObject<CSErrors>(errors));
                    }
                    catch
                    {
                        onResponseError.Invoke(response.Text.Substring(response.Text.IndexOf('|') + 1));
                    }
                }
                else
                {
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
                }
            });
            cardRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // Status
    public void SetStatus(int id, int status)
    {
        CardSetStatusOperation cardStatusOp = new CardSetStatusOperation();
        try
        {
            cardStatusOp.id = id;
            cardStatusOp.status = status;
            cardStatusOp["on-complete"] = (Action<CardSetStatusOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onCardStatusChanged.Invoke();
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            cardStatusOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}
