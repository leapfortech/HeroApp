using System;
using UnityEngine;

using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Data.Web;

using Sirenix.OdinInspector;
using Leap.Data.Collections;
using System.Collections.Generic;

public class IdentityUpdateAction : MonoBehaviour
{
    [Title("Elements")]
#pragma warning disable 0414
    [SerializeField]
    ToggleGroup tggPep = null;
    [SerializeField]
    ToggleGroup tggPepIdentity = null;
    [SerializeField]
    ToggleGroup tggCpe = null;
#pragma warning restore 0414
    [SerializeField]
    ListScroller lstNationality = null;

    [Title("Input")]
    [SerializeField]
    ValueList vllEnrollNationality = null;

    [Title("Dpi")]
    [SerializeField]
    Image imgDpiFront = null;

    [SerializeField]
    Image imgDpiBack = null;

    [SerializeField]
    Image imgDpiPortrait = null;

    [Title("Data")]
    [SerializeField]
    ValueList vllCountry = null;

    [SerializeField]
    ValueList vllFlags = null;

    [SerializeField]
    DataMapper dtmEnrollNationalityValueList = null;

    [SerializeField]
    DataMapper dtmEnrollNationalityListScroller = null;
#pragma warning disable 0414
    [SerializeField]
    DataMapper dtmPep = null;
    [SerializeField]
    DataMapper dtmCpe = null;
#pragma warning restore 0414

    [SerializeField]
    DataMapper dtmUpdateIdentity = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;
    [SerializeField]
    Page pagBirth = null;
    [SerializeField]
    Page pagDpi = null;

    [Title("Message")]
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

    [Space]
    [SerializeField]
    String identityUpdatedTitle = "Información actualizada";
    [SerializeField, TextArea(2, 4)]
    String identityUpdatedMessage = "La información fue guardada exitosamente.";

    IdentityService identityService = null;
    
    Identity identity = null;

    private void Awake()
    {
        identityService = GetComponent<IdentityService>();
    }

    public void PopulateIdentity()
    {
        dtmUpdateIdentity.PopulateClass<Identity>(StateManager.Instance.Identity);

        lstNationality.Clear();
        String[] nationalities = identity.NationalityIds.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < nationalities.Length; i++)
        {
            vllEnrollNationality.AddRecord(vllCountry.FindRecordCellString(Convert.ToInt32(nationalities[i]), "Name"),
                                           vllCountry.FindRecordCellString(Convert.ToInt32(nationalities[i]), "Code"), 
                                           vllFlags.FindRecordCellSprite("Code", vllCountry.FindRecordCellString(Convert.ToInt32(nationalities[i]), "Code"), "Flag"),
                                           nationalities[i]);

            List<CountryFlag> countryFlag = dtmEnrollNationalityValueList.BuildClassList<CountryFlag>();
            dtmEnrollNationalityListScroller.PopulateClassList(countryFlag);
        }
    }

    public bool Register()
    {
        ScreenDialog.Instance.Display();

        identity = dtmUpdateIdentity.BuildClass<Identity>();

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

        if (identity.DpiIssueDate == new DateTime(0001, 1, 1))
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

        identityService?.Register(new IdentityRegister(identity, dpiPortrait));

        return false;
    }

    public void ApplyIdentityInfo(int identityId)
    {
        if (identityService != null)
        {
            identity.Id = identityId;
            StateManager.Instance.Identity = identity;
            StateManager.Instance.OnboardingStage = 0;
            //StateManager.Instance.DpiFront = imgDpiFront.Sprite;
            //StateManager.Instance.DpiBack = imgDpiBack.Sprite;

            identity = null;
            //imgDpiFront.Sprite = imgDpiBack.Sprite = imgDpiPortrait.Sprite = null;

            ChoiceDialog.Instance.Info(identityUpdatedTitle, identityUpdatedMessage, () => PageManager.Instance.ChangePage(pagNext));
        }
        else
        {
            StateManager.Instance.OnboardingStage = 0;
            identity = null;
            //imgDpiFront.Sprite = imgDpiBack.Sprite = imgDpiPortrait.Sprite = null;
            PageManager.Instance.ChangePage(pagNext);
        }
    }

    public void ErrorIdentityInfo(String message)
    {
        identity = null;
        //imgDpiFront.Sprite = imgDpiBack.Sprite = imgDpiPortrait.Sprite = null;
        ChoiceDialog.Instance.Error(message);
    }

    public int CalculateAge(DateTime birthDate)
    {
        int age = DateTime.Today.Year - birthDate.Year;

        if (DateTime.Today.Month < birthDate.Month)
            return age - 1;
        
        if (DateTime.Today.Month == birthDate.Month && DateTime.Today.Day < birthDate.Day)
            return age - 1;

        return age;
    }
}