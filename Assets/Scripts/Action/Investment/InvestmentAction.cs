using System;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.Graphics.Tools;
using Leap.Data.Collections;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;


public class InvestmentAction : MonoBehaviour
{
    [Serializable]
    public class InvestmentPaymentInfoEvent : UnityEvent<InvestmentPaymentInfo> { }

    [Space]
    [SerializeField]
    String[] productNames = null;
    [Space]
    [SerializeField]
    String[] investmentNames = null;

    [Title("Investment")]
    [SerializeField, Space]
    String[] investmentStatusTitles;
    [SerializeField, Space]
    ListScroller lstInvestment = null;
    [SerializeField]
    Text txtEmpty = null;

    [Title("Details")]
    [SerializeField]
    Text txtProjectName = null;
    [SerializeField]
    Image imgCover = null;
    [SerializeField]
    Text txtProductType = null;
    [SerializeField]
    Text txtStatus = null;
    [SerializeField]
    FieldValue fldRemainingTerm = null;
    [SerializeField]
    FieldValue fldCpiCount = null;
    [SerializeField]
    FieldValue fldTotalAmount = null;
    [SerializeField]
    FieldValue fldDiscountAmount = null;

    [Title("Installments")]
    [SerializeField, Space]
    String[] installmentStatusTitles = null;
    [SerializeField, Space]
    ValueList vllInvestmentPaymentType = null;

    [SerializeField]
    ListScroller lstInstallment = null;
    [SerializeField]
    ListScroller lstInstallmentPayment = null;
    [SerializeField]
    Text txtInstallmentPaymentEmpty = null;

    [SerializeField, Space]
    ListScroller lstAppliedPayment = null;
    [SerializeField]
    Text txtAppliedPaymentEmpty = null;

    [Title("Payments")]
    [SerializeField, Space]
    String[] paymentStatusTitles = null;
    [SerializeField, Space]
    ListScroller lstPayment = null;
    [SerializeField]
    Text txtPaymentEmpty = null;
    [SerializeField]
    Button btnPayment = null;

    [SerializeField]
    ValueList vllAccountHpb = null;
    [SerializeField]
    ValueList vllCurrency = null;

    [SerializeField, TextArea(2, 4)]
    String msgRejected = "Tu transacción fue rechazada por {0}.";

    [Title("BankTransaction")]
    [SerializeField]
    Image imgReceipt = null;

    [Space]
    [Title("Event")]
    [SerializeField]
    InvestmentPaymentInfoEvent investmentPaymentInfo = null;

    [Title("Pages")]
    [SerializeField]
    Page pagInvestmentInstallment = null;
    [SerializeField]
    Page pagPaymentResume = null;

    [Title("Messages")]
    [SerializeField]
    String paymentRegisteredTitle = "Pago";
    [SerializeField, TextArea(2, 4)]
    String paymentRegisteredMessage = "Pago registrado satisfactoriamente.";

    InvestmentService investmentService = null;

    List<(InvestmentFull InvestmentFull, ProjectFull ProjectFull)> investmentProjects = new();
    int idxInvestment = 0, idxInstallment = 0, idxInstallmentPayment, idxPayment;

    private void Awake()
    {
        investmentService = GetComponent<InvestmentService>();
    }

    public void Clear()
    {
        investmentProjects.Clear();
        lstInvestment.ClearValues();
        lstInstallment.ClearValues();
        imgReceipt.Clear();
    }

    public void Refresh()
    {
        DisplayInvestments();
    }


    // Investment
    public void DisplayInvestments()
    {
        investmentProjects = StateManager.Instance.GetInvestmentsByStatus(InvestmentStatus.Approved);

        if (investmentProjects.Count == 0)
        {
            lstInvestment.Clear();
            txtEmpty.gameObject.SetActive(true);
            return;
        }

        lstInvestment.Clear();

        txtEmpty.gameObject.SetActive(false);

        for (int i = 0; i < investmentProjects.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(8, true);
            scrollerValue.SetText(0, productNames[investmentProjects[i].InvestmentFull.ProductTypeId - 1]);
            scrollerValue.SetText(1, investmentProjects[i].ProjectFull.Name);
            scrollerValue.SetText(2, "$" + investmentProjects[i].InvestmentFull.TotalAmount.ToString("N2", CultureInfo.InvariantCulture));  // investmentProjects[i].InvestmentFull.CpiCount + " CPI's    $" + 
            scrollerValue.SetText(3, "");
            scrollerValue.SetText(4, investmentStatusTitles[investmentProjects[i].InvestmentFull.InvestmentStatusId]);
            scrollerValue.SetText(5, "");
            scrollerValue.SetText(6, "");
            scrollerValue.SetSprite(7, investmentProjects[i].ProjectFull.CoverSprite);

            lstInvestment.AddValue(scrollerValue);
        }
        lstInvestment.ApplyValues();
    }

    public void Select(int idx)
    {
        this.idxInvestment = idx;
    }

    public void DisplayDetails()
    {
        txtProjectName.TextValue = investmentProjects[idxInvestment].ProjectFull.Name;
        imgCover.Sprite = investmentProjects[idxInvestment].ProjectFull.CoverSprite;
        txtProductType.TextValue = productNames[investmentProjects[idxInvestment].InvestmentFull.ProductTypeId - 1];
        fldCpiCount.TextValue = investmentProjects[idxInvestment].InvestmentFull.CpiCount.ToString() + " CPI's";
        fldTotalAmount.TextValue = "$" + investmentProjects[idxInvestment].InvestmentFull.TotalAmount.ToString("N2");

        // term
        int investmentTerm = CalculateInvestmentTerm(investmentProjects[idxInvestment].ProjectFull.StartDate, investmentProjects[idxInvestment].ProjectFull.DevelopmentTerm);
        
        txtStatus.TextValue = investmentNames[investmentTerm != 0 ? 0 : 1];
        
        fldRemainingTerm.TextValue = investmentTerm.ToString() + (investmentTerm == 1 ? " mes" : " meses");

        fldDiscountAmount.TextValue = "$" + investmentProjects[idxInvestment].InvestmentFull.DiscountAmount.ToString("N2");
        fldDiscountAmount.gameObject.SetActive(investmentProjects[idxInvestment].InvestmentFull.ProductTypeId == 1 || investmentProjects[idxInvestment].InvestmentFull.ProductTypeId == 3);
    }

    public void RefreshPayments()
    {
        DisplayInstallments();
        DisplayPayments();
        DisplayAppliedPayments();
    }

    // Installment

    List<InvestmentInstallmentInfo> installmentInfos = new List<InvestmentInstallmentInfo>();
    public void DisplayInstallments()
    {
        lstInstallment.Clear();

        if (investmentProjects[idxInvestment].InvestmentFull.ProductTypeId == 1)
            installmentInfos = (StateManager.Instance.GetInvestmentFractionatedFullByInvestmentId(investmentProjects[idxInvestment].InvestmentFull.InvestmentId).InvestmentInstallmentInfos);
        else if (investmentProjects[idxInvestment].InvestmentFull.ProductTypeId == 2)
            installmentInfos = (StateManager.Instance.GetInvestmentFinancedFullByInvestmentId(investmentProjects[idxInvestment].InvestmentFull.InvestmentId).InvestmentInstallmentInfos);
        else
            installmentInfos = (StateManager.Instance.GetInvestmentPrepaidFullByInvestmentId(investmentProjects[idxInvestment].InvestmentFull.InvestmentId).InvestmentInstallmentInfos);

        for (int i = 0; i < installmentInfos.Count; i++)
        {
            String investmentType = vllInvestmentPaymentType.GetRecordCellString(installmentInfos[i].InvestmentInstallment.InvestmentPaymentTypeId, "Name");

            if (installmentInfos[i].InvestmentInstallment.InvestmentPaymentTypeId == 1 || installmentInfos[i].InvestmentInstallment.InvestmentPaymentTypeId == 2)
                investmentType += $" #{i.ToString()}";

            ListScrollerValue scrollerValue = new ListScrollerValue(9, true);
            scrollerValue.SetText(0, installmentInfos[i].InvestmentInstallment.DueDate.ToString("MMMM dd, yyyy", StateManager.Instance.CultureInfo));
            scrollerValue.SetText(1, investmentType);

            
            if (installmentInfos[i].InvestmentInstallment.Status == 0)
            {
                scrollerValue.SetText(2, "");
                scrollerValue.SetText(3, "");
                scrollerValue.SetText(4, "");
                scrollerValue.SetText(5, installmentStatusTitles[installmentInfos[i].InvestmentInstallment.Status]);
            }
            else if (installmentInfos[i].InvestmentInstallment.Status == 1 && DateTime.Now > installmentInfos[i].InvestmentInstallment.DueDate)
            {
                scrollerValue.SetText(2, "");
                scrollerValue.SetText(3, "");
                scrollerValue.SetText(4, "");
                scrollerValue.SetText(5, installmentStatusTitles[installmentInfos[i].InvestmentInstallment.Status]);
            }
            else if (installmentInfos[i].InvestmentInstallment.Status == 1 && DateTime.Now <= installmentInfos[i].InvestmentInstallment.DueDate)
            {
                scrollerValue.SetText(2, installmentStatusTitles[installmentInfos[i].InvestmentInstallment.Status]);
                scrollerValue.SetText(3, "");
                scrollerValue.SetText(4, "");
                scrollerValue.SetText(5, "");
            }
            else if (installmentInfos[i].InvestmentInstallment.Status == 2)
            {
                scrollerValue.SetText(2, "");
                scrollerValue.SetText(3, installmentStatusTitles[installmentInfos[i].InvestmentInstallment.Status]);
                scrollerValue.SetText(4, "");
                scrollerValue.SetText(5, "");
            }
            else if (installmentInfos[i].InvestmentInstallment.Status == 3 && DateTime.Now <= installmentInfos[i].InvestmentInstallment.DueDate)
            {
                scrollerValue.SetText(2, "");
                scrollerValue.SetText(3, "");
                scrollerValue.SetText(4, installmentStatusTitles[installmentInfos[i].InvestmentInstallment.Status]);
                scrollerValue.SetText(5, "");
            }
            else if (installmentInfos[i].InvestmentInstallment.Status == 3 && DateTime.Now > installmentInfos[i].InvestmentInstallment.DueDate)
            {
                scrollerValue.SetText(2, "");
                scrollerValue.SetText(3, "");
                scrollerValue.SetText(4, "");
                scrollerValue.SetText(5, installmentStatusTitles[installmentInfos[i].InvestmentInstallment.Status]);
            }

            scrollerValue.SetText(6, "Monto $" + installmentInfos[i].InvestmentInstallment.Amount.ToString("N2", CultureInfo.InvariantCulture));
            scrollerValue.SetText(7, "Saldo $" + installmentInfos[i].InvestmentInstallment.Balance.ToString("N2", CultureInfo.InvariantCulture));
            scrollerValue.SetText(8, "Tipo de cambio " + AppManager.Instance.GetParamValue("ExchangeRate"));

            lstInstallment.AddValue(scrollerValue);
        }
        lstInstallment.ApplyValues();

        btnPayment.Interactable = GetInvestmentPaymentInfo() != null;
    }

    public void SelectInstallment(int idx)
    {
        this.idxInstallment = idx;
    }

    // InstallmentPayments
    public void DisplayInstallmentPayments()
    {
        lstInstallmentPayment.Clear();

        txtInstallmentPaymentEmpty.gameObject.SetActive(installmentInfos[idxInstallment].InvestmentInstallmentPayments.Count == 0);
        if (installmentInfos[idxInstallment].InvestmentInstallmentPayments.Count == 0)
            return;

        if (investmentProjects[idxInvestment].InvestmentFull.ProductTypeId == 1)
            installmentInfos = (StateManager.Instance.GetInvestmentFractionatedFullByInvestmentId(investmentProjects[idxInvestment].InvestmentFull.InvestmentId).InvestmentInstallmentInfos);
        else if (investmentProjects[idxInvestment].InvestmentFull.ProductTypeId == 2)
            installmentInfos = (StateManager.Instance.GetInvestmentFinancedFullByInvestmentId(investmentProjects[idxInvestment].InvestmentFull.InvestmentId).InvestmentInstallmentInfos);
        else
            installmentInfos = (StateManager.Instance.GetInvestmentPrepaidFullByInvestmentId(investmentProjects[idxInvestment].InvestmentFull.InvestmentId).InvestmentInstallmentInfos);

        for (int i = 0; i < installmentInfos[idxInstallment].InvestmentInstallmentPayments.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(4, true);

            scrollerValue.SetText(0, installmentInfos[idxInstallment].InvestmentInstallmentPayments[i].CreateDateTime.ToString("MMMM dd, yyyy", StateManager.Instance.CultureInfo));
            scrollerValue.SetText(1, "Monto $" + installmentInfos[idxInstallment].InvestmentInstallmentPayments[i].Amount.ToString("N2", CultureInfo.InvariantCulture));
            scrollerValue.SetText(2, "Saldo $" + installmentInfos[idxInstallment].InvestmentInstallmentPayments[i].NewBalance.ToString("N2", CultureInfo.InvariantCulture));
            scrollerValue.SetText(3, "Tipo de cambio " + AppManager.Instance.GetParamValue("ExchangeRate"));

            lstInstallmentPayment.AddValue(scrollerValue);
        }
        lstInstallmentPayment.ApplyValues();
    }

    public void SelectInstallmentPayment(int idx)
    {
        this.idxInstallmentPayment = idx;
    }

    public void DisplayAppliedPayments()
    {
        // Global

        List<InvestmentBankPayment> investmentBankPayments = new List<InvestmentBankPayment>();
        List<InvestmentInstallmentInfo> investmentInstallmentInfos = new List<InvestmentInstallmentInfo>();

        if (investmentProjects[idxInvestment].InvestmentFull.ProductTypeId == 1)
        {
            InvestmentFractionatedFull investmentFractionatedFull = StateManager.Instance.GetInvestmentFractionatedFullByInvestmentId(investmentProjects[idxInvestment].InvestmentFull.InvestmentId);
            investmentBankPayments = investmentFractionatedFull.InvestmentBankPayments;
            investmentInstallmentInfos = investmentFractionatedFull.InvestmentInstallmentInfos;
        }
        else if (investmentProjects[idxInvestment].InvestmentFull.ProductTypeId == 2)
        {
            InvestmentFinancedFull investmentFinancedFull = StateManager.Instance.GetInvestmentFinancedFullByInvestmentId(investmentProjects[idxInvestment].InvestmentFull.InvestmentId);
            investmentBankPayments = investmentFinancedFull.InvestmentBankPayments;
            investmentInstallmentInfos = investmentFinancedFull.InvestmentInstallmentInfos;
        }
        else
        {
            InvestmentPrepaidFull investmentPrepaidFull = StateManager.Instance.GetInvestmentPrepaidFullByInvestmentId(investmentProjects[idxInvestment].InvestmentFull.InvestmentId);
            investmentBankPayments = investmentPrepaidFull.InvestmentBankPayments;
            investmentInstallmentInfos = investmentPrepaidFull.InvestmentInstallmentInfos;
        }

        // Diccionario por InvestmentPaymentId
        Dictionary<int, List<InvestmentBankPayment>> bankPaymentsDic = new Dictionary<int, List<InvestmentBankPayment>>();
        foreach (InvestmentBankPayment bankPayment in investmentBankPayments)
        {
            if (!bankPaymentsDic.TryGetValue(bankPayment.InvestmentPayment.Id, out List<InvestmentBankPayment> value))
                bankPaymentsDic[bankPayment.InvestmentPayment.Id] = new List<InvestmentBankPayment> { bankPayment };
            else
                value.Add(bankPayment);
        }

        Dictionary<int, List<InvestmentBankPayment>> installmentPaymentsDic = new Dictionary<int, List<InvestmentBankPayment>>();
        foreach (InvestmentInstallmentInfo installmentInfo in investmentInstallmentInfos)
        {
            List<InvestmentInstallmentPayment> installmentPayments = installmentInfo.InvestmentInstallmentPayments;
            foreach (InvestmentInstallmentPayment installmentPayment in installmentPayments)
            {
                if (!bankPaymentsDic.TryGetValue(installmentPayment.InvestmentPaymentId, out List<InvestmentBankPayment> value))
                    installmentPaymentsDic[installmentPayment.Id] = new List<InvestmentBankPayment>();
                else
                    installmentPaymentsDic[installmentPayment.Id] = value;
            }
        }

        // Local

        lstAppliedPayment.Clear();

        if (!installmentPaymentsDic.TryGetValue(installmentInfos[idxInstallment].InvestmentInstallmentPayments[idxInstallmentPayment].Id, out List<InvestmentBankPayment> invBankPayments))
        {
            txtAppliedPaymentEmpty.gameObject.SetActive(true);
            return;
        }

        txtAppliedPaymentEmpty.gameObject.SetActive(false);

        for (int i = 0; i < invBankPayments.Count; i++)
        {
            int currencyId = Convert.ToInt32(vllAccountHpb.GetRecordCellString(invBankPayments[i].BankTransaction.AccountHpbId - 1, "CurrencyId"));
            String currencyCymbol = vllCurrency.GetRecordCellString(currencyId - 1, "Symbol");

            ListScrollerValue scrollerValue = new ListScrollerValue(9, true);
            scrollerValue.SetText(0, invBankPayments[i].BankTransaction.SendDateTime.ToString("MMMM dd, yyyy", StateManager.Instance.CultureInfo));
            scrollerValue.SetText(1, currencyCymbol + invBankPayments[i].BankTransaction.Amount.ToString("N2", CultureInfo.InvariantCulture));
            scrollerValue.SetText(2, invBankPayments[i].BankTransaction.Number);
            scrollerValue.SetText(3, vllAccountHpb.GetRecordCellString(invBankPayments[i].BankTransaction.AccountHpbId - 1, "Number"));

            if (invBankPayments[i].BankTransaction.Status == 0)
            {
                scrollerValue.SetText(4, "");
                scrollerValue.SetText(5, "");
                scrollerValue.SetText(6, "");
                scrollerValue.SetText(7, paymentStatusTitles[invBankPayments[i].BankTransaction.Status]);

                scrollerValue.SetText(8, "");
            }
            else if (invBankPayments[i].BankTransaction.Status == 1)
            {
                scrollerValue.SetText(4, "");
                scrollerValue.SetText(5, paymentStatusTitles[invBankPayments[i].BankTransaction.Status]);
                scrollerValue.SetText(6, "");
                scrollerValue.SetText(7, "");

                scrollerValue.SetText(8, "Tu pago fue aplicado satisfactoriamente.");
            }

            lstAppliedPayment.AddValue(scrollerValue);
        }
        lstAppliedPayment.ApplyValues();
    }


    // Payment

    public bool HasPendintPayment()
    {
        List<InvestmentBankPayment> bankPayments = investmentProjects[idxInvestment].InvestmentFull.InvestmentBankPayments;

        if (bankPayments == null)
            return false;

        foreach (InvestmentBankPayment bankPayment in bankPayments)
        {
            if (bankPayment.InvestmentPayment != null && bankPayment.InvestmentPayment.Status == 2)
                return true;
        }

        return false;
    }

    public InvestmentPaymentInfo GetInvestmentPaymentInfo()
    {
        int investmentPaymentTypeId = -1;
        double amount = 0.0d;
        DateTime dueDate = default;

        double balance = investmentProjects[idxInvestment].InvestmentFull.Balance;

        for (int i = 0; i < installmentInfos.Count; i++)
        {
            if (installmentInfos[i].InvestmentInstallment.Balance != 0.0d)
            {
                investmentPaymentTypeId = installmentInfos[i].InvestmentInstallment.InvestmentPaymentTypeId;
                amount = installmentInfos[i].InvestmentInstallment.Balance;
                dueDate = installmentInfos[i].InvestmentInstallment.DueDate;
                break;
            }
        }

        if (investmentPaymentTypeId == -1)
            return null;

        return new InvestmentPaymentInfo(investmentProjects[idxInvestment].InvestmentFull.InvestmentId, investmentPaymentTypeId,
                                         amount, dueDate, balance);
    }


    public void StartPayment()
    {
        if (HasPendintPayment())
        {
            ChoiceDialog.Instance.Warning("Pago pendiente", "Tienes un pago pendiente de conciliar. \r\n\r\nPara más información comunícate con atención al cliente.");
            return;
        }

        investmentPaymentInfo.Invoke(GetInvestmentPaymentInfo());

        PageManager.Instance.ChangePage(pagPaymentResume);
    }

    public void ApplyPayment()
    {
        ChoiceDialog.Instance.Info(paymentRegisteredTitle, paymentRegisteredMessage, () => PageManager.Instance.ChangePage(pagInvestmentInstallment));
    }

    public void DisplayPayments()
    {
        List<InvestmentBankPayment> investmentBankPayments = new List<InvestmentBankPayment>();

        if (investmentProjects[idxInvestment].InvestmentFull.ProductTypeId == 1)
            investmentBankPayments = (StateManager.Instance.GetInvestmentFractionatedFullByInvestmentId(investmentProjects[idxInvestment].InvestmentFull.InvestmentId).InvestmentBankPayments);
        else if (investmentProjects[idxInvestment].InvestmentFull.ProductTypeId == 2)
            investmentBankPayments = (StateManager.Instance.GetInvestmentFinancedFullByInvestmentId(investmentProjects[idxInvestment].InvestmentFull.InvestmentId).InvestmentBankPayments);
        else
            investmentBankPayments = (StateManager.Instance.GetInvestmentPrepaidFullByInvestmentId(investmentProjects[idxInvestment].InvestmentFull.InvestmentId).InvestmentBankPayments);

        lstPayment.Clear();

        if (investmentBankPayments.Count == 0)
        {
            txtPaymentEmpty.gameObject.SetActive(true);
            return;
        }

        txtPaymentEmpty.gameObject.SetActive(false);

        for (int i = 0; i < investmentBankPayments.Count; i++)
        {
            int currencyId = Convert.ToInt32(vllAccountHpb.GetRecordCellString(investmentBankPayments[i].BankTransaction.AccountHpbId - 1, "CurrencyId"));
            String currencyCymbol = vllCurrency.GetRecordCellString(currencyId - 1, "Symbol");

            ListScrollerValue scrollerValue = new ListScrollerValue(9, true);
            scrollerValue.SetText(0, investmentBankPayments[i].BankTransaction.SendDateTime.ToString("MMMM dd, yyyy", StateManager.Instance.CultureInfo));
            scrollerValue.SetText(1, currencyCymbol + investmentBankPayments[i].BankTransaction.Amount.ToString("N2", CultureInfo.InvariantCulture));
            scrollerValue.SetText(2, investmentBankPayments[i].BankTransaction.Number);
            scrollerValue.SetText(3, vllAccountHpb.GetRecordCellString(investmentBankPayments[i].BankTransaction.AccountHpbId - 1, "Number"));

            String motive = "";

            if (investmentBankPayments[i].BankTransaction.Status == 3)
                motive += "porque no se encontró el número";
            else if (investmentBankPayments[i].BankTransaction.Status == 4)
                motive += "porque la imagen del recibo es de mala calidad";


            if (investmentBankPayments[i].BankTransaction.Status == 1)
            {
                scrollerValue.SetText(4, "");
                scrollerValue.SetText(5, paymentStatusTitles[investmentBankPayments[i].BankTransaction.Status]);
                scrollerValue.SetText(6, "");
                scrollerValue.SetText(7, "");

                scrollerValue.SetText(8, "Tu pago fue aplicado satisfactoriamente.");
            }
            else if (investmentBankPayments[i].BankTransaction.Status == 2)
            {
                scrollerValue.SetText(4, "");
                scrollerValue.SetText(5, "");
                scrollerValue.SetText(6, "Pendiente de conciliación");
                scrollerValue.SetText(7, "");

                scrollerValue.SetText(8, "Pronto tendrás noticias nuestras.");
            }
            else if (investmentBankPayments[i].BankTransaction.Status == 3 || investmentBankPayments[i].BankTransaction.Status == 4)
            {
                scrollerValue.SetText(4, "");
                scrollerValue.SetText(5, "");
                scrollerValue.SetText(6, "");
                scrollerValue.SetText(7, paymentStatusTitles[investmentBankPayments[i].BankTransaction.Status]);

                scrollerValue.SetText(8, String.Format(msgRejected, motive));
            }

            lstPayment.AddValue(scrollerValue);
        }
        lstPayment.ApplyValues();
    }

    // Validate

    public void UpdateInvestmentFull(int investmentId)
    {
        ScreenDialog.Instance.Display();

        int investmentProductType = StateManager.Instance.GetInvestmentTypeByInvestmentId(investmentId);

        if (investmentProductType == 1)
            investmentService.GetFractionatedFullById(investmentId);
        else if (investmentProductType == 2)
            investmentService.GetFinancedFullById(investmentId);
        else if (investmentProductType == 3)
            investmentService.GetPrepaidFullById(investmentId);
        else
            ScreenDialog.Instance.Hide();
    }

    public void ApplyInvestmentFractionatedFull(InvestmentFractionatedFull investmentFractionatedFull)
    {
        StateManager.Instance.UpdateInvestmentFractionated(investmentFractionatedFull);

        ScreenDialog.Instance.Hide();
    }

    public void ApplyInvestmentFinancedFull(InvestmentFinancedFull investmentFinancedFull)
    {
        StateManager.Instance.UpdateInvestmentFinanced(investmentFinancedFull);

        ScreenDialog.Instance.Hide();
    }

    public void ApplyInvestmentPrepaidFull(InvestmentPrepaidFull investmentPrepaidFull)
    {
        StateManager.Instance.UpdateInvestmentPrepaid(investmentPrepaidFull);

        ScreenDialog.Instance.Hide();
    }

    // Receipt
    public void SelectPayment(int idx)
    {
        this.idxPayment = idx;
    }

    public void DisplayBankTransaction()
    {
        ScreenDialog.Instance.Display();
        //investmentService.GetTransactionReceipt(investmentBankPayments[idxPayment].BankTransaction.Id);
    }

    public void ApplyTransactionReceipt(String receipt)
    {
        imgReceipt.Sprite = receipt.CreateSprite("BankTransactionReceipt");
        ScreenDialog.Instance.Hide();
    }

    static int CalculateInvestmentTerm(DateTime startDate, int developmentTerm)
    {
        DateTime endDate = startDate.AddMonths(developmentTerm);
        if (DateTime.Today >= endDate)
            return 0;
        return (int)((endDate - DateTime.Today).TotalDays / 365d * 12d);
    }
}