using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.Data.Mapper;
using Leap.UI.Extensions;

using Sirenix.OdinInspector;

public class PlaceDependencyAction : MonoBehaviour
{
    [Title("Element")]
    [SerializeField]
    ComboAdapter cmbCountry = null;

    [SerializeField]
    ComboAdapter cmbState = null;

    [SerializeField]
    ComboAdapter cmbCity = null;

    [Title("Element")]
    [SerializeField]
    DataMapper dtmState = null;
    [SerializeField]
    DataMapper dtmCity = null;

    private bool initialized = false;
    HashSet<long> countriesWithState;
    HashSet<long> countriesWithCity;

    private HashSet<long> ParseCountryList(string value)
    {
        HashSet<long> result = new HashSet<long>();

        if (String.IsNullOrWhiteSpace(value))
            return result;

        String[] countryIds = value.Split('|');

        foreach (String countryId in countryIds)
        {
            if (long.TryParse(countryId, out long id))
                result.Add(id);
        }

        return result;
    }

    public void Initialize()
    {
        if (initialized)
            return;

        countriesWithState = ParseCountryList(AppManager.Instance.GetParamValue("CountriesWithState"));
        countriesWithCity = ParseCountryList(AppManager.Instance.GetParamValue("CountriesWithCity"));

        cmbCountry.gameObject.SetActive(true);
        cmbState.gameObject.SetActive(false);
        cmbCity.gameObject.SetActive(false);

        initialized = true;
    }

    public void Clear()
    {
        if (cmbCountry != null)
            cmbCountry.Clear();

        if (cmbState != null)
        {
            cmbState.Clear();
            cmbState.gameObject.SetActive(false);
        }

        if (cmbCity != null)
        {
            cmbCity.Clear();
            cmbCity.gameObject.SetActive(false);
        }

        if (dtmState != null)
            dtmState.ClearRecords();

        if (dtmCity != null)
            dtmCity.ClearRecords();
    }

    public void RefreshCountry()
    {
        Initialize();
        
        dtmState.ClearRecords();
        dtmCity.ClearRecords();

        long countryId = cmbCountry.GetSelectedId();

        bool hasState = countriesWithState.Contains(countryId);
        bool hasCity = countriesWithCity.Contains(countryId);

        cmbState.gameObject.SetActive(hasState);
        cmbCity.gameObject.SetActive(hasCity);
    }

    public void RefreshState()
    {
        Initialize();

        long countryId = cmbCountry.GetSelectedId();

        if (countriesWithCity.Contains(countryId) && cmbState.GetSelectedId() != -1)
            cmbCity.gameObject.SetActive(true);
        else
            cmbCity.gameObject.SetActive(false);
    }
}
