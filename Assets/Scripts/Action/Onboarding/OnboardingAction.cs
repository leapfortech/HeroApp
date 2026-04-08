using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;

using Sirenix.OdinInspector;

public class OnboardingAction : MonoBehaviour
{
    [SerializeField]
    Text[] txtAliass = null;

    [Space, SerializeField]
    Image imgPortrait = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmIdentity = null;

    [SerializeField]
    DataMapper dtmAddress = null;

    [SerializeField, TextArea(2, 4)]
    String exitMessage = "Si sales ahora no se guardara tu perfil, recuerda que puedes hacerlo luego si lo deseas desde el menú.";

    [SerializeField, TextArea(2, 4)]
    String emtpyMessage = "No has ingresado ninguna información.";

    [SerializeField, TextArea(2, 4)]
    String minorError = "No se permite el registro de menores de edad.";

    [Title("Action")]
    [SerializeField]
    Button btnRegister = null;
    [SerializeField]
    Button btnExit = null;

    [Title("Page")]
    [SerializeField]
    Page pagStart = null;

    [SerializeField]
    Page pagNext = null;

    AccessService accessService = null;
    
    Identity identity = null;
    Address address = null;
    String portrait = null;

    private void Awake()
    {
        accessService = GetComponent<AccessService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(Register);
        btnExit?.AddAction(Exit);
    }

    public void Clear()
    {
        imgPortrait.ClearValue();
        dtmIdentity.ClearElements();
        dtmAddress.ClearElements();
    }

    public void Exit()
    {
        ChoiceDialog.Instance.Warning("¿Lo quieres hacer luego?", exitMessage, () => ChangeNextPage(), null, "Si, deseo salir", "Continuar con perfil");
    }

    public void DisplayAlias()
    {
        for (int i = 0; i < txtAliass.Length; i++)
            txtAliass[i].TextValue = StateManager.Instance.AppUser.Alias;
    }

    private void Register()
    {
        DateTime sqlMinDate = new DateTime(1753, 1, 1);

        identity = dtmIdentity.BuildClass<Identity>();

        if (identity.BirthDate == new DateTime(0001, 1, 1))
        {
            identity.BirthDate = sqlMinDate;
        }
        else
        {
            if (CalculateAge(identity.BirthDate) < 18)
            {
                ChoiceDialog.Instance.Error("Error de fecha", minorError);
                return;
            }
        }

        address = dtmAddress.BuildClass<Address>();

        portrait = imgPortrait.Sprite != null ? imgPortrait.Sprite.ToStrBase64(ImageType.JPG) : "";

        if (portrait == "")
            portrait = null;

        bool isPersonalEmpty = String.IsNullOrEmpty(identity.FirstName1) &&
                               String.IsNullOrEmpty(identity.FirstName2) &&
                               String.IsNullOrEmpty(identity.LastName1) &&
                               String.IsNullOrEmpty(identity.LastName2) &&
                               identity.GenderId == -1 &&
                               identity.BirthDate == sqlMinDate;

        bool isAddressEmpty = address.CountryId == -1 && address.StateId == -1 && address.CityId == -1;

        if (isPersonalEmpty && isAddressEmpty)
        {
            ChoiceDialog.Instance.Error("¿Lo quieres hacer luego?", emtpyMessage, () => ChangeNextPage(), () => PageManager.Instance.ChangePage(pagStart), "Sí, deseo salir", "Continuar con perfil");
            return;
        }

        long appUserId = StateManager.Instance.AppUser.Id;

        //accessService.Onboarding(new OnboardingRequest(appUserId, identity, address, portrait));
    }

    public void ApplyOnboarding(OnboardingResponse obdResponse)
    {       
       
        identity.Id = obdResponse.IdentityId;
        address.Id = obdResponse.AddressId;

        StateManager.Instance.Identity = identity;
        StateManager.Instance.Address = address;
        StateManager.Instance.Portrait = portrait.CreateSprite("Portrait");

        ChangeNextPage();
    }

    public void ChangeNextPage()
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
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
