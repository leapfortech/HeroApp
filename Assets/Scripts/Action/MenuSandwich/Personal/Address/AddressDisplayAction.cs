using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.Data.Collections;

using Sirenix.OdinInspector;
using Leap.UI.Dialog;
using Leap.Graphics.Tools;
using UnityEngine.Events;
using Leap.UI.Page;

public class AddressDisplayAction : MonoBehaviour
{
    [Title("Display")]
    [SerializeField]
    FieldValue fldAddress1 = null;
    [SerializeField]
    FieldValue fldAddress2 = null;
    [SerializeField]
    FieldValue fldCountry = null;
    [SerializeField]
    FieldValue fldState = null;
    [SerializeField]
    FieldValue fldCity = null;

    [Title("Data")]
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    [SerializeField]
    ValueList vllCity = null;

    [Title("Page")]
    [SerializeField]
    Page addressPage = null;

    AddressService addressService = null;

    private void Awake()
    {
        addressService = GetComponent<AddressService>();
    }

    public void Clear()
    {
        fldAddress1.Clear();
        fldAddress2.Clear();
        fldState.Clear();
        fldCity.Clear();
    }

    public void GetAddress()
    {
        ScreenDialog.Instance.Display();
        addressService.GetAddress();
    }

    public void ApplyAddress(Address address)
    {       
        PageManager.Instance.ChangePage(addressPage);
        SandwichMenu.Instance.Close();
    }

    public void DisplayAddress()
    {
        Address address = StateManager.Instance.Address;

        fldAddress1.TextValue = address.Address1;
        fldAddress2.TextValue = String.IsNullOrEmpty(address.Address2) ? "-" : address.Address2;
        fldCountry.TextValue = vllCountry.FindRecordCellString(address.CountryId, "Name");
        fldState.TextValue = vllState.FindRecordCellString(address.StateId, "Name");
        fldCity.TextValue = vllCity.FindRecordCellString(address.CityId, "Name");
    }
}