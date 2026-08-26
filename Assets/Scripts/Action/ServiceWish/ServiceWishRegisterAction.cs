using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Collections;

using Sirenix.OdinInspector;

public class ServiceWishRegisterAction : MonoBehaviour
{
    [Title("Scroll")]
    [SerializeField]
    UnityEngine.UI.ScrollRect scrollRect = null;

    [Title("Data")]
    [SerializeField]
    int serviceWishTypeId = -1;
    [SerializeField]
    ToggleGroup tggServiceWishOption = null;
    [SerializeField]
    InputField ifdServiceWish = null;
    [SerializeField]
    Text txtDone = null;

    [Title("Value")]
    [SerializeField]
    ValueList vllServiceWishType = null;

    [Title("Action")]
    [SerializeField]
    Button btnRegister = null;
    [SerializeField]
    Button btnExit = null;

    [Title("Page")]
    [SerializeField]
    Page pagMenu = null;
    [SerializeField]
    Page pagDone = null;

    [Title("Messages")]
    [SerializeField, TextArea(2, 4)]
    String exitMessage = "¿Estás seguro de que deseas salir?";

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
        tggServiceWishOption.Clear();
        ifdServiceWish.Clear();
        scrollRect.verticalNormalizedPosition = 1f;
    }

    public void Exit()
    {
        ChoiceDialog.Instance.Warning("¿Seguro deseas salir?", exitMessage, ChangePageMenu, null, "Si, deseo salir", "Continuar con solicitud");
    }

    private void Register()
    {
        if (!ElementHelper.Validate(ifdServiceWish))
            return;

        ScreenDialog.Instance.Display();

        serviceWishService.Register(new ServiceWish(-1,
                                                    StateManager.Instance.AppUser.Id,
                                                    serviceWishTypeId,
                                                    Convert.ToInt64(tggServiceWishOption.Value),
                                                    ifdServiceWish.Text, -1));
    }

    public void ApplyServiceWish(long serviceWishId)
    {
        Clear();
        txtDone.TextValue = $"Gracias por tu interés en <b>{vllServiceWishType.FindRecordCellString(serviceWishTypeId, "Name")}</b>. Tu opinión nos ayuda a decidir qué construir después.";
        PageManager.Instance.ChangePage(pagDone);
    }

    public void ChangePageMenu()
    {
        Clear();
        PageManager.Instance.ChangePage(pagMenu);
    }
}
