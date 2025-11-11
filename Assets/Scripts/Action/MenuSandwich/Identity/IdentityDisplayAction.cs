using System;
using UnityEngine;

using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Dialog;
using Leap.Data.Collections;

using Sirenix.OdinInspector;
using Leap.UI.Page;

public class IdentityDisplayAction : MonoBehaviour
{
    [Title("Title")]
    [SerializeField]
    Text txtStatusValid = null;
    [SerializeField]
    Text txtStatusExpired = null;

    [Title("Dpi")]
    [SerializeField]
    Image imgDpiFront = null;
    [SerializeField]
    Image imgDpiBack = null;
    [SerializeField]
    ToggleGroup tggDpiPhoto = null;
    [SerializeField]
    FieldValue fldNumber = null;
    [SerializeField]
    FieldValue fldIssueDate = null;
    [SerializeField]
    FieldValue fldDueDate = null;

    [Title("Info")]
    [SerializeField]
    FieldValue fldFirstName1 = null;
    [SerializeField]
    FieldValue fldFirstName2 = null;
    [SerializeField]
    FieldValue fldFirstName3 = null;
    [SerializeField]
    FieldValue fldLastName1 = null;
    [SerializeField]
    FieldValue fldLastName2 = null;
    [SerializeField]
    FieldValue fldMarriedLastName = null;
    [SerializeField]
    FieldValue fldGender = null;
    [SerializeField]
    FieldValue fldBirthDate = null;
    [SerializeField]
    FieldValue fldBirthCountry = null;
    [SerializeField]
    FieldValue fldBirthState = null;
    [SerializeField]
    FieldValue fldBirthCity = null;
    [SerializeField]
    FieldValue fldMaritalStatus = null;
    [SerializeField]
    ListScroller lstNationality = null;

    [Title("Data")]
    [SerializeField]
    ValueList vllGender = null;
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    [SerializeField]
    ValueList vllCity = null;
    [SerializeField]
    ValueList vllMaritalStatus = null;
    [SerializeField]
    ValueList vllFlags = null;

    [Title("Page")]
    [SerializeField]
    Page identityPage = null;

    IdentityService identityService = null;

    private void Awake()
    {
        identityService = GetComponent<IdentityService>();
    }

    public void Clear()
    {
        tggDpiPhoto.Value = "1";
    }

    //public void GetDpiPhoto()
    //{
    //    if (StateManager.Instance.DpiFront != null)
    //    {
    //        PageManager.Instance.ChangePage(identityPage);
    //        SandwichMenu.Instance.Close();
    //        return;
    //    }

    //    ScreenDialog.Instance.Display();
    //    identityService.GetDpiPhotoByAppUser(StateManager.Instance.AppUser.Id);
    //}

    public void ApplyDpiPhoto(DpiPhoto dpiPhoto)
    {
        //StateManager.Instance.DpiFront = dpiPhoto.DpiFront != null ? dpiPhoto.DpiFront.CreateSprite("DpiFrontDisplay") : null;
        //StateManager.Instance.DpiBack = dpiPhoto.DpiBack != null ? dpiPhoto.DpiBack.CreateSprite("DpiBackDisplay") : null;

        PageManager.Instance.ChangePage(identityPage);
        SandwichMenu.Instance.Close();
    }

    private void Display()
    {
        Clear();
        Identity identity = StateManager.Instance.Identity;

        //if (StateManager.Instance.DpiFront != null)
        //    imgDpiFront.Sprite = StateManager.Instance.DpiFront;

        //if (StateManager.Instance.DpiBack != null)
        //    imgDpiBack.Sprite = StateManager.Instance.DpiBack;

        DateTime date = identity.DpiDueDate;
        bool valid = DateTime.Today <= date;
        txtStatusValid.gameObject.SetActive(valid);
        txtStatusExpired.gameObject.SetActive(!valid);

        fldNumber.TextValue = identity.DpiCui;
        fldIssueDate.TextValue = identity.DpiIssueDate.Day + " de " + 
                                 StateManager.Instance.MonthNames[identity.DpiIssueDate.Month - 1] + ", " +
                                 identity.DpiIssueDate.Year;
        fldDueDate.TextValue = identity.DpiDueDate.Day + " de " +
                               StateManager.Instance.MonthNames[identity.DpiDueDate.Month - 1] + ", " +
                               identity.DpiDueDate.Year;

        fldFirstName1.TextValue = identity.FirstName1;
        fldFirstName2.TextValue = identity.FirstName2 == null ? "-" : identity.FirstName2;
        fldFirstName3.TextValue = identity.FirstName3 == null ? "-" : identity.FirstName3;
        fldLastName1.TextValue = identity.LastName1;
        fldLastName2.TextValue = identity.LastName2 == null ? "-" : identity.LastName2;
        fldMarriedLastName.TextValue = identity.LastNameMarried == null ? "-" : identity.LastNameMarried;
        fldGender.TextValue = vllGender.FindRecordCellString(identity.GenderId, "Name");
        fldBirthDate.TextValue = identity.BirthDate.Day + " de " +
                                 StateManager.Instance.MonthNames[identity.BirthDate.Month - 1] + ", " +
                                 identity.BirthDate.Year;
        fldBirthCountry.TextValue = vllCountry.FindRecordCellString(identity.BirthCountryId, "Name");
        fldBirthState.TextValue = identity.BirthStateId == -1 ? "-" : vllState.FindRecordCellString(identity.BirthStateId, "Name");
        fldBirthCity.TextValue = identity.BirthCityId == -1 ? "-" : vllCity.FindRecordCellString(identity.BirthCityId, "Name");
        fldMaritalStatus.TextValue = vllMaritalStatus.FindRecordCellString(identity.MaritalStatusId, "Name");

        lstNationality.Clear();
        String[] nationalities = identity.NationalityIds.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < nationalities.Length; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(2, false);
            scrollerValue.SetText(0, vllCountry.FindRecordCellString(Convert.ToInt32(nationalities[i]), "Name"));
            scrollerValue.SetSprite(1, vllFlags.FindRecordCellSprite("Code", vllCountry.FindRecordCellString(Convert.ToInt32(nationalities[i]), "Code"), "Flag"));
            lstNationality.AddValue(scrollerValue);
        }
        lstNationality.ApplyValues();
    }

    public void ChangeDpiPhoto()
    {
        bool isFront = tggDpiPhoto.Value[0] == '1';
        imgDpiFront.gameObject.SetActive(isFront);
        imgDpiBack.gameObject.SetActive(!isFront);
    }
}