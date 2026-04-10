using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.Data.Web;
using Leap.Data.Collections;

using Sirenix.OdinInspector;


public class AccountDisplayAction : MonoBehaviour
{
    [Title("Account")]
    [SerializeField]
    Text[] txtAliass = null;
    [Space]
    [SerializeField]
    Text txtEmail = null;
    [SerializeField]
    Text txtPhone = null;

    [Title("Data")]
    [SerializeField]
    ValueList vllCountry = null;

    public void Clear()
    {
        for (int i = 0; i < txtAliass.Length; i++)
            txtAliass[i].Clear();

        txtEmail.Clear();
        txtPhone.Clear();
    }


    public void DisplayAccount()
    {
        for (int i = 0; i < txtAliass.Length; i++)
            txtAliass[i].TextValue = StateManager.Instance.AppUser.Alias;

        bool isEmailEmpty = true, isPhoneEmpty = true;

        isEmailEmpty = String.IsNullOrEmpty(WebManager.Instance.WebSysUser.Email);
        isPhoneEmpty = String.IsNullOrEmpty(WebManager.Instance.WebSysUser.Phone);

        txtEmail.TextValue = isEmailEmpty ? "Correo no ingresado" : WebManager.Instance.WebSysUser.Email;
        txtPhone.TextValue = isPhoneEmpty ? "Teléfono no ingresado" : vllCountry.FindRecordCellString(WebManager.Instance.WebSysUser.PhoneCountryId, "PhonePrefix") + " " + WebManager.Instance.WebSysUser.Phone;
    }
}
