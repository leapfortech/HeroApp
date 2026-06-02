using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Dialog;
using Leap.UI.Page;

using Sirenix.OdinInspector;

public class AliasUpdateAction : MonoBehaviour
{
    [Title("Fields")]
    [SerializeField]
    InputField ifdAlias = null;

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    [Space]
    [SerializeField, TextArea(2, 5)]
    String aliasInvalidMessage = "Alias inválido.";
    [SerializeField, TextArea(2, 5)]
    String aliasAlreadyExistsMessage = "El Alias ya existe.";

    [Space]
    [SerializeField, TextArea(2, 4)]
    String updatedMessage = "La información fue guardada exitosamente.";

    AppUserService appUserService;

    bool isAliasAvailable = false;

    private void Awake()
    {
        appUserService = GetComponent<AppUserService>();
    }

    private void Start()
    {
        Initialize();
        btnUpdate?.AddAction(DoUpdate);
    }

    public void Initialize()
    {
        Clear();
        isAliasAvailable = false;
        RefreshUpdateButton();
    }

    public void Clear()
    {
        ifdAlias.Clear();
    }

    // Alias

    public void Validate()
    {
        if (ifdAlias.Text.Length == 0)
        {
            ChoiceDialog.Instance.Error("Alias", aliasInvalidMessage);
            return;
        }
        
        ScreenDialog.Instance.Display();
        appUserService.ValidateAlias(new AliasRequest(ifdAlias.Text));
    }


    public void ApplyAliasValidation(AliasResponse aliasResponse)
    {
        isAliasAvailable = (aliasResponse == null || aliasResponse.Email == null);

        if (!isAliasAvailable)
            ChoiceDialog.Instance.Error("Alias", aliasAlreadyExistsMessage);
        else
            RefreshUpdateButton();
   
        ScreenDialog.Instance.Hide();
    }

    public void DoUpdate()
    {
        ScreenDialog.Instance.Display();

        appUserService.UpdateAlias(new AliasRequest(StateManager.Instance.AppUser.Id, ifdAlias.Text));
    }

    public void ApplyAlias()
    {
        StateManager.Instance.AppUser.Alias = ifdAlias.Text;

        ChoiceDialog.Instance.Info("Información actualizada", updatedMessage, () => PageManager.Instance.ChangePage(pagNext));
    }

    public void RefreshUpdateButton()
    {
        btnUpdate.Interactable = isAliasAvailable;
    }
}
