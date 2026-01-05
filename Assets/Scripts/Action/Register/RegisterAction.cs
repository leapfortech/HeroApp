using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.UI.Extensions;
using Leap.Data.Web;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;
using Leap.Graphics.Tools;


public class RegisterAction : MonoBehaviour
{
    [Header("Fields")]
    [SerializeField]
    InputField ifdAlias = null;

    [SerializeField]
    InputField ifdEmail = null;

    [SerializeField]
    InputField ifdPassword = null;

    [SerializeField]
    ComboAdapter cmbPhonePrefix = null;

    [SerializeField]
    InputField ifdPhone = null;

    [SerializeField]
    InputField ifdConfirm = null;

    [SerializeField]
    InputField ifdReferredCode = null;

    [SerializeField]
    Toggle chkTerms = null;

    [SerializeField]
    Image imgPortrait = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmIdentity = null;

    [SerializeField]
    DataMapper dtmAddress = null;

    [Header("Action")]
    [SerializeField]
    Button btnRegister = null;

    [SerializeField]
    Button btnResendLink = null;

    [Header("Result")]
    [SerializeField]
    Text txtEmail = null;
    [SerializeField]
    Page pagMailLink = null;
    [SerializeField]
    Page pagDone = null;

    [Space]
    [SerializeField, TextArea(2, 5)]
    String aliasInvalidMessage = "Alias inválido.";
    [SerializeField, TextArea(2, 5)]
    String aliasAlreadyExistsMessage = "El Alias ya existe.";
    [SerializeField, TextArea(2, 5)]
    String verifyError = "Unable to send the activation email. Please try again.";
    [SerializeField, TextArea(2, 5)]
    String passwordError = "The password fields do not match. Please enter them again.";
    [SerializeField, TextArea(2, 4)]
    String birthDateError = "La fecha de nacimiento es incorrecta. Revisa e intenta de nuevo.";
    [SerializeField, TextArea(2, 4)]
    String minorError = "No se permite el registro de menores de edad.";

    AppUserService appUserService;
    AccessService accessService;
    WebSysUserService webSysUserService;
    ElementValue[] elementValues = null;

    bool isAliasAvailable = false;

    private void Awake()
    {
        appUserService = GetComponent<AppUserService>();
        accessService = GetComponent<AccessService>();
        webSysUserService = GetComponent<WebSysUserService>();
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (elementValues != null)
            return;

        elementValues = new ElementValue[5];
        elementValues[0] = ifdAlias;
        elementValues[1] = ifdEmail;
        elementValues[2] = ifdPassword;
        elementValues[3] = ifdConfirm;
        elementValues[4] = chkTerms;

        btnRegister?.AddAction(Register);
        btnResendLink?.AddAction(ResendMailLink);
    }

    public void Init()
    {       
        Clear();
        if (cmbPhonePrefix.Combo.IsEmpty())
            cmbPhonePrefix.Select(2);
    }

    public void Clear()
    {
        Initialize();
        for (int i = 0; i < elementValues.Length; i++)
            elementValues[i].Clear();
    }

    public void ClearAll()
    {
       for (int i = 0; i < elementValues.Length; i++)
            elementValues[i].Clear();

        dtmIdentity.ClearElements();
        dtmAddress.ClearElements();

        isAliasAvailable = false;
        RefreshRegisterButton();
    }

    // Alias

    public void ValidateAlias()
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
        isAliasAvailable = aliasResponse == null;

        if (!isActiveAndEnabled)
            ChoiceDialog.Instance.Error("Alias", aliasAlreadyExistsMessage);
        else
            RefreshRegisterButton();
   
        isAliasAvailable = false;
        ScreenDialog.Instance.Hide();
    }

    public void RefreshRegisterButton()
    {
        btnRegister.Interactable = isAliasAvailable;
    }

    // Register

    public void Register()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        if (ifdPassword.Text != ifdConfirm.Text)
        {
            ifdPassword.DisplayValidity(false);
            ifdConfirm.DisplayValidity(false);
            ChoiceDialog.Instance.Error(PageManager.Instance.CurrentPage.HeaderTitle, passwordError);
            return;
        }

        ScreenDialog.Instance.Display();

        FirebaseManager.Instance.LoginStartToken(DoRegister, null);
    }

    private void DoRegister(String _)
    {
        Identity identity = dtmIdentity.BuildClass<Identity>();

        if (identity.BirthDate == new DateTime(0001, 1, 1))
        {
            ChoiceDialog.Instance.Error("Error de fecha", birthDateError, null);
            return;
        }

        if (CalculateAge(identity.BirthDate) < 18)
        {
            ChoiceDialog.Instance.Error("Error de fecha", minorError, null);
            return;
        }

        Address address = dtmAddress.BuildClass<Address>();

        String portrait = null;
       
        if (imgPortrait != null) 
            portrait = imgPortrait.Sprite != null ? imgPortrait.Sprite.ToStrBase64(ImageType.JPG) : "";

        accessService.RegisterApp(new RegisterAppRequest(ifdAlias.Text, ifdEmail.Text, ifdPassword.Text, 
                                                         cmbPhonePrefix.GetSelectedRecord().Id, ifdPhone.Text,
                                                         ifdReferredCode.Text,
                                                         new IdentityRegister(identity, portrait),
                                                         address));
    }

    // Send Mail Link
    public void ResendMailLink()
    {
        ScreenDialog.Instance.Display();

        FirebaseManager.Instance.LoginStartToken(OnResendMailLink, null);
    }

    private void OnResendMailLink(String _)
    {
        SendMailLink();
    }

    public void ApplyRegistered(String registerResponse)  // registerResponse : $"{appUserId}|{isMailVerified}"
    {
        ClearAll();

        if (registerResponse[^1] == '0')
            SendMailLink();  // First time
        else
            PageManager.Instance.ChangePage(pagDone);
    }

    private void SendMailLink()
    {
        FirebaseManager.Instance.Login(ifdEmail.Text, ifdPassword.Text, OnLoginMailLinkDone, null, false);
    }

    private void OnLoginMailLinkDone(String eMail)
    {
        webSysUserService.SendMailLink(eMail);
    }

    public void ChangePageMailLink()
    {
        txtEmail.TextValue = ifdEmail.Text;

        PageManager.Instance.ChangePage(pagMailLink);
    }

    // Messages

    public void VerifyErrorMessage()
    {
        ChoiceDialog.Instance.Info("Registro", verifyError);
    }

    public int CalculateAge(DateTime birthDate)
    {
        int age = DateTime.Today.Year - birthDate.Year;

        if (DateTime.Today.Month < birthDate.Month)
            --age;
        else if (DateTime.Today.Month == birthDate.Month && DateTime.Today.Day < birthDate.Day)
            --age;
        return age;
    }
}
