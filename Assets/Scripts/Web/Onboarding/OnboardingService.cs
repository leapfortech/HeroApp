using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class OnboardingService : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onDpiFrontUpdated = null;

    [SerializeField]
    private UnityEvent onDpiBackUpdated = null;

    [SerializeField]
    private UnityIntEvent onIdentityInfoUpdated = null;

    [SerializeField]
    private UnityEvent onPortraitUpdated = null;

    [SerializeField]
    private UnityEvent onHouseholdBillUpdated = null;

    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;
 
    // Dpi
    public void UpdateDpiFront(int appUserId, String dpiPhotos)
    {
        ObdDpiFrontPutOperation obdDpiFrontPutOp = new ObdDpiFrontPutOperation();
        try
        {
            obdDpiFrontPutOp.appUserId = appUserId;
            obdDpiFrontPutOp.dpiPhotos = "\"" + dpiPhotos + "\"";
            obdDpiFrontPutOp["on-complete"] = (Action<ObdDpiFrontPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onDpiFrontUpdated.Invoke();
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            obdDpiFrontPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void UpdateDpiBack(int appUserId, String dpiBack)
    {
        ObdDpiBackPutOperation obdDpiBackPutOp = new ObdDpiBackPutOperation();
        try
        {
            obdDpiBackPutOp.appUserId = appUserId;
            obdDpiBackPutOp.dpiBack = "\"" + dpiBack + "\"";
            obdDpiBackPutOp["on-complete"] = (Action<ObdDpiBackPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onDpiBackUpdated.Invoke();
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            obdDpiBackPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // Identity
    public void UpdateIdentityInfo(IdentityInfo identityInfo)
    {
        ObdIdentityInfoPutOperation obdIdentityInfoPutOp = new ObdIdentityInfoPutOperation();
        try
        {
            obdIdentityInfoPutOp.identityInfo = identityInfo;
            obdIdentityInfoPutOp["on-complete"] = (Action<ObdIdentityInfoPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onIdentityInfoUpdated.Invoke(Convert.ToInt32(op.id));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            obdIdentityInfoPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // Portrait
    public void UpdatePortrait(int appUserId, String portrait)
    {
        ObdPortraitPutOperation obdPortraitPutOp = new ObdPortraitPutOperation();
        try
        {
            obdPortraitPutOp.appUserId = appUserId;
            obdPortraitPutOp.portrait = "\"" + portrait + "\"";
            obdPortraitPutOp["on-complete"] = (Action<ObdPortraitPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onPortraitUpdated.Invoke();
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            obdPortraitPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // Address
    public void UpdateHouseholdBill(int appUserId, String[] householdBills)
    {
        ObdHouseholdBillPutOperation obdHouseholdBillPutOp = new ObdHouseholdBillPutOperation();
        try
        {
            obdHouseholdBillPutOp.appUserId = appUserId;
            obdHouseholdBillPutOp.householdBills = householdBills; // "\"" + householdBills + "\"";
            obdHouseholdBillPutOp["on-complete"] = (Action<ObdHouseholdBillPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onHouseholdBillUpdated.Invoke();
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            obdHouseholdBillPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}
