using System;

using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.UI.Extensions;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class EmailAction : MonoBehaviour
{
    [Title("Email")]
    [SerializeField]
    InputField ifdEmail = null;

    [SerializeField]
    FieldValue txtEmail = null;

    [Title("Action")]
    [SerializeField]
    Button btnUpdateEmail = null;

    [Title("Pages")]
    [SerializeField]
    Page nextPageUpdate = null;

    [Title("Messages")]
    [SerializeField]
    String updatedTitle = "Correo electrónico";

    [SerializeField]
    String updatedMessage = "Correo electrónico actualizado exitosamente.";

    WebSysUserService webSysUserService = null;

    private void Initialize()
    {
        if (webSysUserService != null)
            return;

        webSysUserService = GetComponent<WebSysUserService>();
    }

    private void Start()
    {
        btnUpdateEmail?.AddAction(UpdatePhone);
    }

    public void Clear()
    {
        ifdEmail.Clear();
    }

    private void UpdatePhone()
    {
        Initialize();

        if (!ElementHelper.Validate(ifdEmail))
            return;

        ScreenDialog.Instance.Display();
    }

    public void ApplyUpdatePhone(String result)
    {
        txtEmail.TextValue = ifdEmail.Text;
        ChoiceDialog.Instance.Info(updatedTitle, updatedMessage, () => PageManager.Instance.ChangePage(nextPageUpdate));
    }
}