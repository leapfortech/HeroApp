using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;
using Leap.UI.Extensions;

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

    public void Initialize()
    {
        cmbCountry.gameObject.SetActive(true);
        cmbState.gameObject.SetActive(false);
        cmbCity.gameObject.SetActive(false);
    }

    public void Clear()
    {
        cmbCountry.Clear();
        cmbState.Clear();
        cmbCity.Clear();
    }

    public void RefreshCountry()
    {
        dtmState.ClearRecords();
        dtmCity.ClearRecords();
        if ((cmbCountry.Combo.Text != null) && (cmbCountry.GetSelectedId() == 84))
            cmbState.gameObject.SetActive(true);
        else
            cmbState.gameObject.SetActive(false);
    }

    public void RefreshState()
    {
        if (cmbState.GetSelectedId() != -1)
            cmbCity.gameObject.SetActive(true);
        else
            cmbCity.gameObject.SetActive(false);
    }
}
