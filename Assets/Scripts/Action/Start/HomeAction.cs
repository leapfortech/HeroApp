using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.Data.Web;
using Leap.UI.Page;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;

public class HomeAction : MonoBehaviour
{
    [Title("Hello")]
    [SerializeField]
    Text txtAppUserName = null;
    [SerializeField]
    Text txtReferredCount = null;

    [SerializeField]
    String link = null;

    [Title("List")]
    [SerializeField]
    ListScroller lstMeeting;
    [SerializeField]
    Text txtEmpty;

    [Title("Onboarding")]
    [SerializeField]
    Image imgObdNew = null;
    [SerializeField]
    Image imgObdUpdate = null;
    [SerializeField]
    Image imgObdWait = null;
    [SerializeField]
    Image imgInvestments = null;

    [SerializeField]
    Text txtStage = null;

    [PropertySpace(5f)]
    [SerializeField]
    String[] stages = null;

    [Title("Flow")]
    [SerializeField]
    PageFlow flowIdentity = null;

    [SerializeField]
    PageFlow flowAddress = null;

    [Title("Action")]
    [SerializeField]
    UnityEvent onAddressStarted = null;

    [Title("Action")]
    [SerializeField]
    Button btnStart = null;
    [SerializeField]
    Button btnUpdate = null;
    [SerializeField]
    Button btnWait = null;
    [SerializeField]
    Button btnResume = null;

    [Title("Pages")]
    [SerializeField]
    Page pagObdStart = null;
    [SerializeField]
    Page pagAddressIntro = null;
    [SerializeField]
    Page obdContinuePage = null;
    [SerializeField]
    Page obdWaitPage = null;
    [SerializeField]
    Page obdUpdatePage = null;
    [SerializeField]
    Page obdApprovedPage = null;
    [SerializeField]
    Page obdRejectedPage = null;

    private void Start()
    {
        btnStart?.AddAction(ChangeStartPage);
        btnUpdate?.AddAction(ChangeStartPage);
        btnWait?.AddAction(ChangeStartPage);
        btnResume?.AddAction(ChangeResumePage);
    }

    public void Refresh()
    {
        RefreshAppUser();
    }

    public void DoLink()
    {
        Application.OpenURL(link);
    }

    public void ObdFinalize(int status)
    {
        if (status == 5)
        {
            StateManager.Instance.AppUser.AppUserStatusId = 1;
            PageManager.Instance.ChangePage(obdApprovedPage);
        }
        else
        {
            StateManager.Instance.AppUser.AppUserStatusId = 6;
            PageManager.Instance.ChangePage(obdRejectedPage);
        }
    }

    public void RefreshAppUser()
    {
        int appUserStatusId = StateManager.Instance.AppUser.AppUserStatusId;
        int onboardingStage = StateManager.Instance.OnboardingStage;

        if (appUserStatusId == 0)
            txtAppUserName.TextValue = WebManager.Instance.WebSysUser.Email.Split('@')[0];
        else
            txtAppUserName.TextValue = StateManager.Instance.Identity.FirstName1;

        imgObdNew.gameObject.SetActive(appUserStatusId == 0 || appUserStatusId == 2);
        imgObdWait.gameObject.SetActive(appUserStatusId == 3 && onboardingStage == 0);
        imgObdUpdate.gameObject.SetActive(appUserStatusId == 3 && onboardingStage != 0);
        imgInvestments.gameObject.SetActive(appUserStatusId == 1);

        String txtCount = StateManager.Instance.ReferredCount.Count == 0 ? "No tienes referidos" : "Tienes " + StateManager.Instance.ReferredCount.Count.ToString() + (StateManager.Instance.ReferredCount.Count == 1 ? " referido" : " referidos");

        txtReferredCount.TextValue = txtCount;
    }

    // Onboarding

    public void ChangeStartPage()
    {
        int appUserStatusId = StateManager.Instance.AppUser.AppUserStatusId;
        int onboardingStage = StateManager.Instance.OnboardingStage;

        if (appUserStatusId == 0)
        {
            flowIdentity.Clear();

            PageManager.Instance.ChangePage(pagObdStart);
        }
        else if (appUserStatusId == 2)
        {
            flowAddress.Clear();
            onAddressStarted.Invoke();

            txtStage.TextValue = Regex.Unescape(stages[appUserStatusId == 0 ? appUserStatusId : appUserStatusId - 1]);
            PageManager.Instance.ChangePage(obdContinuePage);
        }
        else if (appUserStatusId == 3)
        {
            if (onboardingStage == 0)
            {
                PageManager.Instance.ChangePage(obdWaitPage);
                return;
            }
            else
            {
                PageManager.Instance.ChangePage(obdUpdatePage);
                return;
            }
        }
        else if (appUserStatusId == 4)
        {
            PageManager.Instance.ChangePage(obdRejectedPage);
            return;
        }
        else
        {
            ChoiceDialog.Instance.Error("Login", "Unknown Status #" + appUserStatusId);
            return;
        }
    }

    private void ChangeResumePage()
    {
        PageManager.Instance.ChangePage(pagAddressIntro);
    }
}