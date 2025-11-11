using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;

using Sirenix.OdinInspector;
using MPUIKIT;

public class QuoteProductAction : MonoBehaviour
{
    [Serializable]
    public class ProductRequestEvent : UnityEvent<ProductRequest> { }

    [Title("Header")]
    [SerializeField]
    MPImage[] imgProducts = null;
    [Space]
    [SerializeField]
    Color[] colorProducts = null;
    [Space]
    [SerializeField]
    String[] nameProducts = null;
    [Space]
    [SerializeField]
    Text txtProductName = null;


    [Space]
    [Title("Wheels")]
    [SerializeField]
    Image imgWheelBkg = null;
    [SerializeField]
    WheelScroller wheelCpiCount = null;
    [Space]
    [SerializeField]
    Style[] imgWheelStyles = null;

    [Space]
    [Title("Quote")]
    [SerializeField]
    Text txtPrincipalAmount = null;
    [SerializeField]
    FieldValue fldReserveAmount = null;
    [SerializeField]
    GameObject rgnFractionatedPrepaid = null;
    [SerializeField]
    GameObject rgnFinanced = null;


    [Title("Fractionated and Prepaid")]
    [SerializeField]
    FieldValue fldCpiValue = null;
    [SerializeField]
    FieldValue fldRealAmount = null;
    [SerializeField]
    FieldValue fldDiscountRate = null;
    [SerializeField]
    FieldValue fldDiscountAmount = null;
    [SerializeField]
    FieldValue fldDueAmount = null;
    [SerializeField]
    FieldValue fldInstallmentAmount = null;

    [Title("Financed")]
    [SerializeField]
    Text txtFinancedDataType = null;

    [SerializeField]
    FieldValue fldFinancedReserveAmount = null;
    [SerializeField]
    FieldValue fldFinancedCpiValue = null;
    [SerializeField]
    FieldValue fldAdvRate = null;
    [SerializeField]
    FieldValue fldAdvAmount = null;
    [SerializeField]
    FieldValue fldTerm = null;
    [SerializeField]
    FieldValue fldFinancedInstallmentAmount = null;

    [SerializeField]
    FieldValue fldFinancedRate = null;
    [SerializeField]
    FieldValue fldFinancedAmount = null;
    [SerializeField]
    FieldValue fldLoanInterestRate = null;
    [SerializeField]
    FieldValue fldLoanTerm = null;
    [SerializeField]
    FieldValue fldLoanInstallmentAmount = null;
    [SerializeField]
    Image imgFinanced1 = null;
    [SerializeField]
    Image imgFinanced2 = null;

    [Title("Action")]
    [SerializeField]
    public Button btnNext;
    [SerializeField]
    public Button btnPrev;
    [SerializeField]
    Button btnSimulator = null;

    [Space]
    [Title("Pages")]
    [SerializeField]
    Page pagSimulator = null;

    [Space]
    [Title("Event")]
    [SerializeField]
    ProductRequestEvent onProductRequest = null;


    Dictionary<int, int> dicCpiCount = new Dictionary<int, int>();
    ProjectProductFull projectProductFull = new ProjectProductFull();
    
    int productType = -1;
    int cpiCount = 0;
    bool refresh = true;
    int investmentTerm;

    double dueAmount, loanInstallmentAmount, reserveAmount, discountRate, discountAmount;

    private void Start()
    {
        btnNext?.AddAction(Next);
        btnPrev.AddAction(Previous);
        btnSimulator?.AddAction(StartSimulator);
    }

    public void InitializeFinanced()
    {
        txtFinancedDataType.TextValue = "Enganche";

        imgFinanced1.gameObject.SetActive(true);
        imgFinanced2.gameObject.SetActive(false);

        btnNext.Interactable = true;
        btnPrev.Interactable = false;
    }

    private void Clear()
    {
        dicCpiCount.Clear();
        wheelCpiCount.ClearValue();
    }

    public void SetProjectId(int projectId)
    {
        Clear();
        projectProductFull = StateManager.Instance.GetProjectProductFullByProjectId(projectId);
    }

    public void SetProductType(int productType)
    {
        this.productType = productType;
        refresh = true;
    }


    public void CalculateQuote()
    {
        if (dicCpiCount.TryGetValue(wheelCpiCount.SelectedIndex, out cpiCount))
        {
            investmentTerm = CalculateInvestmentTerm(projectProductFull.ProjectFull.StartDate, projectProductFull.ProjectFull.DevelopmentTerm);

            rgnFractionatedPrepaid.gameObject.SetActive(productType == 1 || productType == 3);
            rgnFinanced.gameObject.SetActive(productType == 2);

            if (productType == 1)           // Fractionated
            {
                imgProducts[0].color = colorProducts[0];
                imgProducts[1].color = colorProducts[2];
                imgProducts[2].color = colorProducts[1];
                txtProductName.TextValue = nameProducts[0];

                imgWheelBkg.SetStyle(imgWheelStyles[0]);
                discountRate = GetDiscountRate(productType, cpiCount);
                DisplayFractionated(projectProductFull.ProductFractionated, cpiCount, projectProductFull.ProjectFull.CpiValue, discountRate, investmentTerm);
            }
            else if (productType == 2)      // Financed
            {
                InitializeFinanced();

                imgProducts[0].color = colorProducts[1];
                imgProducts[1].color = colorProducts[0];
                imgProducts[2].color = colorProducts[2];
                txtProductName.TextValue = nameProducts[1];

                imgWheelBkg.SetStyle(imgWheelStyles[1]);
                DisplayFinanced(projectProductFull.ProductFinanced, cpiCount, projectProductFull.ProjectFull.CpiValue, investmentTerm);
            }
            else                            // Prepaid
            {
                imgProducts[0].color = colorProducts[2];
                imgProducts[1].color = colorProducts[1];
                imgProducts[2].color = colorProducts[0];
                txtProductName.TextValue = nameProducts[2];

                imgWheelBkg.SetStyle(imgWheelStyles[2]);
                discountRate = GetDiscountRate(productType, cpiCount); ;
                DisplayPrepaid(projectProductFull.ProductPrepaid, cpiCount, projectProductFull.ProjectFull.CpiValue, discountRate, investmentTerm);
            }
        }
        else
            return;
    }

    public void DisplayFractionated(ProductFractionated productFractionated, int cpiCount, double cpiValue,
                                    double discountRate, int investmentTerm)
    {
        double totalAmount = cpiCount * cpiValue;
        reserveAmount = totalAmount * productFractionated.ReserveRate;
        double installmentAmount = (totalAmount - reserveAmount) / (investmentTerm - 1);

        double baseDiscountAmount = (reserveAmount * discountRate / 12 * investmentTerm) + (installmentAmount * (investmentTerm - 2) * (discountRate / 12) * ((investmentTerm + 1) / 2d));
        double baseDueAmount = totalAmount - baseDiscountAmount;

        // Expande
        double fullPaymentTerm = Math.Floor((baseDueAmount - reserveAmount) / installmentAmount);
        
        double installmentSum = installmentAmount * fullPaymentTerm;
        double penultimateSum = reserveAmount + installmentSum;

        double discountFirstInstallment = (reserveAmount * discountRate / 12 * investmentTerm);
        double discountInstallmentSum = installmentAmount * (discountRate / 12) * ((fullPaymentTerm * (2 * (investmentTerm - 1) - fullPaymentTerm + 1)) / 2);
        double discountPenultimateSum = discountFirstInstallment + discountInstallmentSum;

        double lastInstallmentAmount = (totalAmount - penultimateSum - discountPenultimateSum) / (1 + (discountRate / 12) * (investmentTerm - fullPaymentTerm - 1));
        double lastInstallmentDiscount = lastInstallmentAmount * (discountRate / 12) * (investmentTerm - fullPaymentTerm - 1);

        dueAmount = penultimateSum + lastInstallmentAmount;
        discountAmount = totalAmount - dueAmount;

        loanInstallmentAmount = 0.0d;

        txtPrincipalAmount.TextValue = "$" + dueAmount.ToString("N2");
        fldReserveAmount.TextValue = "$" + reserveAmount.ToString("N2");

        fldCpiValue.TextValue = "$" + cpiValue.ToString("N2");
        fldRealAmount.Field = $"Valor de lista ({cpiCount.ToString()} CPI's)";
        fldRealAmount.TextValue = "$" + totalAmount.ToString("N2");
        fldDiscountRate.TextValue = (discountRate * 100.0f).ToString("N2") + " %";
        fldDiscountAmount.TextValue = "$" + discountAmount.ToString("N2");
        fldDueAmount.TextValue = "$" + dueAmount.ToString("N2");
        fldInstallmentAmount.gameObject.SetActive(true);
        fldInstallmentAmount.Field = $"Monto cuota ({investmentTerm.ToString()} meses)";
        fldInstallmentAmount.TextValue = "$" + installmentAmount.ToString("N2");
    }

    public void DisplayFinanced(ProductFinanced productFinaced, int cpiCount, double cpiValue, int investmentTerm)
    {
        // Product
        double totalAmount = cpiCount * cpiValue;
        dueAmount = totalAmount * productFinaced.AdvRate;
        reserveAmount = totalAmount * productFinaced.ReserveRate;

        discountRate = 0.0d;
        discountAmount = 0.0d;

        // Installment
        double installmentAmount = (dueAmount - reserveAmount) / investmentTerm;

        fldFinancedReserveAmount.TextValue = "$" + reserveAmount.ToString("N2");
        fldFinancedCpiValue.TextValue = cpiValue.ToString("N2"); ;
        fldAdvRate.TextValue = (productFinaced.AdvRate * 100.0d).ToString("N2") + " %";
        fldAdvAmount.TextValue = "$" + dueAmount.ToString("N2"); ;
        fldTerm.TextValue = $"{investmentTerm} meses";
        fldFinancedInstallmentAmount.TextValue = "$" + installmentAmount.ToString("N2");

        // Loan
        double financedRate = 1.0d - productFinaced.AdvRate;
        int loanTerm = Convert.ToInt32(AppManager.Instance.GetParamValue("LoanTermDefault"));
        double loanInterestRate = Convert.ToDouble(AppManager.Instance.GetParamValue("LoanInterestRateDefault"));

        double tem = Convert.ToDouble(AppManager.Instance.GetParamValue("LoanInterestRateDefault")) / 12;
        double loanAmount = totalAmount * financedRate;
        loanInstallmentAmount = loanAmount * (tem / (1 - (1 / Math.Pow(1 + tem, loanTerm))));

        fldFinancedRate.TextValue = (financedRate * 100.0d).ToString("N2") + " %";
        fldFinancedAmount.TextValue = "$" + loanAmount.ToString("N2");
        fldLoanInterestRate.TextValue = (loanInterestRate * 100d).ToString("N2") + " %";
        fldLoanTerm.TextValue = loanTerm.ToString() + " " + "(" + (loanTerm / 12).ToString("N0") + " años)";
        fldLoanInstallmentAmount.TextValue = "$" + loanInstallmentAmount.ToString("N2");

        double loanTotalPaid = loanInstallmentAmount * loanTerm;
        double loanTotalInterest = loanTotalPaid - loanAmount;

        double totalPaid = totalAmount + loanTotalInterest;
        txtPrincipalAmount.TextValue = "$" + totalPaid.ToString("N2");
    }

    public void DisplayPrepaid(ProductPrepaid productPrepaid, int cpiCount, double cpiValue, double discountRate,
                               int investmentTerm)
    {
        double totalAmount = cpiCount * cpiValue;
        dueAmount = totalAmount / (1 + (discountRate / 12.0d * investmentTerm));
        reserveAmount = dueAmount * productPrepaid.ReserveRate;
        discountAmount = totalAmount - dueAmount;

        loanInstallmentAmount = 0.0d;

        txtPrincipalAmount.TextValue = "$" + dueAmount.ToString("N2");
        fldReserveAmount.TextValue = "$" + reserveAmount.ToString("N2");

        fldCpiValue.TextValue = "$" + cpiValue.ToString("N2");
        fldRealAmount.Field = $"Valor de lista ({cpiCount.ToString()} CPI's)";
        fldRealAmount.TextValue = "$" + totalAmount.ToString("N2");
        fldDiscountRate.TextValue = (discountRate * 100.0f).ToString("N2") + " %";
        fldDiscountAmount.TextValue = "$" + discountAmount.ToString("N2");
        fldDueAmount.TextValue = "$" + dueAmount.ToString("N2");
        fldInstallmentAmount.gameObject.SetActive(false);
    }

    public void Next()
    {
        txtFinancedDataType.TextValue = "Financiamiento";

        imgFinanced1.gameObject.SetActive(false);
        imgFinanced2.gameObject.SetActive(true);

        btnNext.Interactable = false;
        btnPrev.Interactable = true;
    }

    public void Previous()
    {
        txtFinancedDataType.TextValue = "Enganche";

        imgFinanced1.gameObject.SetActive(true);
        imgFinanced2.gameObject.SetActive(false);

        btnNext.Interactable = true;
        btnPrev.Interactable = false;
    }

    public void FillWheel()
    {
        if (!refresh && wheelCpiCount.ValuesCount != 0)
            return;

        wheelCpiCount.ClearValues();
        dicCpiCount.Clear();

        int cpiCountMax = 150;
        int cpiCountMin = 0;

        if (productType == 1)             // Fractionated
        {
            cpiCountMin = projectProductFull.ProductFractionated.CpiMin;
            cpiCountMax = Math.Min(projectProductFull.ProductFractionated.CpiMax, projectProductFull.ProjectFull.CpiRemain);
        }
        else if (productType == 2)         // Financed
        {
            cpiCountMin = projectProductFull.ProductFinanced.CpiMin;
            cpiCountMax = Math.Min(projectProductFull.ProductFinanced.CpiMax, projectProductFull.ProjectFull.CpiRemain);
        }
        else                               // Prepaid
        {
            cpiCountMin = projectProductFull.ProductPrepaid.CpiMin;
            cpiCountMax = Math.Min(projectProductFull.ProductPrepaid.CpiMax, projectProductFull.ProjectFull.CpiRemain);
        }

        int wllIdx = 0;
        for (int i = cpiCountMin; i <= cpiCountMax; i++)
        {
            WheelScrollerValue wheelValue = new WheelScrollerValue(1);
            wheelValue.SetText(0, i.ToString());

            wheelCpiCount.AddValue(wheelValue);
            dicCpiCount.Add(wllIdx, i);
            wllIdx++;
        }
        wheelCpiCount.ApplyValues();

        if (productType == 1)                   // Fractionated
            wheelCpiCount.SelectedIndex = projectProductFull.ProductFractionated.CpiDefault - projectProductFull.ProductFractionated.CpiMin;    
        else if (productType == 2)              // Financed
            wheelCpiCount.SelectedIndex = projectProductFull.ProductFinanced.CpiDefault - projectProductFull.ProductFinanced.CpiMin;
        else                                    // Prepaid
            wheelCpiCount.SelectedIndex = projectProductFull.ProductPrepaid.CpiDefault - projectProductFull.ProductPrepaid.CpiMin; ;

        refresh = false;
    }

    public void StartSimulator()
    {
        onProductRequest.Invoke(new ProductRequest(projectProductFull.ProjectFull, productType, cpiCount, dueAmount,
                                                   loanInstallmentAmount, investmentTerm, reserveAmount, discountRate,
                                                   discountAmount));

        PageManager.Instance.ChangePage(pagSimulator);
    }

    //static int CalculateInvestmentTerm(DateTime startDate, int developmentTerm)
    //{
    //    DateTime endDate = startDate.AddMonths(developmentTerm);
    //    DateTime currentDate = DateTime.Now;

    //    if (currentDate >= endDate)
    //        return 0;

    //    if (currentDate < startDate)
    //        return (endDate.Year - startDate.Year) * 12 + (endDate.Month - startDate.Month);

    //    int monthDifference = (endDate.Year - currentDate.Year) * 12 + (endDate.Month - currentDate.Month);

    //    if (currentDate.Day > endDate.Day)
    //        monthDifference--;

    //    return monthDifference;
    //}

    static int CalculateInvestmentTerm(DateTime startDate, int developmentTerm)
    {
        DateTime endDate = startDate.AddMonths(developmentTerm);
        if (DateTime.Today >= endDate)
            return 0;
        return (int)((endDate - DateTime.Today).TotalDays / 365d * 12d);
    }

    public double GetDiscountRate(int productTypeId, int cpiCount)
    {
        for (int i = 0; i < projectProductFull.ProjectFull.CpiRanges.Count; i++)
        {
            CpiRange cpiRange = projectProductFull.ProjectFull.CpiRanges[i];

            if (cpiRange.Status == 1 && cpiRange.ProductTypeId == productTypeId && cpiCount >= cpiRange.AmountMin && cpiCount <= cpiRange.AmountMax)
                return cpiRange.DiscountRate;
        }
        return 0;
    }
}