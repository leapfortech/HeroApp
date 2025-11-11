using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.Graphics.Tools;
using Leap.UI.Dialog;
using Leap.Data.Collections;
using Leap.Core.Tools;

using Sirenix.OdinInspector;


public class DisplayProjectAction : MonoBehaviour
{
    [Space]
    [Title("Lists")]
    [SerializeField]
    ListScroller lstProject = null;
    [SerializeField]
    Text txtProjectEmpty;
    [SerializeField]
    ListScroller lstProjectLike = null;
    [SerializeField]
    Text txtProjectLikeEmpty;
    [SerializeField]
    ListScroller lstProjectSearch = null;
    [SerializeField]
    Text txtProjectSearchEmpty;
    [SerializeField]
    InputField ifdFilter = null;

    [Space]
    [Title("Description")]
    [SerializeField]
    Image imgCover = null;
    [Space]
    [SerializeField]
    FieldValue fldProjectType = null;
    [SerializeField]
    FieldValue fldTerm = null;
    [Space]
    [SerializeField]
    Text txtAddress1 = null;
    [SerializeField]
    Text txtAddress2 = null;
    [SerializeField]
    Text txtDescription = null;

    [Space]
    [SerializeField]
    Text[] txtCodes = null;
    [Space]
    [SerializeField]
    Text[] txtNames = null;

    [Space]
    [Title("Images")]
    [SerializeField]
    ListScroller lstImage = null;

    [Space]
    [Title("Details")]
    [SerializeField]
    Text txtAddress1D = null;
    [SerializeField]
    Text txtAddress2D = null;
    [Space]
    [SerializeField]
    FieldValue fldProjectTypeD = null;
    [SerializeField]
    FieldValue fldStartDate = null;
    [SerializeField]
    FieldValue fldEndDate = null;
    [SerializeField]
    FieldValue fldTermD = null;
    [Space]
    [SerializeField]
    Text txtDetails = null;
    [SerializeField]
    FieldValue fldTotalArea = null;
    [SerializeField]
    FieldValue fldTotalConstructedArea = null;
    [SerializeField]
    FieldValue fldLevelCount = null;
    [SerializeField]
    FieldValue fldTotalValue = null;
    [SerializeField]
    FieldValue fldCapitalGrowthRate = null;
    [SerializeField]
    FieldValue fldRentalGrowthRate = null;
    [SerializeField]
    Text txtAdvantagesCond = null;
    [SerializeField]
    Text txtConditions = null;

    [Space]
    [Title("Events")]
    [SerializeField]
    private UnityIntEvent onProjectSelected;
    
    [Title("Values")]
    [SerializeField]
    ValueList vllProjectDescriptionType = null;


    [Title("Action")]
    [SerializeField]
    Button btnProjectImages = null;
    [SerializeField]
    Button btnFilterProjects = null;

    [SerializeField]
    Button btnFractionated = null;
    [SerializeField]
    Button btnFinanced = null;
    [SerializeField]
    Button btnPrepaid = null;
    //[SerializeField]
    //Button btnProjectLike = null;

    [Title("Page")]
    [SerializeField]
    Page pagProjects;
    [SerializeField]
    Page pagProjectsLike;
    [SerializeField]
    Page pagProjectsSearch;

    [SerializeField]
    Page pagProjectDetail;
    [SerializeField]
    Page pagProjectImages;
    [SerializeField]
    Page pagProjectDescription;

    ProjectService projectService;
    public int SelProjectIdx { get; set; } = 0;
    //public int DisplayType { get; set; } = 0;  // 1-Prducts 2-ProductsLike 3-ProductsSearch
    public int ProjectInformationType { get; set; } = 0;

    bool initialize = false;


    private void Awake()
    {
        projectService = GetComponent<ProjectService>();
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (initialize)
            return;

        btnProjectImages?.AddAction(GetImages);
        btnFilterProjects?.AddAction(() => { DisplayProjects(3); });
        
        initialize = true;
    }

    // Display

    public void ClearDisplay()
    {
        idx = 0;
        indexes.Clear();
        lstProject.ClearValues();
        lstProjectLike.ClearValues();
        lstProjectSearch.ClearValues();
    }

    Dictionary<int, int> indexes = new Dictionary<int, int>();
    int idx = 0;

    public void DisplayProjects(int displayType)
    {
        ClearDisplay();

        List<ListScrollerValue> productValues = new List<ListScrollerValue>();

        List<int> projectLikeIds = displayType == 2 ? StateManager.Instance.ProjectLikeIds : null;
        String nameFilter = displayType == 3 ? ifdFilter.Text.ToLower() : null;

        productValues.AddRange(GetProjectProductFullValues(StateManager.Instance.AvaProjectProductFulls, projectLikeIds, nameFilter));

        ListScroller targetList = displayType == 1 ? lstProject : displayType == 2 ? lstProjectLike : lstProjectSearch;
        if (displayType == 1 && productValues.Count == 0)
        {
            ScreenDialog.Instance.Hide();
            txtProjectEmpty.gameObject.SetActive(true);
            targetList.ClearValues();
            targetList.ApplyClearValues();
            return;
        }
        else if (displayType == 2 && productValues.Count == 0)
        {
            ScreenDialog.Instance.Hide();
            txtProjectLikeEmpty.gameObject.SetActive(true);
            targetList.ClearValues();
            targetList.ApplyClearValues();
            return;
        }
        else if (displayType == 3 && productValues.Count == 0)
        {
            ScreenDialog.Instance.Hide();
            txtProjectSearchEmpty.gameObject.SetActive(true);
            lstProjectSearch.ClearValues();
            lstProjectSearch.ApplyClearValues();
            return;
        }

        txtProjectEmpty.gameObject.SetActive(false);
        txtProjectLikeEmpty.gameObject.SetActive(false);
        txtProjectSearchEmpty.gameObject.SetActive(false);

        foreach (ListScrollerValue value in productValues)
            targetList.AddValue(value);

        targetList.ApplyValues();
    }

    public List<ListScrollerValue> GetProjectProductFullValues(List<ProjectProductFull> projectProductFulls,
                                                               List<int> projectLikeFilterIds, 
                                                               String nameFilter)
    {
        List<ListScrollerValue> scrollerValues = new List<ListScrollerValue>();

        for (int i = 0; i < projectProductFulls.Count; i++)
        {
            bool isProjectIdMatch = projectLikeFilterIds != null && projectLikeFilterIds.Contains(projectProductFulls[i].ProjectFull.ProjectId);
            bool isNameMatch = (nameFilter != null && nameFilter.Length == 0) || (nameFilter != null && projectProductFulls[i].ProjectFull.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase));
            bool hasNoFilter = projectLikeFilterIds == null && nameFilter == null;

            if (hasNoFilter || isProjectIdMatch || isNameMatch)
            {
                indexes.Add(idx, projectProductFulls[i].ProjectFull.ProjectId);
                idx++;

                int term = CalculateInvestmentTerm(projectProductFulls[i].ProjectFull.StartDate, projectProductFulls[i].ProjectFull.DevelopmentTerm);
                Tuple<double, double> discountRates = GetDiscountRanges(projectProductFulls[i].ProjectFull.CpiRanges);

                ListScrollerValue scrollerValue = new ListScrollerValue(7, true);
                scrollerValue.SetText(0, projectProductFulls[i].ProjectFull.Name);
                scrollerValue.SetText(1, projectProductFulls[i].ProjectFull.AddressFull.Address1);
                scrollerValue.SetText(2, projectProductFulls[i].ProjectFull.AddressFull.Address2);
                scrollerValue.SetText(3, projectProductFulls[i].ProjectFull.ProjectType);
                scrollerValue.SetText(4, (discountRates.Item1 * 100).ToString("N2") + "%  -  " + (discountRates.Item2 * 100).ToString("N2") + "%");
                scrollerValue.SetText(5, term.ToString() + " meses");
                scrollerValue.SetSprite(6, projectProductFulls[i].ProjectFull.CoverSprite);

                scrollerValues.Add(scrollerValue);
            }
        }

        return scrollerValues;
    }

    // Display Detail

    private int GetProjectId()
    {
        if (indexes.TryGetValue(SelProjectIdx, out int projectId))
            return projectId;
        return -1;
    }

    public Page PagProjectDetailBack
    {
        get => pagProjectDetail.HeaderPage;
        set => pagProjectDetail.HeaderPage = value;
    }

    public void DisplayDetails()
    {
        ProjectProductFull projectProductFull = StateManager.Instance.GetProjectProductFullByProjectId(GetProjectId());

        if (projectProductFull == null)
            return;

        imgCover.Sprite = projectProductFull.ProjectFull.CoverSprite;

        for (int i = 0; i < txtCodes.Length; i++)
            txtCodes[i].TextValue = projectProductFull.ProjectFull.Code;

        for (int i = 0; i < txtNames.Length; i++)
            txtNames[i].TextValue = projectProductFull.ProjectFull.Name;

        txtAddress1.TextValue = txtAddress1D.TextValue = projectProductFull.ProjectFull.AddressFull.Address1;
        txtAddress2.TextValue = txtAddress2D.TextValue = projectProductFull.ProjectFull.AddressFull.Address2;

        fldProjectType.TextValue = fldProjectTypeD.TextValue = projectProductFull.ProjectFull.ProjectType;
        fldStartDate.TextValue = projectProductFull.ProjectFull.StartDate.ToString("MMMM dd, yyyy", StateManager.Instance.CultureInfo);
        fldEndDate.TextValue = projectProductFull.ProjectFull.StartDate.AddMonths(projectProductFull.ProjectFull.DevelopmentTerm).ToString("MMMM dd, yyyy", StateManager.Instance.CultureInfo);
        fldTerm.TextValue = fldTermD.TextValue = CalculateInvestmentTerm(projectProductFull.ProjectFull.StartDate, projectProductFull.ProjectFull.DevelopmentTerm).ToString();

        txtDescription.TextValue = projectProductFull.ProjectFull.Description;
        txtDetails.TextValue = projectProductFull.ProjectFull.Details;

        fldTotalArea.TextValue = (projectProductFull.ProjectFull.TotalArea != 0 ? projectProductFull.ProjectFull.TotalArea.ToString("N0") + " metros cuadrados" : "No disponible");
        fldTotalConstructedArea.TextValue = (projectProductFull.ProjectFull.TotalBuiltArea != 0 ? projectProductFull.ProjectFull.TotalBuiltArea.ToString("N0") : "No disponible");
        fldLevelCount.TextValue = (projectProductFull.ProjectFull.LevelCount != 0 ? projectProductFull.ProjectFull.LevelCount.ToString("N0") : "No disponible");

        fldTotalValue.TextValue = projectProductFull.ProjectFull.TotalValue.ToString("N0");
        fldCapitalGrowthRate.TextValue = (projectProductFull.ProjectFull.CapitalGrowthRate * 100).ToString() + "%";
        fldRentalGrowthRate.TextValue = (projectProductFull.ProjectFull.RentalGrowthRate * 100).ToString() + "%";

        txtAdvantagesCond.TextValue = "";
        for (int i = 0; i < projectProductFull.ProjectFull.Informations.Count; i++)
            if (projectProductFull.ProjectFull.Informations[i].ProjectInformationTypeId == 1)
                txtAdvantagesCond.TextValue = projectProductFull.ProjectFull.Informations[i].Information;

        int productTypeBitMask = GetProductType(projectProductFull);

        btnFractionated.gameObject.SetActive((productTypeBitMask & 1) != 0 && projectProductFull.ProjectFull.CpiRemain >= projectProductFull.ProductFractionated.CpiMin);
        btnFinanced.gameObject.SetActive((productTypeBitMask & 2) != 0 && projectProductFull.ProjectFull.CpiRemain >= projectProductFull.ProductFinanced.CpiMin);
        btnPrepaid.gameObject.SetActive((productTypeBitMask & 4) != 0 && projectProductFull.ProjectFull.CpiRemain >= projectProductFull.ProductPrepaid.CpiMin);

        onProjectSelected.Invoke(GetProjectId());

        ChangePage(pagProjectDetail);
    }

    public void DisplayConditions()
    {
        txtConditions.TextValue = "";

        ProjectProductFull projectProductFull = StateManager.Instance.GetProjectProductFullByProjectId(GetProjectId());

        for (int i = 0; i < projectProductFull.ProjectFull.Informations.Count; i++)
        {
            if (projectProductFull.ProjectFull.Informations[i].ProjectInformationTypeId == ProjectInformationType)
            {
                txtConditions.TextValue = projectProductFull.ProjectFull.Informations[i].Information;
                pagProjectDescription.HeaderTitle = vllProjectDescriptionType.FindRecordCellString(ProjectInformationType, "Name");
            }
        }

        PageManager.Instance.ChangePage(pagProjectDescription);
    }

    // Display Images
    public void DisplayImages()
    {
        List<Sprite> projectImages = StateManager.Instance.GetProjectImagesById(GetProjectId());

        lstImage.Clear();

        for (int i = 0; i < projectImages.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(1, true);
            scrollerValue.SetSprite(0, projectImages[i]);
            lstImage.ApplyAddValue(scrollerValue);
        }

        ChangePage(pagProjectImages);
    }

    public void GetImages()
    {
        if (StateManager.Instance.GetProjectImagesById(GetProjectId()) != null)
        {
            DisplayImages();
            return;
        }
        
        ScreenDialog.Instance.Display();
        projectService.GetImagesById(GetProjectId());
    }

    public void ApplyImages(String[] stgImages)
    {
        StateManager.Instance.AddProjectImages(GetProjectId(), stgImages);
        DisplayImages();
    }

    private void ChangePage(Page nextPage)
    {
        PageManager.Instance.ChangePage(nextPage);
    }

    //static int CalculateCurrentTerm(DateTime startDate, int developmentTerm)
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

    private int GetProductType(ProjectProductFull projectProductFull)
    {
        int productTypeBitMask = 0;

        if (projectProductFull.ProductFractionated != null) productTypeBitMask |= 1; // 0001
        if (projectProductFull.ProductFinanced != null) productTypeBitMask |= 2;     // 0010
        if (projectProductFull.ProductPrepaid != null) productTypeBitMask |= 4;      // 0100

        return productTypeBitMask;
    }

    public static Tuple<double,double> GetDiscountRanges(List<CpiRange> cpiRanges)
    {
        if (cpiRanges == null || cpiRanges.Count == 0)
            return new Tuple<double, double>(0.0, 0.0);

        double minDiscountRate = double.MaxValue;
        double maxDiscountRate = double.MinValue;

        foreach (var cpi in cpiRanges)
        {
            if (cpi.DiscountRate < minDiscountRate)
                minDiscountRate = cpi.DiscountRate;
            if (cpi.DiscountRate > maxDiscountRate)
                maxDiscountRate = cpi.DiscountRate;
        }

        return new Tuple<double, double>(minDiscountRate, maxDiscountRate);
    }
}