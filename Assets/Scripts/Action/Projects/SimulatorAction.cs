using System;
using System.Collections;

using UnityEngine;
using MPUIKIT;

using Leap.UI.Elements;

using Sirenix.OdinInspector;

public class SimulatorAction : MonoBehaviour
{
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
    [SerializeField]
    Text txtWaitMessage = null;
    [SerializeField]
    Button btnOnboarding = null;
    [SerializeField]
    Button btnInvestment = null;

    [Space]
    [Title("Simulator")]
    [SerializeField]
    FieldValue fldProfitablityRate = null;
    [SerializeField]
    FieldValue fldProfitablityAmount = null;
    [SerializeField]
    FieldValue fldCapitalGainAmount = null;

    [Space]
    [Title("Graph")]
    [SerializeField]
    public RectTransform graphArea;
    [SerializeField]
    Text txtYear;

    [Header("Action")]
    [SerializeField]
    public Button btnNext;
    [SerializeField]
    public Button btnPrev;

    [Space]
    [SerializeField]
    private Color[] colors;
    [Space]
    [SerializeField]
    Style[] txtStyles = null;

    [Header("Prefabs")]
    [SerializeField]
    public GameObject barPrefab;
    [SerializeField]
    private GameObject linePrefab;
    [SerializeField]
    private Text txtScalePrefab;

    private float maxHeight = 20000f;
    private int[] years = { 5, 10, 15, 20 };
    private int idx = 0;

    ProductRequest productRequest = new ProductRequest();
    double netCost = 0.0d;

    void Start()
    {
        btnNext?.AddAction(Next);
        btnPrev.AddAction(Previous);
    }

    public void SetProductRequest(ProductRequest productRequest)
    {
        this.productRequest = productRequest;
    }

    public void Initialize()
    {
        idx = 0;
        CreateGraph(years[idx]);
        btnNext.Interactable = true;
        btnPrev.Interactable = false;
    }

    public void DisplayProduct()
    {
        Initialize();

        if (productRequest.ProductType == 1)           // Fractionated
        {
            imgProducts[0].color = colorProducts[0];
            imgProducts[1].color = colorProducts[2];
            imgProducts[2].color = colorProducts[1];
            txtProductName.TextValue = nameProducts[0];
        }
        else if (productRequest.ProductType == 2)      // Financed
        {
            imgProducts[0].color = colorProducts[1];
            imgProducts[1].color = colorProducts[0];
            imgProducts[2].color = colorProducts[2];
            txtProductName.TextValue = nameProducts[1];
        }
        else                                             // Prepaid
        {
            imgProducts[0].color = colorProducts[2];
            imgProducts[1].color = colorProducts[1];
            imgProducts[2].color = colorProducts[0];
            txtProductName.TextValue = nameProducts[2];
        }

        btnOnboarding.gameObject.SetActive(StateManager.Instance.AppUser.AppUserStatusId == 0 || StateManager.Instance.AppUser.AppUserStatusId == 2);
        btnInvestment.gameObject.SetActive(StateManager.Instance.AppUser.AppUserStatusId == 1);
        txtWaitMessage.gameObject.SetActive(StateManager.Instance.AppUser.AppUserStatusId != 0 && StateManager.Instance.AppUser.AppUserStatusId != 1 && StateManager.Instance.AppUser.AppUserStatusId != 2);
    }

    public SimulatorFinancialInfo GetFinancialInfo(int year)
    {
        double cpiValue = productRequest.ProjectFull.CpiValue;
        double capitalGrowthRate = productRequest.ProjectFull.CapitalGrowthRate;
        double profitabilityRate = GetProfitabilityRate(productRequest.ProductType, productRequest.CpiCount);
        double totalAmount = productRequest.CpiCount * cpiValue;
        double profitabilityAmount = totalAmount * profitabilityRate * ((Math.Pow(1 + capitalGrowthRate, year) - 1) / capitalGrowthRate);
        double accumulatedCapitalGainAmount = totalAmount * (1 + (capitalGrowthRate * year));

        if (productRequest.ProductType != 2)
        {
            netCost = productRequest.DueAmount;
        }
        else
        {
            if (year == 5)
                netCost = productRequest.DueAmount + (productRequest.LoanInstallmentAmount * 12 * year) - profitabilityAmount;
        }

        fldProfitablityRate.TextValue = (profitabilityRate * 100.0d).ToString("N2") + "%";
        fldProfitablityAmount.TextValue = "$" + profitabilityAmount.ToString("N2");
        fldCapitalGainAmount.TextValue = "$" + accumulatedCapitalGainAmount.ToString("N2");

        return new SimulatorFinancialInfo
        {
            ProfitabilityAmount = profitabilityAmount,
            TotalAmount = totalAmount,
            CapitalGainAmount = accumulatedCapitalGainAmount - totalAmount,
            AccumulatedCapitalGainAmount = accumulatedCapitalGainAmount,
            DueAmount = netCost
        };
    }

    public void CreateGraph(int year)
    {
        SimulatorFinancialInfo simulatorFinancialInfo = GetFinancialInfo(20);

        int profitabilityAmount = Convert.ToInt32(simulatorFinancialInfo.ProfitabilityAmount);
        float factor = Mathf.Pow(10, Mathf.Floor(Mathf.Log10(profitabilityAmount)));

        maxHeight = Mathf.Ceil(profitabilityAmount / factor) * factor;

        simulatorFinancialInfo = GetFinancialInfo(year);

        foreach (Transform child in graphArea)
            Destroy(child.gameObject);

        DrawLinesAndScale();

        float xPosition = 200;

        CreateBar(xPosition, simulatorFinancialInfo.ProfitabilityAmount, colors[0], txtStyles[0]);
        xPosition += 150;
        CreateBar(xPosition, simulatorFinancialInfo.TotalAmount, colors[1], txtStyles[1]);
        xPosition += 150;
        CreateBar(xPosition, simulatorFinancialInfo.CapitalGainAmount, colors[2], txtStyles[0]);
        xPosition += 150;
        CreateBar(xPosition, simulatorFinancialInfo.AccumulatedCapitalGainAmount, colors[3], txtStyles[0]);
        xPosition += 150;
        CreateBar(xPosition, simulatorFinancialInfo.DueAmount, colors[4], txtStyles[1]);

        txtYear.TextValue = "Proyección a " + year.ToString() + " años";
    }

    private void CreateBar(float xPosition, double value, Color bkgColor, Style txtStyle)
    {
        GameObject bar = Instantiate(barPrefab, graphArea);
        RectTransform rectTransform = bar.GetComponent<RectTransform>();
        
        rectTransform.anchoredPosition = new Vector2(xPosition, 0);
        rectTransform.sizeDelta = new Vector2(120, rectTransform.sizeDelta.y);

        Text text = bar.GetComponentInChildren<Text>();
        text.SetStyle(txtStyle);

        MPImage mpImage = bar.GetComponent<MPImage>();
        if (mpImage != null)
        {
            mpImage.color = bkgColor;
            mpImage.fillAmount = Mathf.Clamp01((float)value / maxHeight);
        }

        Text txtAmount = bar.GetComponentInChildren<Text>();
        if (txtAmount != null)
            txtAmount.TextValue = "$" + value.ToString("N0");
    }

    private void DrawLinesAndScale()
    {
        int linesCount = 10;
        float lineSpacing = graphArea.rect.height / linesCount;
        float valuePerLine = maxHeight / linesCount;

        for (int i = 0; i <= linesCount; i++)
        {
            float yPosition = i * lineSpacing;

            GameObject line = Instantiate(linePrefab, graphArea);
            RectTransform lineTransform = line.GetComponent<RectTransform>();
            lineTransform.anchoredPosition = new Vector2(0, yPosition);
            lineTransform.sizeDelta = new Vector2(graphArea.rect.width, 2);

            Text scaleText = Instantiate(txtScalePrefab, graphArea);
            RectTransform textTransform = scaleText.GetComponent<RectTransform>();
            textTransform.anchoredPosition = new Vector2(0, yPosition + 16);
            scaleText.TextValue = "$" + (i * valuePerLine).ToString("N0");
        }
    }

    public void Next()
    {
        if (idx < years.Length - 1)
        {
            idx++;
            CreateGraph(years[idx]);
        }

        btnNext.Interactable = idx < years.Length - 1;
        btnPrev.Interactable = idx > 0;
    }

    public void Previous()
    {
        if (idx > 0)
        {
            idx--;
            CreateGraph(years[idx]);
        }

        btnNext.Interactable = idx < years.Length - 1;
        btnPrev.Interactable = idx > 0;
    }

    public void StartYearAnimation()
    {
        StartCoroutine(AnimateGraphOverYears());
    }

    private IEnumerator AnimateGraphOverYears()
    {
        while (true)
        {
            foreach (int year in years)
            {
                CreateGraph(year);
                yield return new WaitForSeconds(3f);
            }
        }
    }

    public double GetProfitabilityRate(int productTypeId, int cpiCount)
    {
        for (int i = 0; i < productRequest.ProjectFull.CpiRanges.Count; i++)
        {
            CpiRange cpiRange = productRequest.ProjectFull.CpiRanges[i];

            if (cpiRange.Status == 1 && cpiRange.ProductTypeId == productTypeId && cpiCount >= cpiRange.AmountMin && cpiCount <= cpiRange.AmountMax)
                return cpiRange.ProfitablityRate;
        }
        return 0;
    }
}

