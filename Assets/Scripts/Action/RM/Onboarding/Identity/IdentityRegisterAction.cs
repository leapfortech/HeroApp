using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Data.Collections;
using Leap.Data.Web;

using Sirenix.OdinInspector;


public class IdentityRegisterAction : MonoBehaviour
{
    [Title("Data")]
    [SerializeField]
    DataMapper dtmIdentity = null;

    [Space]
    [SerializeField]
    Image imgPortrait = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;
    [SerializeField]
    Page pagBirth = null;

    [Title("Message")]
    [SerializeField]
    String identityAddedTitle = "Paso Completo";
    [SerializeField, TextArea(2, 4)]
    String identityAddedMessage = "La información fue guardada exitosamente, puedes continuar con el siguiente paso.";

    [SerializeField]
    String dateErrorTitle = "Error de fecha";
    [SerializeField, TextArea(2, 4)]
    String birthDateErrorMessage = "La fecha de nacimiento es incorrecta. Revisa e intenta de nuevo.";

    IdentityService identityService = null;

    private void Awake()
    {
        identityService = GetComponent<IdentityService>();
    }

    public bool Register()
    {
        ScreenDialog.Instance.Display();

        Identity identity = dtmIdentity.BuildClass<Identity>();

        if (identity.BirthDate == new DateTime(0001, 1, 1))
        {
            ChoiceDialog.Instance.Error(dateErrorTitle, birthDateErrorMessage, () => PageManager.Instance.ChangePage(pagBirth));
            return false;
        }

        if (CalculateAge(identity.BirthDate) < 18)
        {
            ChoiceDialog.Instance.Error(dateErrorTitle, birthDateErrorMessage, () => PageManager.Instance.ChangePage(pagBirth));
            return false;
        }

        identity.OriginStateId = identity.OriginStateId == 0 ? -1 : identity.OriginStateId;

        identity.PhoneCountryId = WebManager.Instance.WebSysUser.PhoneCountryId;
        identity.Phone = WebManager.Instance.WebSysUser.Phone;
        identity.Email = WebManager.Instance.WebSysUser.Email;

        String portrait = imgPortrait.Sprite != null ? imgPortrait.Sprite.ToStrBase64(ImageType.JPG) : "";

        identityService.Register(new IdentityRegister(identity, portrait));
        return false;
    }

    public void ApplyIdentity()
    {
        StateManager.Instance.AppUser.AppUserStatusId = 2;

        ChoiceDialog.Instance.Info(identityAddedTitle, identityAddedMessage, () => PageManager.Instance.ChangePage(pagNext));
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