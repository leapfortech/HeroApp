using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.Data.Web;
using Leap.Data.Collections;

using Sirenix.OdinInspector;


public class LocalityDisplayAction : MonoBehaviour
{
    [Title("Interest")]
    [SerializeField]
    Text txtInterestCountry = null;
    [SerializeField]
    Text txtInterestState = null;
    [SerializeField]
    Text txtInterestCity = null;

    [Title("Current")]
    [SerializeField]
    Text txtCurrentCountry = null;
    [SerializeField]
    Text txtCurrentState = null;
    [SerializeField]
    Text txtCurrentCity = null;

    [Title("Data")]
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    [SerializeField]
    ValueList vllCity = null;

    public void Clear()
    {
        txtInterestCountry.Clear();
        txtInterestState.Clear();
        txtInterestCity.Clear();

        txtCurrentCountry.Clear();
        txtCurrentState.Clear();
        txtCurrentCity.Clear();
    }


    public void DisplayLocality()
    {
        Locality interestLocality = StateManager.Instance.InterestLocality;
        Locality currentLocality = StateManager.Instance.CurrentLocality;

        bool isInterestEmpty = interestLocality == null;
        bool isCurrentEmpty = currentLocality == null;

        // Interest locality
        txtInterestCountry.TextValue = isInterestEmpty ? "No definido" : vllCountry.FindRecordCellString(interestLocality.CountryId, "Name");
        txtInterestState.TextValue = isInterestEmpty ? "-" : vllState.FindRecordCellString(interestLocality.StateId, "Name");
        txtInterestCity.TextValue = isInterestEmpty ? "-" : vllCity.FindRecordCellString(interestLocality.CityId, "Name");

        // Current locality
        txtCurrentCountry.TextValue = isCurrentEmpty ? "No definido" : vllCountry.FindRecordCellString(currentLocality.CountryId, "Name");
        txtCurrentState.TextValue = isCurrentEmpty ? "-" : vllState.FindRecordCellString(currentLocality.StateId, "Name");
        txtCurrentCity.TextValue = isCurrentEmpty ? "-" : vllCity.FindRecordCellString(currentLocality.CityId, "Name");
    }
}
