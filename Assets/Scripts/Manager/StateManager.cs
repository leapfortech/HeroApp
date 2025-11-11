using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

using Leap.Core.Tools;
using Leap.Graphics.Tools;

using Sirenix.OdinInspector;

public class StateManager : SingletonBehaviour<StateManager>
{
    private readonly String[] monthNames = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
    public String[] MonthNames => monthNames;

    public CultureInfo CultureInfo = new CultureInfo("es-ES");

    public int OnboardingStage { get; set; } = -1;

    public List<NewsInfo> NewsInfos { get; set; } = null;

    public List<MeetingFull> MeetingFulls { get; set; } = null;

    public ReferredCount ReferredCount { get; set; } = null;

    [PropertySpace]
    [ShowInInspector, HideReferenceObjectPicker, ReadOnly]
    public AppUser AppUser { get; set; } = null;

    [PropertySpace]
    [ShowInInspector, HideReferenceObjectPicker, ReadOnly]
    public Identity Identity { get; set; } = null;

    [PropertySpace]
    [ShowInInspector, HideReferenceObjectPicker, ReadOnly]
    public Address Address { get; set; } = null;

    [PropertySpace]
    [ShowInInspector, HideReferenceObjectPicker, ReadOnly]
    public Card Card { get; set; } = null;

    private Sprite portrait = null;
    public Sprite Portrait
    {
        get => portrait;
        set { portrait?.Destroy(); portrait = value; }
    }

    // PROJECTS

    public List<ProjectProductFull> AvaProjectProductFulls { get; set; }
    public List<ProjectProductFull> InvProjectProductFulls { get; set; }
    private Dictionary<int, ProjectProductFull> DictProjectProductFulls { get; set; } = new Dictionary<int, ProjectProductFull>();

    public void SetProjectProductFulls(List<ProjectProductFull> projectProductFulls)
    {
        int count = projectProductFulls.Count;
        List<ProjectProductFull> avaProjectProductFulls = new List<ProjectProductFull>(count);
        List<ProjectProductFull> invProjectProductFulls = new List<ProjectProductFull>(count);

        DictProjectProductFulls = new Dictionary<int, ProjectProductFull>(count);

        foreach (ProjectProductFull project in projectProductFulls)
        {
            DictProjectProductFulls[project.ProjectFull.ProjectId] = project;

            if (project.ProjectFull.Status == 1)
                avaProjectProductFulls.Add(project);
            else
                invProjectProductFulls.Add(project);
        }

        AvaProjectProductFulls = avaProjectProductFulls;
        InvProjectProductFulls = invProjectProductFulls;
    }

    public ProjectProductFull GetProjectProductFullByProjectId(int projectId)
    {
        if (!DictProjectProductFulls.TryGetValue(projectId, out ProjectProductFull projectProductFull))
            return null;
        return projectProductFull;
    }


    // Project Images

    public Dictionary<int, List<Sprite>> dicProjectImages = new Dictionary<int, List<Sprite>>();
    public void AddProjectImages(int productId, String[] stgImages)
    {
        List<Sprite> productImages = new List<Sprite>();
        for (int i = 0; i < stgImages.Length; i++)
            productImages.Add(stgImages[i].CreateSprite($"ProjectImages_{i}"));
        dicProjectImages.Add(productId, productImages);
    }

    public List<Sprite> GetProjectImagesById(int projectId)
    {
        if (!dicProjectImages.TryGetValue(projectId, out List<Sprite> productImages))
            return null;
        return productImages;
    }

    // Project Like

    public List<int> ProjectLikeIds { get; set; } = null;


    // INVESTMENTS

    public List<InvestmentFull> InvestmentFulls { get; set; } = new List<InvestmentFull>();
    //private Dictionary<int, Investment> DictInvestments { get; set; } = new Dictionary<int, Investment>();

    private static readonly HashSet<int> InProgressStatus = new() { 0, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

    public InvestmentFull GetInvestmentById(int id)
    {
        InvestmentFull investmentFull = GetInvestmentFractionatedFullByInvestmentId(id);

        if (investmentFull != null)
            return investmentFull;

        investmentFull = GetInvestmentFinancedFullByInvestmentId(id);

        if (investmentFull != null)
            return investmentFull;

        return GetInvestmentPrepaidFullByInvestmentId(id);
    }

    public List<(InvestmentFull, ProjectFull)> GetInvestmentsByStatus(InvestmentStatus statusFilter)
    {
        List<(InvestmentFull, ProjectFull)> investmentProjects = new List<(InvestmentFull, ProjectFull)>();

        for (int i = 0; i < StateManager.Instance.InvestmentFulls.Count; i++)
        {
            InvestmentFull investmentFull = StateManager.Instance.InvestmentFulls[i];
            bool shouldInclude = false;

            if (statusFilter == InvestmentStatus.Approved && investmentFull.InvestmentStatusId == (int)InvestmentStatus.Approved)
                shouldInclude = true;
            else if (statusFilter == InvestmentStatus.Rejected && investmentFull.InvestmentStatusId == (int)InvestmentStatus.Rejected)
                shouldInclude = true;
            else if (statusFilter == InvestmentStatus.InProgress && InProgressStatus.Contains(investmentFull.InvestmentStatusId))
                shouldInclude = true;

            if (shouldInclude)
            {
                ProjectProductFull projectProductFull = StateManager.Instance.GetProjectProductFullByProjectId(investmentFull.ProjectId);

                investmentProjects.Add((investmentFull, projectProductFull.ProjectFull));
            }
        }

        return investmentProjects;
    }

    public bool UpdateInvestmentStatus(int investmentId, int newStatus)
    {
        if (UpdateInvestmentFractionatedStatus(investmentId, newStatus))
            return true;
        if (UpdateInvestmentFinancedStatus(investmentId, newStatus))
            return true;
        return UpdateInvestmentPrepaidStatus(investmentId, newStatus);
    }

    public void UpdateInvestmentMotive(int[] parameters)
    {
        UpdateInvestmentFractionatedMotive(parameters[0], parameters[1]);
        UpdateInvestmentFinancedMotive(parameters[0], parameters[1]);
        UpdateInvestmentPrepaidMotive(parameters[0], parameters[1]);
    }

    public void UpdateInvestmentMotiveStatus(int[] parameters)
    {
        UpdateInvestmentFractionatedStatusMotive(parameters[0], parameters[1], parameters[2]);
        UpdateInvestmentFinancedStatusMotive(parameters[0], parameters[1], parameters[2]);
        UpdateInvestmentPrepaidStatusMotive(parameters[0], parameters[1], parameters[2]);
    }


    // InvestmentFractionated

    public List<InvestmentFractionatedFull> InvestmentFractionatedFulls { get; set; } = new();
    private Dictionary<int, InvestmentFractionatedFull> DictInvestmentFractionatedFulls { get; set; } = new Dictionary<int, InvestmentFractionatedFull>();

    public InvestmentFractionatedFull GetInvestmentFractionatedFullByInvestmentId(int investmentId)
    {
        if (DictInvestmentFractionatedFulls.TryGetValue(investmentId, out var investment))
            return investment;
        return null;
    }

    public void SetInvestmentFractionatedFulls(List<InvestmentFractionatedFull> investmentFractionatedFulls)
    {
        Dictionary<int, InvestmentFractionatedFull> newDict = new Dictionary<int, InvestmentFractionatedFull>();

        foreach (var investment in investmentFractionatedFulls)
            newDict[investment.InvestmentId] = investment;

        InvestmentFractionatedFulls = investmentFractionatedFulls;
        DictInvestmentFractionatedFulls = newDict;

        InvestmentFulls.AddRange(investmentFractionatedFulls);
        InvestmentFulls.Sort((InvestmentFull i1, InvestmentFull i2) => i1.InvestmentId.CompareTo(i2.InvestmentId));
    }

    public void AddInvestmentFractionatedFull(InvestmentFractionatedFull investmentFractionatedFull)
    {
        if (InvestmentFractionatedFulls == null)
            InvestmentFractionatedFulls = new List<InvestmentFractionatedFull>();
        
        InvestmentFractionatedFulls.Add(investmentFractionatedFull);

        DictInvestmentFractionatedFulls.Add(investmentFractionatedFull.InvestmentId, investmentFractionatedFull);

        InvestmentFulls.Add(investmentFractionatedFull);
    }

    public bool UpdateInvestmentFractionatedStatus(int investmentId, int newStatus)
    {
        if (!DictInvestmentFractionatedFulls.TryGetValue(investmentId, out InvestmentFractionatedFull investmentFractionatedFull))
            return false;
        
        investmentFractionatedFull.InvestmentStatusId = newStatus;
        return true;
    }

    public bool UpdateInvestmentFractionatedMotive(int investmentId, int newMotive)
    {
        if (!DictInvestmentFractionatedFulls.TryGetValue(investmentId, out InvestmentFractionatedFull investmentFractionatedFull))
            return false;

        investmentFractionatedFull.InvestmentMotiveId = newMotive;
        return true;
    }

    public bool UpdateInvestmentFractionatedStatusMotive(int investmentId, int newStatus, int newMotive)
    {
        if (!DictInvestmentFractionatedFulls.TryGetValue(investmentId, out InvestmentFractionatedFull investmentFractionatedFull))
            return false;

        investmentFractionatedFull.InvestmentStatusId = newStatus;
        investmentFractionatedFull.InvestmentMotiveId = newMotive;
        return true;
    }

    public bool UpdateInvestmentFractionated(InvestmentFractionatedFull investmentFractionatedFullNew)
    {
        if (!DictInvestmentFractionatedFulls.TryGetValue(investmentFractionatedFullNew.InvestmentId, out InvestmentFractionatedFull investmentFractionatedFull))
            return false;

        investmentFractionatedFull.Set(investmentFractionatedFullNew);
        return true;
    }

    public void AddInvestmentFractionatedPayment(int investmentId, InvestmentBankPayment investmentBankPayment)
    {
        if (!DictInvestmentFractionatedFulls.TryGetValue(investmentId, out InvestmentFractionatedFull investmentFractionatedFull))
            return;

        investmentFractionatedFull.InvestmentBankPayments.Add(investmentBankPayment);

        //for (int i = 0; i < InvestmentFractionatedFulls.Count; i++)
        //{
        //    if (InvestmentFractionatedFulls[i].InvestmentId == investmentId)
        //    {
        //        InvestmentFractionatedFulls[i].InvestmentBankPayments.Add(investmentBankPayment);
        //        break;
        //    }
        //}
    }

    // InvestmentFinanced

    public List<InvestmentFinancedFull> InvestmentFinancedFulls { get; set; } = new();
    private Dictionary<int, InvestmentFinancedFull> DictInvestmentFinancedFulls { get; set; } = new Dictionary<int, InvestmentFinancedFull>();

    public InvestmentFinancedFull GetInvestmentFinancedFullByInvestmentId(int investmentId)
    {
        if (DictInvestmentFinancedFulls.TryGetValue(investmentId, out InvestmentFinancedFull investment))
            return investment;
        return null;
    }

    public void SetInvestmentFinancedFulls(List<InvestmentFinancedFull> investmentFinancedFulls)
    {
        var newDict = new Dictionary<int, InvestmentFinancedFull>();

        foreach (var investment in investmentFinancedFulls)
            newDict[investment.InvestmentId] = investment;

        InvestmentFinancedFulls = investmentFinancedFulls;
        DictInvestmentFinancedFulls = newDict;

        InvestmentFulls.AddRange(investmentFinancedFulls);
        InvestmentFulls.Sort((InvestmentFull i1, InvestmentFull i2) => i1.InvestmentId.CompareTo(i2.InvestmentId));
    }

    public void AddInvestmentFinancedFull(InvestmentFinancedFull investmentFinancedFull)
    {
        if (InvestmentFinancedFulls == null)
            InvestmentFinancedFulls = new List<InvestmentFinancedFull>();

        InvestmentFinancedFulls.Add(investmentFinancedFull);
        DictInvestmentFinancedFulls.Add(investmentFinancedFull.InvestmentId, investmentFinancedFull);

        InvestmentFulls.Add(investmentFinancedFull);
    }

    public bool UpdateInvestmentFinancedStatus(int investmentId, int newStatus)
    {
        if (!DictInvestmentFinancedFulls.TryGetValue(investmentId, out var investmentFinancedFull))
            return false;

        investmentFinancedFull.InvestmentStatusId = newStatus;
        return true;
    }

    public bool UpdateInvestmentFinancedMotive(int investmentId, int newMotive)
    {
        if (!DictInvestmentFinancedFulls.TryGetValue(investmentId, out var investmentFinancedFull))
            return false;

        investmentFinancedFull.InvestmentMotiveId = newMotive;
        return true;
    }

    public bool UpdateInvestmentFinancedStatusMotive(int investmentId, int newStatus, int newMotive)
    {
        if (!DictInvestmentFinancedFulls.TryGetValue(investmentId, out var investmentFinancedFull))
            return false;

        investmentFinancedFull.InvestmentStatusId = newStatus;
        investmentFinancedFull.InvestmentMotiveId = newMotive;
        return true;
    }

    public bool UpdateInvestmentFinanced(InvestmentFinancedFull investmentFinancedFullNew)
    {
        if (!DictInvestmentFinancedFulls.TryGetValue(investmentFinancedFullNew.InvestmentId, out InvestmentFinancedFull investmentFinancedFull))
            return false;

        investmentFinancedFull.Set(investmentFinancedFullNew);
        return true;
    }

    public void AddInvestmentFinancedPayment(int investmentId, InvestmentBankPayment investmentBankPayment)
    {
        if (!DictInvestmentFinancedFulls.TryGetValue(investmentId, out InvestmentFinancedFull investmentFinancedFull))
            return;
        
        investmentFinancedFull.InvestmentBankPayments.Add(investmentBankPayment);

        //for (int i = 0; i < InvestmentFinancedFulls.Count; i++)
        //{
        //    if (InvestmentFinancedFulls[i].InvestmentId == investmentId)
        //    {
        //        InvestmentFinancedFulls[i].InvestmentBankPayments.Add(investmentBankPayment);
        //        break;
        //    }
        //}
    }

    // InvestmentPrepaid

    public List<InvestmentPrepaidFull> InvestmentPrepaidFulls { get; set; } = new();
    private Dictionary<int, InvestmentPrepaidFull> DictInvestmentPrepaidFulls { get; set; } = new Dictionary<int, InvestmentPrepaidFull>();

    public InvestmentPrepaidFull GetInvestmentPrepaidFullByInvestmentId(int investmentId)
    {
        if (DictInvestmentPrepaidFulls.TryGetValue(investmentId, out InvestmentPrepaidFull investment))
            return investment;
        return null;
    }

    public void SetInvestmentPrepaidFulls(List<InvestmentPrepaidFull> investmentPrepaidFulls)
    {
        Dictionary<int, InvestmentPrepaidFull> newDict = new Dictionary<int, InvestmentPrepaidFull>();

        foreach (var investment in investmentPrepaidFulls)
            newDict[investment.InvestmentId] = investment;

        InvestmentPrepaidFulls = investmentPrepaidFulls;
        DictInvestmentPrepaidFulls = newDict;

        InvestmentFulls.AddRange(investmentPrepaidFulls);
        InvestmentFulls.Sort((InvestmentFull i1, InvestmentFull i2) => i1.InvestmentId.CompareTo(i2.InvestmentId));
    }

    public void AddInvestmentPrepaidFull(InvestmentPrepaidFull investmentPrepaidFull)
    {
        if (InvestmentPrepaidFulls == null)
            InvestmentPrepaidFulls = new List<InvestmentPrepaidFull>();

        InvestmentPrepaidFulls.Add(investmentPrepaidFull);
        DictInvestmentPrepaidFulls.Add(investmentPrepaidFull.InvestmentId, investmentPrepaidFull);

        InvestmentFulls.Add(investmentPrepaidFull);
    }

    public bool UpdateInvestmentPrepaidStatus(int investmentId, int newStatus)
    {
        if (!DictInvestmentPrepaidFulls.TryGetValue(investmentId, out var investmentPrepaidFull))
            return false;
        
        investmentPrepaidFull.InvestmentStatusId = newStatus;
        return true;
    }

    public bool UpdateInvestmentPrepaidMotive(int investmentId, int newMotive)
    {
        if (!DictInvestmentPrepaidFulls.TryGetValue(investmentId, out var investmentPrepaidFull))
            return false;

        investmentPrepaidFull.InvestmentMotiveId = newMotive;
        return true;
    }

    public bool UpdateInvestmentPrepaidStatusMotive(int investmentId, int newStatus, int newMotive)
    {
        if (!DictInvestmentPrepaidFulls.TryGetValue(investmentId, out var investmentPrepaidFull))
            return false;

        investmentPrepaidFull.InvestmentStatusId = newStatus;
        investmentPrepaidFull.InvestmentMotiveId = newMotive;
        return true;
    }

    public bool UpdateInvestmentPrepaid(InvestmentPrepaidFull investmentPrepaidFullNew)
    {
        if (!DictInvestmentPrepaidFulls.TryGetValue(investmentPrepaidFullNew.InvestmentId, out InvestmentPrepaidFull investmentPrepaidFull))
            return false;

        investmentPrepaidFull.Set(investmentPrepaidFullNew);
        return true;
    }

    public void AddInvestmentPrepaidPayment(int investmentId, InvestmentBankPayment investmentBankPayment)
    {
        if (!DictInvestmentPrepaidFulls.TryGetValue(investmentId, out InvestmentPrepaidFull investmentPrepaidFull))
            return;

        investmentPrepaidFull.InvestmentBankPayments.Add(investmentBankPayment);

        //for (int i = 0; i < InvestmentPrepaidFulls.Count; i++)
        //{
        //    if (InvestmentPrepaidFulls[i].InvestmentId == investmentId)
        //    {
        //        InvestmentPrepaidFulls[i].InvestmentBankPayments.Add(investmentBankPayment);
        //        break;
        //    }
        //}
    }

    // InvestmentType

    public int GetInvestmentTypeByInvestmentId(int investmentId)
    {
        if (DictInvestmentFractionatedFulls.ContainsKey(investmentId))
            return 1;
        
        if (DictInvestmentFinancedFulls.ContainsKey(investmentId))
            return 2;

        if (DictInvestmentPrepaidFulls.ContainsKey(investmentId))
            return 3;
        
        return -1;
    }

    public void ClearAll()
    {
        NewsInfos = null;
        ReferredCount = null;
        AppUser = null;
        Address = null;
        Identity = null;
        Card = null;
        Portrait = null;

        AvaProjectProductFulls = null;
        InvProjectProductFulls = null;
        DictProjectProductFulls.Clear();
        dicProjectImages.Clear();

        ProjectLikeIds = null;

        InvestmentFulls.Clear();
        InvestmentFractionatedFulls.Clear();
        DictInvestmentFractionatedFulls.Clear();
        InvestmentFinancedFulls.Clear();
        DictInvestmentFinancedFulls.Clear();
        InvestmentPrepaidFulls.Clear();
        DictInvestmentPrepaidFulls.Clear();
    }
}