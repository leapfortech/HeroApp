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
    Text txtEmail = null;
    [SerializeField]
    Text txtPhone = null;

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

        txtEmail.Clear();
        txtPhone.Clear();
    }


    public void DisplayAccount()
    {
        for (int i = 0; i < txtAliass.Length; i++)
            txtAliass[i].TextValue = StateManager.Instance.AppUser.Alias;

        for (int i = 0; i < ifdAliass.Length; i++)
            ifdAliass[i].Text = StateManager.Instance.AppUser.Alias;

        bool isPhone = WebManager.Instance.WebSysUser.Email.StartsWith("hm.") &&
                       WebManager.Instance.WebSysUser.Email.EndsWith("@heroesmigrantes.com");

        txtEmail.TextValue = !isPhone ? WebManager.Instance.WebSysUser.Email : "Correo no ingresado";
        btnUpdateEmail.Interactable = !isPhone;

        txtPhone.TextValue = isPhone ? vllCountry.FindRecordCellString(WebManager.Instance.WebSysUser.PhoneCountryId, "PhonePrefix") + " " + WebManager.Instance.WebSysUser.Phone : "Teléfono no ingresado";
        btnUpdatePhone.Interactable = isPhone;


    }
}
