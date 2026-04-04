using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Web;


public class ValidateAlias : MonoBehaviour
{
    [Header("Fields")]
    [SerializeField]
    InputField ifdAlias = null;

    [Header("Action")]
    [SerializeField]
    Button btnNext = null;

    [Space]
    [SerializeField, TextArea(2, 5)]
    String aliasInvalidMessage = "Alias inválido.";
    [SerializeField, TextArea(2, 5)]
    String aliasAlreadyExistsMessage = "El Alias ya existe.";

    AppUserService appUserService;

    bool isAliasAvailable = false;

    private void Awake()
    {
        appUserService = GetComponent<AppUserService>();
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        Clear();
        isAliasAvailable = false;
        RefreshRegisterButton();
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
        FirebaseManager.Instance.LoginStartToken(DoValidateAlias, null);
    }

    private void DoValidateAlias(String _)
    {
        appUserService.ValidateAlias(new AliasRequest(ifdAlias.Text));
    }

    public void ApplyAliasValidation(AliasResponse aliasResponse)
    {
        isAliasAvailable = (aliasResponse == null || aliasResponse.Email == null);

        if (!isAliasAvailable)
            ChoiceDialog.Instance.Error("Alias", aliasAlreadyExistsMessage);
        else
            RefreshRegisterButton();
   
        ScreenDialog.Instance.Hide();
    }

    public void RefreshRegisterButton()
    {
        btnNext.Interactable = isAliasAvailable;
    }
}
