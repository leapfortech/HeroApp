using UnityEngine;
using System;

using Leap.UI.Elements;
using Leap.UI.Dialog;
using Leap.UI.Page;

using Sirenix.OdinInspector;

public class ReferredUpdateAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    InputField ifdReferredCode = null;
    [SerializeField]
    Text txtInstructions = null;

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page nextPage = null;

    [Title("Messages")]
    [SerializeField, TextArea(2, 4)]
    String updMessage = null;
    [SerializeField, TextArea(2, 4)]
    String updDone = null;
    [SerializeField]
    String referredUpdateTitle = "Referencias";
    [SerializeField, TextArea(2, 4)]
    String referredUpdateMessage = "La información fue guardada exitosamente.";

    AppUserService appUserService = null;


    private void Awake()
    {
        appUserService = GetComponent<AppUserService>();
    }

    private void Start()
    {
        btnUpdate?.AddAction(Register);
    }

    public void Clear()
    {
        ifdReferredCode.Clear();
    }

    public void Display()
    {
        Clear();

        ifdReferredCode.gameObject.SetActive(StateManager.Instance.AppUser.ReferrerAppUserId == -1);
        btnUpdate.gameObject.SetActive(StateManager.Instance.AppUser.ReferrerAppUserId == -1);
        txtInstructions.TextValue = StateManager.Instance.AppUser.ReferrerAppUserId == -1 ? updMessage : updDone;
    }

    private void Register()
    {
        if (!ElementHelper.Validate(ifdReferredCode))
            return;

        ScreenDialog.Instance.Display();

        appUserService.UpdateReferred(Convert.ToInt64(ifdReferredCode.Text));
    }

    public void ApplyReferred(int referredAppUserId)
    {
        if (referredAppUserId == -1)
        {
            ChoiceDialog.Instance.Error("Código de referido", "El código de referido es incorrecto.");
            return;
        }

        StateManager.Instance.AppUser.ReferrerAppUserId = referredAppUserId;

        ScreenDialog.Instance.Hide();

        ChoiceDialog.Instance.Info(referredUpdateTitle, referredUpdateMessage, () => PageManager.Instance.ChangePage(nextPage));
    }
}
