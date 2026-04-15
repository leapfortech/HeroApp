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
    InputField[] ifdAliass = null;
    [Space]
    [SerializeField]
    Text[] txtEmails = null;
    [Space]
    [SerializeField]
    Text[] txtPhones = null;

    [Title("Actions")]
    [SerializeField]
    Button btnUpdateEmail = null;

    [SerializeField]
    Button btnUpdatePhone = null;

    [Title("Data")]
    [SerializeField]
    ValueList vllCountry = null;

    public void Clear()
    {
        for (int i = 0; i < txtAliass.Length; i++)
            txtAliass[i].Clear();

        for (int i = 0; i < txtEmails.Length; i++)
            txtEmails[i].Clear();

        for (int i = 0; i < txtPhones.Length; i++)
            txtPhones[i].Clear();
    }


    public void DisplayAccount()
    {
        for (int i = 0; i < txtAliass.Length; i++)
            txtAliass[i].TextValue = StateManager.Instance.AppUser.Alias;

        for (int i = 0; i < ifdAliass.Length; i++)
            ifdAliass[i].Text = StateManager.Instance.AppUser.Alias;

        bool isPhone = WebManager.Instance.WebSysUser.Email.StartsWith("hm.") &&
                       WebManager.Instance.WebSysUser.Email.EndsWith("@heroesmigrantes.com");

        for (int i = 0; i < txtEmails.Length; i++)
            txtEmails[i].TextValue = !isPhone ? WebManager.Instance.WebSysUser.Email : "Correo no ingresado";
        
        btnUpdateEmail.Interactable = !isPhone;

        for (int i = 0; i < txtPhones.Length; i++)
            txtPhones[i].TextValue = isPhone ? vllCountry.FindRecordCellString(WebManager.Instance.WebSysUser.PhoneCountryId, "PhonePrefix") + " " + WebManager.Instance.WebSysUser.Phone : "Teléfono no ingresado";
        
        btnUpdatePhone.Interactable = isPhone;
    }
}
