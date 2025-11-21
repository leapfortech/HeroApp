using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Dialog;
using Leap.UI.Extensions;

using Sirenix.OdinInspector;

public class ReferredDisplayAction : MonoBehaviour
{
    [Title("Display")]
    [SerializeField]
    ComboAdapter cmbPeriod;
    [SerializeField]
    FieldValue fldCount = null;
    [SerializeField]
    FieldValue fldInvestmentCount = null;

    [Title("List")]
    [SerializeField]
    ListScroller lstReferred;
    [SerializeField]
    Text txtEmpty;

    ReferredService referredService;

    List<Referred> referreds;
    bool firstDisplay = true;

    private void Awake()
    {
        referredService = GetComponent<ReferredService>();
    }

    public void Clear()
    {
        firstDisplay = true;
        lstReferred.ClearValues();
        referreds.Clear();
    }

    public void FillCurrentPeriod()
    {
        fldCount.TextValue = StateManager.Instance.ReferredCount.Count.ToString("N0");
        fldInvestmentCount.TextValue = StateManager.Instance.ReferredCount.InvestmentCount.ToString("N0");

        if (!firstDisplay)
            return;

        firstDisplay = false;

        if (cmbPeriod.Combo.IsEmpty())
            cmbPeriod.SelectIndexes((DateTime.Today.Month - 1) / 3, DateTime.Today.Year - 2023);

        FillReferreds();
    }

    public void FillReferreds()
    {
        if (cmbPeriod.Combo.IsEmpty())
            return;

        ScreenDialog.Instance.Display();

        int quarter = cmbPeriod.GetSelectedId(0);
        int year = cmbPeriod.GetSelectedId(1);

        DateTime dateStart = new DateTime(2023 + year, 1 + quarter * 3, 1);
        DateTime dateEnd = dateStart.AddMonths(3).AddDays(-1);


        if (referredService == null)
            referredService = GetComponent<ReferredService>();

        referredService.GetByPeriod(dateStart, dateEnd);
    }


    public void ApplyReferreds(List<Referred> referreds)
    {
        this.referreds = referreds;
        for (int i = 0; i < referreds.Count; i++)
            this.referreds[i].CreateDateTime = this.referreds[i].CreateDateTime.ToLocalTime();
        DisplayFiltered();
    }

    public void DisplayFiltered()
    {
        lstReferred.Clear();

        if (referreds.Count == 0)
        {
            txtEmpty.gameObject.SetActive(true);
            ScreenDialog.Instance.Hide();
            return;
        }

        txtEmpty.gameObject.SetActive(false);

        for (int i = 0; i < referreds.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(3, true);
            scrollerValue.SetText(0, referreds[i].Id.ToString());
            //scrollerValue.SetText(1, $"{referreds[i].FirstName} {referreds[i].LastName}");
            scrollerValue.SetText(2, referreds[i].CreateDateTime.ToString("dd/MM/yyyy HH:mm"));

            lstReferred.AddValue(scrollerValue);
        }

        lstReferred.ApplyValues();

        ScreenDialog.Instance.Hide();
    }
}
