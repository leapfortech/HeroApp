using System;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;


public class ReserveRegisterAction : MonoBehaviour
{
    [Serializable]
    public class InvestmentPaymentInfoEvent : UnityEvent<InvestmentPaymentInfo> { }

    [Space]
    [Title("Action")]
    [SerializeField]
    Button btnStart = null;

    [Title("Event")]
    [SerializeField]
    InvestmentPaymentInfoEvent investmentPaymentInfo = null;

    [Title("Page")]
    [SerializeField]
    Page pagPaymentResume = null;
    [SerializeField]
    Page pagNext = null;

    [Title("Message")]
    [SerializeField]
    String reserveRegisteredTitle = "Paso Completo";
    [SerializeField, TextArea(2, 4)]
    String reserveRegisteredMessage = "La información fue guardada exitosamente, puedes continuar con el siguiente paso.";


    int investmentId = -1;
    bool isUpdate = false;

    private void Start()
    {
        btnStart?.AddAction(Register);
    }

    public void Clear()
    {
        investmentId = -1;
    }

    public void SetInvestmentRequest((int investmentId, bool isUpdate) investmentRequest)
    {
        this.investmentId = investmentRequest.investmentId;
        this.isUpdate = investmentRequest.isUpdate;
    }

    private void Register()
    {
        InvestmentFull investmentFull = StateManager.Instance.GetInvestmentById(investmentId);
        double reserveAmount = investmentFull.ReserveAmount;
        double balance = investmentFull.Balance;
        DateTime dueDate = investmentFull.EffectiveDate.AddDays(5);

        investmentPaymentInfo.Invoke(new InvestmentPaymentInfo(investmentId, 0, reserveAmount, dueDate, balance));

        PageManager.Instance.ChangePage(pagPaymentResume);
    }

    public void ApplyPayment()
    {
        StateManager.Instance.UpdateInvestmentStatus(investmentId, 2);

        ChoiceDialog.Instance.Info(reserveRegisteredTitle, reserveRegisteredMessage, () => PageManager.Instance.ChangePage(pagNext));
    }
}
