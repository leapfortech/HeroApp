using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

using Leap.UI.Elements;
using UnityEngine.Events;

using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Core.Tools;

using Sirenix.OdinInspector;

public class InvestmentRegisterAction : MonoBehaviour
{
    [Serializable]
    public class InvestmentInfoEvent : UnityEvent<(int investmentId, bool isUpdate)> { }

    [Space]
    [Title("Params")]
    [SerializeField]
    Text txtQuoteAmount = null;
    [SerializeField]
    FieldValue fldTotalAmount = null;
    [SerializeField]
    FieldValue fldReserveAmount = null;

    [SerializeField]
    FieldValue fldProjectName = null;
    [SerializeField]
    FieldValue fldCpiCount = null;
    [SerializeField]
    FieldValue fldTerm = null;
    [SerializeField]
    FieldValue fldProductType = null;

    [Space]
    [SerializeField]
    String[] nameProducts = null;

    [Space]
    [Title("Action")]
    [SerializeField]
    Button btnRegister = null;

    [Space]
    [Title("Events")]
    [SerializeField]
    private InvestmentInfoEvent onInvestment;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    InvestmentService investmentService;
    Investment investment = new Investment();

    ProductRequest productRequest = new ProductRequest();

    private void Awake()
    {
        investmentService = GetComponent<InvestmentService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(AskQuestion);
    }

    public void SetProductRequest(ProductRequest productRequest)
    {
        this.productRequest = productRequest;
    }

    

    public void DisplayInvestment()
    {
        txtQuoteAmount.TextValue = productRequest.DueAmount.ToString("N2", CultureInfo.InvariantCulture);
        fldTotalAmount.TextValue = (productRequest.CpiCount * productRequest.ProjectFull.CpiValue).ToString("N2", CultureInfo.InvariantCulture);
        fldReserveAmount.TextValue = "$" + productRequest.ReserveAmount.ToString("N2");
        fldProjectName.TextValue = productRequest.ProjectFull.Name;
        fldCpiCount.TextValue = productRequest.CpiCount.ToString("N0");
        fldTerm.TextValue = productRequest.InvestmentTerm.ToString("N0") + " Meses";
        fldProductType.TextValue = nameProducts[productRequest.ProductType - 1];
    }

    private void AskQuestion()
    {
        ChoiceDialog.Instance.Info("Solicitar inversión", "¿Está seguro que desea adquirir la inversión?", RegisterInvestment, null, "Confirmar", "Regresar");
    }


    private void RegisterInvestment()
    {
        ScreenDialog.Instance.Display();

        double totalAmount = productRequest.CpiCount * productRequest.ProjectFull.CpiValue;
        double dueAmount = totalAmount - productRequest.DiscountAmount;

        investment = new Investment(-1, productRequest.ProjectFull.ProjectId, productRequest.ProductType,
                                    StateManager.Instance.AppUser.Id, DateTime.Now, productRequest.InvestmentTerm,
                                    productRequest.CpiCount, totalAmount, productRequest.ReserveAmount, dueAmount,
                                    productRequest.DiscountRate, productRequest.DiscountAmount, totalAmount,
                                    null, null, 0, null, 1);
        
        if (productRequest.ProductType == 1)
            investmentService.RegisterFractionated(investment);
        else if (productRequest.ProductType == 2)
            investmentService.RegisterFinanced(investment);
        else
            investmentService.RegisterPrepaid(investment);
    }

    public void ApplyInvestmentFractionated(InvestmentFractionatedFull investmentFractionatedFull)
    {
        StateManager.Instance.AddInvestmentFractionatedFull(investmentFractionatedFull);

        onInvestment.Invoke((investmentFractionatedFull.InvestmentId, false));

        PageManager.Instance.ChangePage(pagNext);
    }

    public void ApplyInvestmentFinanced(InvestmentFinancedFull investmentFinancedFull)
    {
        StateManager.Instance.AddInvestmentFinancedFull(investmentFinancedFull);

        onInvestment.Invoke((investmentFinancedFull.InvestmentId, false));

        PageManager.Instance.ChangePage(pagNext);
    }

    public void ApplyInvestmentPrepaid(InvestmentPrepaidFull investmentPrepaidFull)
    {
        StateManager.Instance.AddInvestmentPrepaidFull(investmentPrepaidFull);

        onInvestment.Invoke((investmentPrepaidFull.InvestmentId, false));

        PageManager.Instance.ChangePage(pagNext);
    }
}