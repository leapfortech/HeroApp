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
    [Title("Elements")]
    [SerializeField]
    ToggleGroup tggPep = null;
    [SerializeField]
    ToggleGroup tggPepIdentity = null;
    [SerializeField]
    ToggleGroup tggCpe = null;

    [Title("Data")]
    [SerializeField]
    ValueList vllEnrollNationality = null;
    [SerializeField]
    DataMapper dtmIdentity = null;
    [SerializeField]
    DataMapper dtmPep = null;
    [SerializeField]
    DataMapper dtmCpe = null;

    [Title("Dpi")]
    [SerializeField]
    Image imgDpiFront = null;

    [SerializeField]
    Image imgDpiBack = null;

    [SerializeField]
    Image imgDpiPortrait = null;

    [Space]
    [SerializeField]
    Image imgPortrait = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;
    [SerializeField]
    Page pagBirth = null;
    [SerializeField]
    Page pagDpi = null;

    [Title("Message")]
    [SerializeField]
    String identityAddedTitle = "Paso Completo";
    [SerializeField, TextArea(2, 4)]
    String identityAddedMessage = "La información fue guardada exitosamente, puedes continuar con el siguiente paso.";

    [SerializeField]
    String dateErrorTitle = "Error de fecha";
    [SerializeField, TextArea(2, 4)]
    String birthDateErrorMessage = "La fecha de nacimiento es incorrecta. Revisa e intenta de nuevo.";
    [SerializeField, TextArea(2, 4)]
    String issueDateErrorMessage = "La fecha de emisión es incorrecta. Revisa e intenta de nuevo.";
    [SerializeField, TextArea(2, 4)]
    String dueDateErrorMessage = "La fecha de vencimiento es incorrecta. Revisa e intenta de nuevo.";
    [SerializeField, TextArea(2, 4)]
    String dpiDatesErrorMessage = "La fecha de emisión o vencimiento son incorrectas. Revisa e intenta de nuevo.";

    IdentityService identityService = null;
    List<PepIdentityRequest> pepIdentityRequests = new List<PepIdentityRequest>();

    private void Awake()
    {
        identityService = GetComponent<IdentityService>();
    }

    public void SetPepIdentityRequest(List<PepIdentityRequest> pepIdentityRequests)
    {
        this.pepIdentityRequests.Clear();

        this.pepIdentityRequests.AddRange(pepIdentityRequests);
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

        if (identity.DpiIssueDate == new DateTime(0001,1,1))
        {
            ChoiceDialog.Instance.Error(dateErrorTitle, issueDateErrorMessage, () => PageManager.Instance.ChangePage(pagDpi));
            return false;
        }

        if (identity.DpiDueDate == new DateTime(0001, 1, 1))
        {
            ChoiceDialog.Instance.Error(dateErrorTitle, dueDateErrorMessage, () => PageManager.Instance.ChangePage(pagDpi));
            return false;
        }

        int years = identity.DpiDueDate.Year - identity.DpiIssueDate.Year;
        double days = (identity.DpiIssueDate - identity.DpiDueDate.AddYears(-10)).TotalDays;

        if (years != 10 || days != 1)
        {
            ChoiceDialog.Instance.Error(dateErrorTitle, dpiDatesErrorMessage, () => PageManager.Instance.ChangePage(pagDpi));
            return false;
        }

        if (identity.DpiDueDate.Date < DateTime.Now.Date)
        {
            ChoiceDialog.Instance.Error(dateErrorTitle, dueDateErrorMessage, () => PageManager.Instance.ChangePage(pagDpi));
            return false;
        }

        String nationalityIds = vllEnrollNationality.RecordCount > 0 ? vllEnrollNationality[0].Id.ToString() : "";
        for (int i = 1; i < vllEnrollNationality.RecordCount; i++)
            nationalityIds += "|" + vllEnrollNationality[i].Id;

        identity.NationalityIds = nationalityIds;
        identity.AppUserId = StateManager.Instance.AppUser.Id;
        identity.BirthStateId = identity.BirthStateId == 0 ? -1 : identity.BirthStateId;
        identity.BirthCityId = identity.BirthCityId == 0 ? -1 : identity.BirthCityId;

        identity.PhoneCountryId = WebManager.Instance.WebSysUser.PhoneCountryId;
        identity.Phone = WebManager.Instance.WebSysUser.Phone;
        identity.Email = WebManager.Instance.WebSysUser.Email;

        String dpiFront = imgDpiFront.Sprite != null ? imgDpiFront.Sprite.ToStrBase64(ImageType.JPG) : "";
        String dpiBack = imgDpiBack.Sprite != null ? imgDpiBack.Sprite.ToStrBase64(ImageType.JPG) : "";
        String dpiPortrait = imgDpiPortrait.Sprite != null ? imgDpiPortrait.Sprite.ToStrBase64(ImageType.JPG) : "";
        String portrait = imgPortrait.Sprite != null ? imgPortrait.Sprite.ToStrBase64(ImageType.JPG) : "";

        identityService.Register(new IdentityRegister(new IdentityInfo(identity, new DpiPhoto(dpiFront, dpiBack, dpiPortrait)),
                                                      tggPep.Value == "1" ? dtmPep.BuildClass<Pep>() : null,
                                                      tggPepIdentity.Value == "1" ? pepIdentityRequests.ToArray() : null,
                                                      tggCpe.Value == "1" ? dtmCpe.BuildClass<Cpe>() : null,
                                                      portrait));
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