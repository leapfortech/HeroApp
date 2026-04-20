using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;

public class ServiceWishRegisterAction : MonoBehaviour
{
    [Title("Data")]
    [SerializeField]
    InputField ifdComment = null;
    [SerializeField]
    int serviceTypeId = -1;

    [Title("Action")]
    [SerializeField]
    Button btnRegister = null;
    [SerializeField]
    Button btnExit = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    [Title("Messages")]
    [SerializeField, TextArea(2, 4)]
    String exitMessage = "Si sales ahora y no habías enviado otra solicitud no te enterarás cuando la sección este disponible.";
    [SerializeField, TextArea(2, 4)]
    String doneMessage = "En cuánto la sección este lista o existan noticias acerca de ella te las haremos saber.";

    ServiceWishService serviceWishService = null;


    private void Awake()
    {
        serviceWishService = GetComponent<ServiceWishService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(Register);
        btnExit?.AddAction(Exit);
    }

    public void Clear()
    {
        ifdComment.Clear();
    }

    public void Exit()
    {
        ChoiceDialog.Instance.Warning("¿Seguro deseas salir?", exitMessage, () => ChangeNextPage(), null, "Si, deseo salir", "Continuar con solicitud");
    }

    private void Register()
    {
        if (!ElementHelper.Validate(ifdComment))
            return;

        ScreenDialog.Instance.Display();

        serviceWishService.Register(new ServiceWish(-1, StateManager.Instance.AppUser.Id, serviceTypeId, ifdComment.Text, -1));
    }

    public void ApplyServiceWish(long serviceWishId)
    {
        Clear();
        ChoiceDialog.Instance.Info("Solicitud completada", doneMessage, () => ChangeNextPage(), null);
    }

    public void ChangeNextPage()
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
