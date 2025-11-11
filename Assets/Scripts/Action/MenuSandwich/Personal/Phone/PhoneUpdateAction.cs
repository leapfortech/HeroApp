using System;

using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.UI.Extensions;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class PhoneUpdateAction : MonoBehaviour
{
    [Title("Phone")]
    [SerializeField]
    ComboAdapter cmbPhoneCountry = null;

    [SerializeField]
    InputField ifdPhoneNumber = null;

    [SerializeField]
    Page nextPageUpdated = null;

    AppUserService appUserService = null;
    PhoneRequest phoneRequest = null;


    private void Awake()
    {
        appUserService = GetComponent<AppUserService>();
    }

    public void UpdatePhone()
    {
        phoneRequest = new PhoneRequest(StateManager.Instance.AppUser.Id, cmbPhoneCountry.GetSelectedId(), ifdPhoneNumber.Text);
        appUserService.UpdatePhone(phoneRequest);    
    }
    
    public void ApplyPhoneUpdate()
    {
        WebManager.Instance.WebSysUser.PhoneCountryId = phoneRequest.PhoneCountryId;
        WebManager.Instance.WebSysUser.Phone = phoneRequest.Phone;
        PageManager.Instance.ChangePage(nextPageUpdated);
    }
}