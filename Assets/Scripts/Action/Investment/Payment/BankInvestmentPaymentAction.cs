using System;
using UnityEngine;
using UnityEngine.Events;
using System.Globalization;

using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Dialog;
using Leap.UI.Dialog.Gallery;
using Leap.UI.Extensions;
using Leap.Data.Mapper;
using Leap.Data.Collections;

using Sirenix.OdinInspector;

public class BankInvestmentPaymentAction : MonoBehaviour
{
    [SerializeField, Space]
    InputField ifdAmount = null;
    [SerializeField]
    ComboAdapter cmbBankTransaction = null;
    [SerializeField]
    Image imgBankTransaction = null;
    [SerializeField]
    ComboAdapter cmbSendTime = null;
    [SerializeField]
    ValueList vllAccountHpb = null;

    [Title("Photo")]
    [SerializeField]
    Button btnReceipt = null;

    [Title("Gallery")]
    [SerializeField]
    Vector2Int gallerySize = new Vector2Int(794, 560);

    [Title("Data")]
    [SerializeField]
    DataMapper dtmBankTransaction = null;

    [Title("Event")]
    [SerializeField]
    UnityEvent onReserveTransactionRegistered = null;
    [SerializeField]
    UnityEvent onQuoteTransactionRegistered = null;

    [Title("Rejected")]


    InvestmentService investmentService;
    Texture2D receipt = null;
    
    InvestmentPaymentInfo investmentPaymentInfo;

    private void Awake()
    {
        investmentService = GetComponent<InvestmentService>();
    }

    public void Clear()
    {
        dtmBankTransaction.ClearElements();
        cmbBankTransaction.Clear();
        cmbSendTime.Clear();
        imgBankTransaction.ClearValue();
        
        if (btnReceipt != null)
            btnReceipt.Clear();
    }

    public void SetInvestmentPaymentInfo(InvestmentPaymentInfo investmentPaymentInfo)
    {
        Clear();
        this.investmentPaymentInfo = investmentPaymentInfo;
    }

    public void SearchDoc()
    {
        GalleryDialog.Instance.Search(gallerySize, false, ApplyDoc);
    }

    private void ApplyDoc(Texture2D receipt)
    {
        this.receipt?.Destroy();
        this.receipt = receipt;

        Sprite sprite = receipt?.CreateSprite(true);
        btnReceipt.Sprite = sprite;
    }

    public int[] GetDateIndexes()
    {
        return new int[] { DateTime.Today.Day - 1, DateTime.Today.Month - 1, DateTime.Today.Year - 2023};
    }

    public int[] GetTimeIndexes()
    {
        return new int[] { DateTime.Now.Hour, DateTime.Now.Minute};
    }

    public void FillBankInfo()
    {
        if (cmbBankTransaction.GetSelectedId() == -1)
            return;
        
        imgBankTransaction.Sprite = vllAccountHpb.FindRecordCellSprite(cmbBankTransaction.GetSelectedId(), "Logo");

        int currencyId = Convert.ToInt32(vllAccountHpb.GetRecordCellString(cmbBankTransaction.GetSelectedId() - 1, "CurrencyId"));
        double amount = currencyId == 47 ? investmentPaymentInfo.Amount : Convert.ToDouble(AppManager.Instance.GetParamValue("ExchangeRate")) * investmentPaymentInfo.Amount;

        ifdAmount.Clear();
        ifdAmount.Interactable = investmentPaymentInfo.InvestmentPaymentTypeId != 0;
        ifdAmount.Required = investmentPaymentInfo.InvestmentPaymentTypeId != 0;
        ifdAmount.Text = amount.ToString("N2", CultureInfo.InvariantCulture);
        ifdAmount.InputValidate();

        //if (investmentPaymentInfo.InvestmentPaymentTypeId == 0)
        //{
        //    ifdAmount.Text = amount.ToString("N2", CultureInfo.InvariantCulture);
        //    ifdAmount.InputValidate();
        //}
    }

    public bool Register()
    {
        ScreenDialog.Instance.Display();

        BankTransaction bankTransaction = dtmBankTransaction.BuildClass<BankTransaction>();
        bankTransaction.AccountHpbId = Convert.ToInt32(cmbBankTransaction.GetSelectedId());

        int currencyId = Convert.ToInt32(vllAccountHpb.GetRecordCellString(bankTransaction.AccountHpbId - 1, "CurrencyId"));
        double maxAmount = currencyId == 47 ? investmentPaymentInfo.Balance : Convert.ToDouble(AppManager.Instance.GetParamValue("ExchangeRate")) * investmentPaymentInfo.Balance;

        if (bankTransaction.Amount > maxAmount)
        {
            ChoiceDialog.Instance.Error("Monto a pagar", "El monto a pagar no puede exceder el balance.");
            return false;
        }

        int hour = cmbSendTime.GetSelectedId(0);
        int minute = cmbSendTime.GetSelectedId(1);
        
        bankTransaction.SendDateTime = bankTransaction.SendDateTime.AddHours(hour).AddMinutes(minute);

        if (bankTransaction.SendDateTime > DateTime.Now)
        {
            ChoiceDialog.Instance.Error("Fecha y hora", "La fecha y hora no pueden ser mayor que la fecha y hora actual.");
            return false;
        }

        bankTransaction.AppUserId = StateManager.Instance.AppUser.Id;
        bankTransaction.TransactionTypeId = btnReceipt == null ? 1 : 2;

        if (investmentPaymentInfo.InvestmentPaymentTypeId == 0)
            bankTransaction.Amount = Convert.ToDouble(ifdAmount.Text);

        InvestmentPayment investmentPayment = new InvestmentPayment
        {
            InvestmentId = investmentPaymentInfo.InvestmentId,
            AppUserId = StateManager.Instance.AppUser.Id,
            InvestmentPaymentTypeId = investmentPaymentInfo.InvestmentPaymentTypeId,
            TransactionTypeId = btnReceipt == null ? 1 : 2
        };

        InvestmentBankPayment investmentBankPayment = new InvestmentBankPayment
        {
            InvestmentPayment = investmentPayment,
            BankTransaction = bankTransaction,
            Receipt = btnReceipt?.Sprite.ToStrBase64(ImageType.JPG)
        };

        investmentService.BankPayment(investmentBankPayment);
        
        return false;
    }

    public void ApplyInvestmentPaymentLocal(InvestmentBankPayment investmentBankPayment)
    {
        int investmentProductType = StateManager.Instance.GetInvestmentTypeByInvestmentId(investmentBankPayment.InvestmentPayment.InvestmentId);

        if (investmentProductType == 1)
            StateManager.Instance.AddInvestmentFractionatedPayment(investmentBankPayment.InvestmentPayment.InvestmentId, investmentBankPayment);
        else if (investmentProductType == 2)
            StateManager.Instance.AddInvestmentFinancedPayment(investmentBankPayment.InvestmentPayment.InvestmentId, investmentBankPayment);
        else
            StateManager.Instance.AddInvestmentPrepaidPayment(investmentBankPayment.InvestmentPayment.InvestmentId, investmentBankPayment);

        if (investmentBankPayment.InvestmentPayment.InvestmentPaymentTypeId == 0)
            onReserveTransactionRegistered.Invoke();
        else
            onQuoteTransactionRegistered.Invoke();

        Clear();
    }

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
}
