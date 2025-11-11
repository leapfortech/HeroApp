using System;

using System.Globalization;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;

using Sirenix.OdinInspector;


public class InvestmentPaymentAction : MonoBehaviour
{
    [Serializable]
    public class InvestmentPaymentInfoEvent : UnityEvent<InvestmentPaymentInfo> { }

    [Title("Fields")]
    [SerializeField]
    Text txtAmount = null;
    [SerializeField]
    Text txtAmountGTQ = null;
    [SerializeField]
    Text txtDueDate = null;

    [Title("Action")]
    [SerializeField]
    Button btnDone = null;

    [Title("Action")]
    [SerializeField]
    private InvestmentPaymentInfoEvent onPaymentInfoDisplayed = null;

    [Title("Page")]
    [SerializeField]
    Page pagPaymentResume;

    Page pagNext;

    private void Start()
    {
        btnDone?.AddAction(NextPage);
    }

    public void Clear()
    {
        txtAmount.Clear();
        txtDueDate.Clear();
    }

    public void SetNextPage (Page pag)
    {
        pagNext = pag;
    }
    
    public void DisplayResume(InvestmentPaymentInfo investmentPaymentInfo)
    {
        Clear();

        txtAmount.TextValue = "$" + investmentPaymentInfo.Amount.ToString("N2", CultureInfo.InvariantCulture);
        txtAmountGTQ.TextValue = "Q" + (investmentPaymentInfo.Amount * Convert.ToDouble(AppManager.Instance.GetParamValue("ExchangeRate"))).ToString("N2", CultureInfo.InvariantCulture);
        txtDueDate.TextValue = $"A pagar antes del: {investmentPaymentInfo.DueDate.ToString("dd/MM/yyyy")}";

        onPaymentInfoDisplayed.Invoke(investmentPaymentInfo);
    }

    public Page PagPaymentResumeBack
    {
        get => pagPaymentResume.HeaderPage;
        set => pagPaymentResume.HeaderPage = value;
    }

    private void NextPage()
    {
        PageManager.Instance.ChangePage(pagNext);
    }
}