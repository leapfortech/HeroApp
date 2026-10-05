using System;
using UnityEngine;

using Leap.UI.Extensions;

using Sirenix.OdinInspector;


public class CountryDefaultIndexAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ComboAdapter cmbSaleCountry = null;
    [SerializeField]
    ComboAdapter cmbPhonePrefix = null;
    [SerializeField]
    ComboAdapter cmbWhatsAppPrefix = null;

    public bool isUpdate { get; set; } = false;

    public void SetDefaultIndexes()
    {
        if (isUpdate)
            return;

        long selectedCountryId = cmbSaleCountry.GetSelectedId();
        
        cmbPhonePrefix.Select(selectedCountryId);
        cmbWhatsAppPrefix.Select(selectedCountryId);
    }
}
