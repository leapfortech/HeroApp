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

    [Title("List")]
    [SerializeField]
    ListScroller lstReferred;
    [SerializeField]
    Text txtEmpty;

    ReferredService referredService;

    List<ReferredFull> referredFulls;
    bool firstDisplay = true;

    private void Awake()
    {
        referredService = GetComponent<ReferredService>();
    }

    public void Clear()
    {
        firstDisplay = true;
        lstReferred.ClearValues();
        referredFulls?.Clear();
    }

    public void FillCurrentPeriod()
    {
        fldCount.TextValue = StateManager.Instance.ReferredCount.Count.ToString("N0");

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

        long quarter = cmbPeriod.GetSelectedId(0);
        long year = cmbPeriod.GetSelectedId(1);

        DateTime dateStart = new DateTime(2023 + (int)year, 1 + (int)quarter * 3, 1);
        DateTime dateEnd = dateStart.AddMonths(3).AddDays(-1);


        if (referredService == null)
            referredService = GetComponent<ReferredService>();

        referredService.GetByPeriod(dateStart, dateEnd);
    }


    public void ApplyReferreds(List<ReferredFull> referredFulls)
    {
        this.referredFulls = referredFulls;
        for (int i = 0; i < referredFulls.Count; i++)
            this.referredFulls[i].CreateDateTime = this.referredFulls[i].CreateDateTime.ToLocalTime();
        DisplayFiltered();
    }

    public void DisplayFiltered()
    {
        lstReferred.Clear();

        if (referredFulls == null || referredFulls.Count == 0)
        {
            txtEmpty.gameObject.SetActive(true);
            ScreenDialog.Instance.Hide();
            return;
        }

        txtEmpty.gameObject.SetActive(false);

        for (int i = 0; i < referredFulls.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(3, true);
            scrollerValue.SetText(0, referredFulls[i].Code);
            scrollerValue.SetText(1, $"{referredFulls[i].FirstName1} {referredFulls[i].LastName1}");
            scrollerValue.SetText(2, referredFulls[i].CreateDateTime.ToString("dd/MM/yyyy HH:mm"));

            lstReferred.AddValue(scrollerValue);
        }

        lstReferred.ApplyValues();

        ScreenDialog.Instance.Hide();
    }
}
