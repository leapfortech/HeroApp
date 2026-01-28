using UnityEngine;

using Leap.UI.Elements;
using Leap.Data.Collections;

using Sirenix.OdinInspector;

public class AddressDisplayAction : MonoBehaviour
{
    [Title("Info")]
    [SerializeField]
    FieldValue fldCountry = null;
    [SerializeField]
    FieldValue fldState = null;
    //[SerializeField]
    //FieldValue fldCity = null;

    [Title("Data")]
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    //[SerializeField]
    //ValueList vllCity = null;

    public void Display()
    {
        Address address = StateManager.Instance.Address;

        fldCountry.TextValue = vllCountry.FindRecordCellString(address.CountryId, "Name");
        fldState.TextValue = address.StateId == -1 ? "-" : vllState.FindRecordCellString(address.StateId, "Name");
        //fldCity.TextValue = address.OriginCityId == -1 ? "-" : vllCity.FindRecordCellString(address.CityId, "Name");
    }
}