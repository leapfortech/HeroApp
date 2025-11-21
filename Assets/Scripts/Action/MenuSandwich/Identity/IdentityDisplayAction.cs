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
    [Title("Info")]
    [SerializeField]
    FieldValue fldFirstName1 = null;
    [SerializeField]
    FieldValue fldFirstName2 = null;
    [SerializeField]
    FieldValue fldLastName1 = null;
    [SerializeField]
    FieldValue fldLastName2 = null;
    //[SerializeField]
    //FieldValue fldGender = null;
    [SerializeField]
    FieldValue fldBirthDate = null;
    //[SerializeField]
    //FieldValue fldOriginCountry = null;
    //[SerializeField]
    //FieldValue fldOriginState = null;

    [Title("Data")]
    //[SerializeField]
    //ValueList vllGender = null;
    //[SerializeField]
    //ValueList vllCountry = null;
    //[SerializeField]
    //ValueList vllState = null;
    //[SerializeField]
    //ValueList vllFlags = null;

    IdentityService identityService = null;

    private void Awake()
    {
        identityService = GetComponent<IdentityService>();
    }

    public void Clear()
    {

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

    //public void ApplyDpiPhoto(DpiPhoto dpiPhoto)
    //{
    //    //StateManager.Instance.DpiFront = dpiPhoto.DpiFront != null ? dpiPhoto.DpiFront.CreateSprite("DpiFrontDisplay") : null;
    //    //StateManager.Instance.DpiBack = dpiPhoto.DpiBack != null ? dpiPhoto.DpiBack.CreateSprite("DpiBackDisplay") : null;

    //    PageManager.Instance.ChangePage(identityPage);
    //    SandwichMenu.Instance.Close();
    //}

    private void Display()
    {
        Clear();
        Identity identity = StateManager.Instance.Identity;

        //if (StateManager.Instance.DpiFront != null)
        //    imgDpiFront.Sprite = StateManager.Instance.DpiFront;

        //if (StateManager.Instance.DpiBack != null)
        //    imgDpiBack.Sprite = StateManager.Instance.DpiBack;

        //bool valid = DateTime.Today <= date;
        //txtStatusValid.gameObject.SetActive(valid);
        //txtStatusExpired.gameObject.SetActive(!valid);


        fldFirstName1.TextValue = identity.FirstName1;
        fldFirstName2.TextValue = identity.FirstName2 == null ? "-" : identity.FirstName2;
        fldLastName1.TextValue = identity.LastName1;
        fldLastName2.TextValue = identity.LastName2 == null ? "-" : identity.LastName2;
        //fldGender.TextValue = vllGender.FindRecordCellString(identity.GenderId, "Name");
        fldBirthDate.TextValue = identity.BirthDate.Day + " de " +
                                 StateManager.Instance.MonthNames[identity.BirthDate.Month - 1] + ", " +
                                 identity.BirthDate.Year;
        //fldBirthCountry.TextValue = vllCountry.FindRecordCellString(identity.OriginCountryId, "Name");
        //fldBirthState.TextValue = identity.OriginStateId == -1 ? "-" : vllState.FindRecordCellString(identity.OriginStateId, "Name");
    }
}