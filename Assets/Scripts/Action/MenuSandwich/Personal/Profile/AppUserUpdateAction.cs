using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Dialog;
using Leap.UI.Page;
using Leap.Data.Collections;
using Leap.Data.Mapper;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class AppUserUpdateAction : MonoBehaviour
{
    [Title("Profile")]
    [SerializeField]
    FieldValue fldFirstName = null;
    [SerializeField]
    FieldValue fldLastName = null;
    //[SerializeField]
    //FieldValue fldBirthDate = null;
    //[SerializeField]
    //FieldValue fldSSN = null;
    [SerializeField]
    FieldValue fldPhone = null;
    [SerializeField]
    FieldValue fldEmail = null;

    //[SerializeField]
    //Toggle chkPerson = null;

    [Title("Data")]
    [SerializeField]
    ValueList vllCountry = null;

    //[SerializeField]
    //DataMapper dtmPerson = null;

    //[SerializeField]
    //MultiWheel mwlBirthDate = null;

    //[Title("Action")]
    //[SerializeField]
    //Button btnUpdate = null;

    //[Title("Pages")]
    //[SerializeField]
    //Page nextPage = null;

    //[Title("Messages")]
    //[SerializeField]
    //String minorError = "To use our services, you must be of legal age.";

    //AppUserService appUserService = null;


    //private void Awake()
    //{
    //    appUserService = GetComponent<AppUserService>();
    //}

    //private void Start()
    //{
    //    btnUpdate?.AddAction(UpdateAppUser);
    //}

    //private void UpdateAppUser()
    //{
    //    ScreenDialog.Instance.Display();
    //    Invoke(nameof(UpdatePerson), 0.2f);
    //}

    public void DisplayAppUser()
    {
        fldFirstName.TextValue = StateManager.Instance.Identity.FirstName1;
        fldLastName.TextValue = StateManager.Instance.Identity.LastName1;
        //fldBirthDate.TextValue = StateManager.Instance.AppUser.BirthDate == null ? "" : GetDateText(StateManager.Instance.AppUser.BirthDate);
        //fldSSN.TextValue = "***-**-" + StateManager.Instance.AppUser.SSN.Substring(StateManager.Instance.AppUser.SSN.Length - 4);

        fldPhone.TextValue = vllCountry.FindRecordCellString(WebManager.Instance.WebSysUser.PhoneCountryId, "PhonePrefix") + " " + WebManager.Instance.WebSysUser.Phone;
        fldEmail.TextValue = WebManager.Instance.WebSysUser.Email;

        // Next Page
        //SandwichMenu.Instance.Close();
        //PageManager.Instance.ChangePage(nextPage);
    }

    private String GetDateText(DateTime date)
    {
        return $"{StateManager.Instance.MonthNames[date.Month - 1]} {date.Day}, {date.Year}";
    }

    //public void UpdatePerson()
    //{
    //    if (!ElementHelper.Validate(elementValuesPerson))
    //        return;

    //    AppUser person = dtmPerson.BuildClass<AppUser>();

    //    int month = mwlBirthDate.GetSelectedIndex(0) + 1;
    //    int day = Convert.ToInt32(mwlBirthDate.GetSelectedValue(1).Values[0]);
    //    int year = Convert.ToInt32(mwlBirthDate.GetSelectedValue(2).Values[0]);
    //    person.BirthDate = new DateTime(year, month, day);

    //    if (DateTime.Today < person.BirthDate.AddYears(18))
    //    {
    //        ChoiceDialog.Instance.Error(minorError);
    //        return;
    //    }

    //    person.WebSysUserId = WebManager.Instance.WebSysUser.Id;

    //    appUserService.UpdatePerson(person);
    //}

    //public void CheckPerson()
    //{
    //    if (chkPerson.Checked)
    //        btnUpdate.Interactable = true;
    //    else
    //        btnUpdate.Interactable = false;
    //}

    //public int GetAge(DateTime birthDate)
    //{
    //    DateTime now = DateTime.Now;             // To avoid a race condition around midnight
    //    int age = now.Year - birthDate.Year;

    //    if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
    //        age--;

    //    return age;
    //}

    //public void ClearPerson()
    //{
    //    chkPerson.Uncheck();
    //    btnUpdate.Interactable = false;
    //    for (int i = 0; i < elementValuesPerson.Length; i++)
    //        elementValuesPerson[i].Clear();
    //}
}
