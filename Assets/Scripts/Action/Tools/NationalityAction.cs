using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Dialog;
using Leap.UI.Extensions;
using Leap.Data.Collections;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;

public class NationalityAction : MonoBehaviour
{
    [Title("Parameters")]
    [SerializeField]
    ComboAdapter cmbNationality = null;

    [SerializeField]
    int maxNationalities = 10;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmNationalityVLL = null;

    [SerializeField]
    DataMapper dtmNationalityLST = null;

    [SerializeField]
    ValueList vllNationalities = null;

    [SerializeField]
    ValueList vllCountries = null;

    [SerializeField]
    ValueList vllFlags = null;

    public void AddNationality(DpiFront dpiFront)
    {
        int countryId = -1;

        if (dpiFront.Nationality == null)
            return;

        countryId = Convert.ToInt32(vllCountries.FindRecordId("Code", dpiFront.Nationality));
        
        if (countryId == -1)
            countryId = Convert.ToInt32(vllCountries.FindRecordId("Name", dpiFront.Nationality));

        if (countryId == -1)
            return;

        String countryCode = vllCountries.FindRecordCellString(countryId, "Code");
        String countryName = vllCountries.FindRecordCellString(countryId,"Name");
        
        AddNationality(countryId, countryCode, countryName);
    }

    public void AddNationality(String[] nationalities)
    {
        if (nationalities.Length == 0)
            return;

        for (int i = 0; i < nationalities.Length; i++)
        {
            int countryId = Convert.ToInt32(nationalities[i]);
            String countryCode = vllCountries.FindRecordCellString(countryId, "Code");
            String countryName = vllCountries.FindRecordCellString(countryId, "Name");

            AddNationality(countryId, countryCode, countryName);
        }
    }

    public void AddNationality()
    {
        if (cmbNationality.Combo.IsEmpty())
            return;

        String cmbNationalityCode = cmbNationality.GetSelectedCellString("Code");

        int cmbCountryId = cmbNationality.GetSelectedId();

        AddNationality(cmbCountryId, cmbNationalityCode, cmbNationality.Combo.Text);

        cmbNationality.Clear();
    }

    private void AddNationality(int countryId, String countryCode, String countryName)
    {
        if (vllNationalities.RecordCount >= maxNationalities)
        {
            ChoiceDialog.Instance.Error("No se pueden ingresar más de " + maxNationalities + " Nacionalidades.");
            return;
        }

        String vllNationalityCode = vllNationalities.FindRecordCellString("Code", countryCode, "Code");
        if (vllNationalityCode != null)
        {
            ChoiceDialog.Instance.Error("<b>" + countryName + "</b> ya está en la lista.");
            return;
        }

        if (countryCode == null)
            countryCode = "Flags_NON";

        vllNationalities.AddRecord(countryId, countryName, countryCode, vllFlags.FindRecordCellSprite("Code", countryCode, "Flag"));

        List<CountryFlag> countryFlag = dtmNationalityVLL.BuildClassList<CountryFlag>();
        dtmNationalityLST.PopulateClassList(countryFlag);
    }

    public void RemoveRecord(int recordIdx)
    {
        vllNationalities.RemoveRecord(recordIdx);

        List<CountryFlag> enrollNationality = dtmNationalityVLL.BuildClassList<CountryFlag>();
        dtmNationalityLST.PopulateClassList(enrollNationality);
    }
}
