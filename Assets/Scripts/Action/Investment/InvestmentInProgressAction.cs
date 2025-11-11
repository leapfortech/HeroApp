using System;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Collections;

using Sirenix.OdinInspector;


public class InvestmentInProgressAction : MonoBehaviour
{
    [Serializable]
    public class InvestmentPaymentInfoEvent : UnityEvent<InvestmentPaymentInfo> { }
    [Serializable]
    public class InvestmentInfoEvent : UnityEvent<(int investmentId, bool isUpdate)> { }

    [Space]
    [SerializeField]
    String[] nameProducts = null;

    [Title("Summary")]   
    [SerializeField]
    Text txtInProgressCount = null;
    [SerializeField]
    Text txtApprovedCount = null;

    [Title("Investment")]
    [SerializeField]
    ToggleGroup tggDisplay = null;
    [SerializeField]
    ListScroller lstInvestment;
    [SerializeField]
    Text txtEmpty = null;

    [SerializeField, Space]
    String[] statusTitles;

    [Title("Event")]
    [SerializeField]
    InvestmentInfoEvent onInvestmentSelected = null;
    [SerializeField]
    UnityEvent onPagNext = null;

    [Title("Page")]
    [SerializeField]
    Page pagInvestmentIntro = null;
    [SerializeField]
    Page pagBack = null;
    [SerializeField]
    Page pagReserveIntro = null;
    [SerializeField]
    Page pagDocRtuIntro = null;
    [SerializeField]
    Page pagDocIncomeIntro = null;
    [SerializeField]
    Page pagDocBankIntro = null;
    [SerializeField]
    Page pagReferencesIntro = null;
    [SerializeField]
    Page pagSignatoriesIntro = null;
    [SerializeField]
    Page pagBeneficiariesIntro = null;

    [Title("Update")]
    [SerializeField]
    Text txtUpdateMotive = null;
    [SerializeField]
    Text txtUpdateComment = null;
    [SerializeField]
    Button btnUpdateNext = null;
    [SerializeField]
    Page pagUpdateIntro = null;
    [SerializeField]
    ValueList vllInvestmentUpdateMotive = null;

    [Title("Rejected")]
    [SerializeField]
    Text txtRejectedMotive = null;
    [SerializeField]
    ValueList vllInvestmentRejectMotive = null;
    [SerializeField]
    Page pagRejected = null;

    List<(InvestmentFull InvestmentFull, ProjectFull ProjectFull)> investmentProjects = new();

    private void Start()
    {
        btnUpdateNext?.AddAction(ChangeUpdatePage);
    }

    public void Clear()
    {
        investmentProjects.Clear();
        lstInvestment.ClearValues();
    }

    bool isSdw = false;
    public void SetBackPage(bool isSdw)
    {
        this.isSdw = isSdw;
    }

    public void ChangeBackPage()
    {
        PageManager.Instance.ChangePage(pagBack);

        if (isSdw)
            SandwichMenu.Instance.Open();
    }

    public void ChangeNextPage()
    {
        if (StateManager.Instance.AppUser.AppUserStatusId == 1)
            PageManager.Instance.ChangePage(pagInvestmentIntro);
        else
            onPagNext.Invoke();

        SandwichMenu.Instance.Close();
    }

    public void DisplaySummary()
    {
        int approvedCount = 0;
        int inProgressCount = 0;
        int rejectedCount = 0;

        foreach (InvestmentFull investmentFull in StateManager.Instance.InvestmentFulls)
        {
            if (investmentFull.InvestmentStatusId == (int)InvestmentStatus.Approved)
                approvedCount++;
            else if (investmentFull.InvestmentStatusId == (int)InvestmentStatus.Rejected)
                rejectedCount++;
            else
                inProgressCount++;
        }

        txtInProgressCount.TextValue = inProgressCount.ToString();
        txtApprovedCount.TextValue = approvedCount.ToString();
    }

    public void Refresh()
    {
        DisplaySummary();
        DisplayInvestments();
    }

    public void DisplayInvestments()
    {
        investmentProjects = StateManager.Instance.GetInvestmentsByStatus(tggDisplay.Value == "1" ? InvestmentStatus.InProgress : InvestmentStatus.Rejected);

        if (investmentProjects.Count == 0)
        {
            lstInvestment.Clear();
            txtEmpty.gameObject.SetActive(true);
            return;
        }

        lstInvestment.Clear();

        txtEmpty.gameObject.SetActive(false);

        String status;

        for (int i = 0; i < investmentProjects.Count; i++)
        {
            if (tggDisplay.Value != "1")
                status = vllInvestmentRejectMotive.FindRecordCellString(investmentProjects[i].InvestmentFull.InvestmentMotiveId, "Name");
            else
            {
                if (investmentProjects[i].InvestmentFull.InvestmentStatusId != 9 || (investmentProjects[i].InvestmentFull.InvestmentStatusId == 9 && investmentProjects[i].InvestmentFull.InvestmentMotiveId == 0))
                    status = statusTitles[investmentProjects[i].InvestmentFull.InvestmentStatusId];
                else
                    status = "Requiere actualización";
            }

            ListScrollerValue scrollerValue = new ListScrollerValue(8, true);
            scrollerValue.SetText(0, nameProducts[investmentProjects[i].InvestmentFull.ProductTypeId - 1]);
            scrollerValue.SetText(1, investmentProjects[i].ProjectFull.Name);
            scrollerValue.SetText(2, "$" + investmentProjects[i].InvestmentFull.TotalAmount.ToString("N2", CultureInfo.InvariantCulture));  // investmentProjects[i].InvestmentFull.CpiCount + " CPI's    $" + 

            if (investmentProjects[i].InvestmentFull.InvestmentStatusId == 2 || 
                (investmentProjects[i].InvestmentFull.InvestmentStatusId == 9 && investmentProjects[i].InvestmentFull.InvestmentMotiveId == 0)) // Wait Board
            {
                scrollerValue.SetText(3, "");
                scrollerValue.SetText(4, "");
                scrollerValue.SetText(5, status);
                scrollerValue.SetText(6, "");
            }
            else if (investmentProjects[i].InvestmentFull.InvestmentStatusId == 11 ||
                    (investmentProjects[i].InvestmentFull.InvestmentStatusId == 9 && investmentProjects[i].InvestmentFull.InvestmentMotiveId != 0)) // Rejected
            {
                scrollerValue.SetText(3, "");
                scrollerValue.SetText(4, "");
                scrollerValue.SetText(5, "");
                scrollerValue.SetText(6, status);
            }
            else // Progress
            {
                scrollerValue.SetText(3, "");
                scrollerValue.SetText(4, status);
                scrollerValue.SetText(5, "");
                scrollerValue.SetText(6, "");
            }

            scrollerValue.SetSprite(7, investmentProjects[i].ProjectFull.CoverSprite);

            lstInvestment.AddValue(scrollerValue);
        }
        lstInvestment.ApplyValues();
    }

    public void SelectInvestment(int idx)
    {
        switch (investmentProjects[idx].InvestmentFull.InvestmentStatusId)
        {
            case 0:
                onInvestmentSelected.Invoke((investmentProjects[idx].InvestmentFull.InvestmentId, false));
                PageManager.Instance.ChangePage(pagReserveIntro);
                break;
            case 1:
                break;
            case 2:
                ChoiceDialog.Instance.Info("En validación", "Nos encontramos revisando tu información, pronto tendrás noticias nuestras.");
                break;
            case 3:
                onInvestmentSelected.Invoke((investmentProjects[idx].InvestmentFull.InvestmentId, false));
                PageManager.Instance.ChangePage(pagDocRtuIntro);
                break;
            case 4:
                onInvestmentSelected.Invoke((investmentProjects[idx].InvestmentFull.InvestmentId, false));
                PageManager.Instance.ChangePage(pagDocIncomeIntro);
                break;
            case 5:
                onInvestmentSelected.Invoke((investmentProjects[idx].InvestmentFull.InvestmentId, false));
                PageManager.Instance.ChangePage(pagDocBankIntro);
                break;
            case 6:
                onInvestmentSelected.Invoke((investmentProjects[idx].InvestmentFull.InvestmentId, false));
                PageManager.Instance.ChangePage(pagReferencesIntro);
                break;
            case 7:
                onInvestmentSelected.Invoke((investmentProjects[idx].InvestmentFull.InvestmentId, false));
                PageManager.Instance.ChangePage(pagSignatoriesIntro);
                break;
            case 8:
                onInvestmentSelected.Invoke((investmentProjects[idx].InvestmentFull.InvestmentId, false));
                PageManager.Instance.ChangePage(pagBeneficiariesIntro);
                break;
            case 9:
                VerificationStart(idx);
                break;
            case 10:
                ChoiceDialog.Instance.Info("Compraventa", "Nos encontramos en proceso de generación y firma de compraventa.");
                break;
            case 11:
                //ChoiceDialog.Instance.Error("Solicitud rechazada", "Tu solicitud fue rechazada porque no cumplió con los requisitos establecidos, si deseas puedes iniciar una nueva inversión.");
                txtRejectedMotive.TextValue = investmentProjects[idx].InvestmentFull.BoardComment;
                PageManager.Instance.ChangePage(pagRejected);
                break;
        }
    }

    int motiveId = -1;
    private void VerificationStart(int idx)
    {
        motiveId = investmentProjects[idx].InvestmentFull.InvestmentMotiveId;

        if (motiveId == 0)
        {
            ChoiceDialog.Instance.Info("En validación", "Nos encontramos revisando tu información, pronto tendrás noticias nuestras.");
            return;
        }

        String motive = vllInvestmentUpdateMotive.FindRecordCellString(motiveId, "Name");

        txtUpdateMotive.TextValue = $"Tu solicitud requiere la actualización de los datos de <b>{motive}</b>.";
        txtUpdateComment.TextValue = investmentProjects[idx].InvestmentFull.BoardComment;
        onInvestmentSelected.Invoke((investmentProjects[idx].InvestmentFull.InvestmentId, true));
        PageManager.Instance.ChangePage(pagUpdateIntro);
    }

    public void ChangeUpdatePage()
    {
        switch (motiveId)
        {
            case 1:
                PageManager.Instance.ChangePage(pagDocRtuIntro);
                break;
            case 2:
            case 3:
                PageManager.Instance.ChangePage(pagDocIncomeIntro);
                break;
            case 4:
                PageManager.Instance.ChangePage(pagDocBankIntro);
                break;
            case 5:
                PageManager.Instance.ChangePage(pagReferencesIntro);
                break;
        }
    }
}