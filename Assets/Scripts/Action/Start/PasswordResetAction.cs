using System;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_ANDROID
using Leap.Core.Security;
#endif
using Leap.UI.Elements;
using Leap.UI.Dialog;
using Leap.Data.Web;

public class PasswordResetAction : MonoBehaviour
{
    [Header("Fields")]
    [SerializeField]
    InputField ifdEmail = null;

    [SerializeField]
    String dialogTitle = "Reiniciar contraseña";

    [SerializeField]
    String dialogMessage = "¿Estás seguro de reiniciar tu contraseña?";

    [Header("Action")]
    [SerializeField]
    Button btnReset = null;

    [Header("Messages")]
    [SerializeField, TextArea(2, 5)]
    String noInternetError = "No tienes conexión a internet. Revisa e intenta de nuevo.";
    [SerializeField, TextArea(2, 5)]
    String resetDone = "Hemos enviado información al correo electrónico registrado con las instrucciones para reiniciar tu contraseña.";

    WebSysUserService webSysUserService;
    ElementValue[] elements = null;

    private void Awake()
    {
        webSysUserService = GetComponent<WebSysUserService>();

        elements = new ElementValue[1];
        elements[0] = ifdEmail;
    }

    private void Start()
    {
        btnReset?.AddAction(Ask);
    }

    public void Clear()
    {
        for (int i = 0; i < elements.Length; i++)
            elements[i].Clear();
    }

    public void Ask()
    {
        if (!ElementHelper.Validate(elements))
            return;

        ChoiceDialog.Instance.Info(dialogTitle, dialogMessage, Login, (UnityAction)null);
    }

    // Reset Password
    private void Login()
    {
        ScreenDialog.Instance.Display();

        FirebaseManager.Instance.LoginStartToken(ResetPassword, null);
    }

    private void ResetPassword(String _)
    {
        webSysUserService.ResetPassword(ifdEmail.Text);
    }

    // Messages
    public void PasswordResetMessage()
    {
        FirebaseManager.Instance.AuthLogOut();
#if !UNITY_EDITOR && UNITY_ANDROID
        if (NativeAuthManager.Instance.IsRegistered(ifdEmail.Text))
            NativeAuthManager.Instance.Unregister();
#endif
        ChoiceDialog.Instance.Info(dialogTitle, resetDone);
    }

    public void InternetErrorMessage()
    {
        FirebaseManager.Instance.AuthLogOut();
        ChoiceDialog.Instance.Error(dialogTitle, noInternetError);
    }
}
