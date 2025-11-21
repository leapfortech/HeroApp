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

    [Title("Data")]
    //[SerializeField]
    //ValueList vllCountry = null;

    //[SerializeField]
    //ValueList vllFlags = null;

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

    [Title("Message")]
    [SerializeField]
    String dateErrorTitle = "Error de fecha";
    [SerializeField, TextArea(2, 4)]
    String birthDateErrorMessage = "La fecha de nacimiento es incorrecta. Revisa e intenta de nuevo.";

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

        //lstNationality.Clear();
        //for (int i = 0; i < nationalities.Length; i++)
        //{
        //    vllEnrollNationality.AddRecord(vllCountry.FindRecordCellString(Convert.ToInt32(nationalities[i]), "Name"),
        //                                   vllCountry.FindRecordCellString(Convert.ToInt32(nationalities[i]), "Code"), 
        //                                   vllFlags.FindRecordCellSprite("Code", vllCountry.FindRecordCellString(Convert.ToInt32(nationalities[i]), "Code"), "Flag"),
        //                                   nationalities[i]);

        //    List<CountryFlag> countryFlag = dtmEnrollNationalityValueList.BuildClassList<CountryFlag>();
        //    dtmEnrollNationalityListScroller.PopulateClassList(countryFlag);
        //}
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

        identity.OriginStateId = identity.OriginStateId == 0 ? -1 : identity.OriginStateId;

        identity.PhoneCountryId = WebManager.Instance.WebSysUser.PhoneCountryId;
        identity.Phone = WebManager.Instance.WebSysUser.Phone;
        identity.Email = WebManager.Instance.WebSysUser.Email;

        identityService?.Register(new IdentityRegister(identity, null));

        return false;
    }

    public void ApplyIdentityInfo(int identityId)
    {
        if (identityService != null)
        {
            identity.Id = identityId;
            StateManager.Instance.Identity = identity;

            identity = null;

            ChoiceDialog.Instance.Info(identityUpdatedTitle, identityUpdatedMessage, () => PageManager.Instance.ChangePage(pagNext));
        }
        else
        {
            identity = null;
            PageManager.Instance.ChangePage(pagNext);
        }
    }

    public void ErrorIdentityInfo(String message)
    {
        identity = null;
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