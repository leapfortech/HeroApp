using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.Data.Collections;

using Sirenix.OdinInspector;

public class IdentityDisplayAction : MonoBehaviour
{
    [Title("Info")]
    [SerializeField]
    FieldValue fldFirstNames = null;
    [SerializeField]
    FieldValue fldLastNames = null;
    [SerializeField]
    FieldValue fldGender = null;
    [SerializeField]
    FieldValue fldBirthDate = null;
    [SerializeField]
    FieldValue fldOriginCountry = null;
    [SerializeField]
    FieldValue fldOriginState = null;
    //[SerializeField]
    //FieldValue fldOriginCity = null;

    [Title("Data")]
    [SerializeField]
    ValueList vllGender = null;
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    //[SerializeField]
    //ValueList vllCity = null;

    public void Display()
    {
        Identity identity = StateManager.Instance.Identity;

        String firstNames = $"{identity.FirstName1} {identity.FirstName2}".Trim();
        fldFirstNames.TextValue = string.IsNullOrWhiteSpace(firstNames) ? "-" : firstNames;

        String lastNames = $"{identity.LastName1} {identity.LastName2}".Trim();
        fldLastNames.TextValue = string.IsNullOrWhiteSpace(lastNames) ? "-" : lastNames;

        fldGender.TextValue = vllGender.FindRecordCellString(identity.GenderId, "Name");
        fldBirthDate.TextValue = identity.BirthDate.Day + " de " +
                                 StateManager.Instance.MonthNames[identity.BirthDate.Month - 1] + ", " +
                                 identity.BirthDate.Year;
        fldOriginCountry.TextValue = vllCountry.FindRecordCellString(identity.OriginCountryId, "Name");
        fldOriginState.TextValue = identity.OriginStateId == -1 ? "-" : vllState.FindRecordCellString(identity.OriginStateId, "Name");
        //fldOriginCity.TextValue = identity.OriginCityId == -1 ? "-" : vllCity.FindRecordCellString(identity.OriginCityId, "Name");
    }
}