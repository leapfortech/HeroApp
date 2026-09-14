using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;

public class NotificationsUpdateAction : MonoBehaviour
{
    [Title("Fields")]
    [SerializeField]
    Toggle tglNotifications = null;

    AppUserService appUserService;


    private void Awake()
    {
        appUserService = GetComponent<AppUserService>();
    }

    public void AsKQuestion()
    {
        ChoiceDialog.Instance.Info("Notificaciones" , "¿Estás seguro que deseas actualizar tus notificaciones?", DoUpdate, CancelUpdate, "Sí", "No");
    }

    private void CancelUpdate()
    {
        if (StateManager.Instance.GetOption(1) == 0)
            tglNotifications.Uncheck();
        else
            tglNotifications.Check();
    }

    private void DoUpdate()
    {
        ScreenDialog.Instance.Display();

        appUserService.UpdateOption(StateManager.Instance.AppUser.Id, 1, tglNotifications.Checked ? 1 : 0);
    }

    public void ApplyUpdateOption(long options)
    {
        StateManager.Instance.AppUser.Options = options;

        ScreenDialog.Instance.Hide();
    }
}
