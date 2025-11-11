using System;
using System.Threading.Tasks;
using UnityEngine;

#if !UNITY_WEBGL
using Firebase.Auth;
using Firebase.Extensions;
#endif

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Web;
using UnityEngine.Events;

public class PasswordChangeAction : MonoBehaviour
{
    [Serializable]
    public class UnityPasswordChangedEvent : UnityEvent { }

    [Header("Fields")]
    [SerializeField]
    InputField ifdCurrentPassword = null;

    [SerializeField]
    InputField ifdNewPassword = null;

    [SerializeField]
    InputField ifdConfirmPassword = null;

    [Header("Action")]
    [SerializeField]
    Button btnSave = null;

    [Header("Result")]
    [SerializeField]
    Page nextPage = null;

#pragma warning disable 414
    [Header("Event")]
    [SerializeField]
    UnityPasswordChangedEvent onPasswordChanged = null;

    [Header("Messages")]
    [SerializeField, TextArea(2, 5)]
    String noInternetError = "Cannot change your Password.\r\nPlease, check your internet connection and try again.";
    [SerializeField, TextArea(2, 5)]
    String passwordError = "The current password is incorrect.\r\n\r\nPlease, check it and try again.";
    [SerializeField, TextArea(2, 5)]
    String changeError = "Cannot change your Password.\r\nPlease, try again.";
#pragma warning restore 414

    WebSysUserService webSysUserService;
    ElementValue[] elementValues = null;

    private void Awake()
    {
        webSysUserService = GetComponent<WebSysUserService>();

        elementValues = new ElementValue[3];
        elementValues[0] = ifdCurrentPassword;
        elementValues[1] = ifdNewPassword;
        elementValues[2] = ifdConfirmPassword;
    }

    private void Start()
    {
        btnSave?.AddAction(Do);
    }

    public void Clear()
    {
        for (int i = 0; i < elementValues.Length; i++)
            elementValues[i].Clear();
    }

    // Change Password
#if !UNITY_WEBGL
    public async void Do()
#else
    public void Do()
#endif
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        if (ifdNewPassword.Text != ifdConfirmPassword.Text)
        {
            ifdNewPassword.DisplayValidity(false);
            ifdConfirmPassword.DisplayValidity(false);
            ChoiceDialog.Instance.Error(PageManager.Instance.CurrentPage.HeaderTitle, "Los campos <b>" + ifdNewPassword.Title + "</b> y <b>" + ifdConfirmPassword.Title + "</b> son diferentes. Por favor, revisa e intenta de nuevo.");
            return;
        }

        ScreenDialog.Instance.Display();

#if !UNITY_WEBGL
        try
        {
            Credential credential = FirebaseManager.Instance.GetCredential(ifdCurrentPassword.Text);
            await FirebaseManager.Instance.Auth.CurrentUser.ReauthenticateAsync(credential).ContinueWithOnMainThread(VerifyPassword);
        }
        catch (Exception aex)
        {
            ChoiceDialog.Instance.Error(PageManager.Instance.CurrentPage.HeaderTitle, aex.Message);
        }
#else
        webSysUserService.ChangePassword(WebManager.Instance.WebSysUser.Id, WebManager.Instance.WebSysUser.AuthUserId, ifdCurrentPassword.Text);
#endif
    }

#if !UNITY_WEBGL
    private async void VerifyPassword(Task task)
    {
        if (task.IsCanceled)
        {
            return;
        }
        if (task.IsFaulted)
        {
            ChoiceDialog.Instance.Error(PageManager.Instance.CurrentPage.HeaderTitle, passwordError);
            return;
        }

        try
        {
            await FirebaseManager.Instance.Auth.CurrentUser.UpdatePasswordAsync(ifdNewPassword.Text).ContinueWithOnMainThread(ChangePassword);
        }
        catch (Exception aex)
        {
            ChoiceDialog.Instance.Error(PageManager.Instance.CurrentPage.HeaderTitle, aex.Message);
        }
    }

    private void ChangePassword(Task task)
    {
        if (task.IsCanceled)
        {
            return;
        }
        if (task.IsFaulted)
        {
            ChangePasswordErrorMessage();
            return;
        }

        Clear();
        onPasswordChanged?.Invoke();
        PageManager.Instance.ChangePage(nextPage);
    }
#endif

    public void PasswordApply(int result)
    {
        if (result == -1)
        {
            ChoiceDialog.Instance.Error(PageManager.Instance.CurrentPage.HeaderTitle, "Error : Unknown user.");
            return;
        }

        if (result == 0)
        {
            ChangePasswordErrorMessage();
            return;
        }

        Clear();
        PageManager.Instance.ChangePage(nextPage);
    }

    // Messages
    public void InternetErrorMessage()
    {
        ChoiceDialog.Instance.Error(PageManager.Instance.CurrentPage.HeaderTitle, noInternetError);
    }

    public void ChangePasswordErrorMessage()
    {
        ChoiceDialog.Instance.Error(PageManager.Instance.CurrentPage.HeaderTitle, changeError);
    }
}
