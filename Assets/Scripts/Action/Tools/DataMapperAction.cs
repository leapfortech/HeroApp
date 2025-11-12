using System.Collections.Generic;
using UnityEngine;

using Leap.Data.Collections;
using Leap.Data.Mapper;
using Leap.Data.Web;
using System;

public class DataMapperAction : MonoBehaviour
{
    [Space]
    [SerializeField]
    DataMapper[] dataMappers = null;

    public DataMapper[] DataMappers => dataMappers;
    public DataMapper DataMapper => dataMappers == null ? null : dataMappers[0];

    [Space]
    [SerializeField]
    UnityEngine.Object[] parameters = null;

    public void FillNotifications(LoginAppInfo loginAppInfo)
    {
        FillNotifications(loginAppInfo.Notifications);
    }

    public void FillNotifications(Notification[] notifications)
    {
        for(int i = 0; i < notifications.Length; i++)
            notifications[i].DateTime = notifications[i].DateTime.ToLocalTime();
        
        for (int i = 0; i < dataMappers.Length; i++)
        {
            dataMappers[i].ClearRecords();
            dataMappers[i].PopulateClassArray(notifications);
        }
    }

    public void FillCountryFlagAll()
    {
        ValueList valueCountryAll = (ValueList)parameters[0];
        ValueList valueFlag = (ValueList)parameters[1];

        List<ValueRecord> countryRecords = valueCountryAll.GetRecords();
        CountryFlag[] countryFlags = new CountryFlag[countryRecords.Count];
        for (int i = 0; i < countryFlags.Length; i++)
        {
            countryFlags[i] = new CountryFlag();
            countryFlags[i].Name = countryRecords[i].GetCellString("Name");
            countryFlags[i].Code = countryRecords[i].GetCellString("Code");
            countryFlags[i].Flag = valueFlag.FindRecordCellSprite("Code", countryFlags[i].Code, "Flag");
        }

        dataMappers[0].ClearRecords();
        dataMappers[0].PopulateClassArray(countryFlags);

        ValueList valueCountryFlagAll = (ValueList)parameters[2];
        for (int i = 0; i < valueCountryFlagAll.RecordCount; i++)
            valueCountryFlagAll[i].Id = i + 1; // valueCountryAll.FindRecord("Code", valueCountryFlagAll[i].GetCellString("Code")).Id;
        valueCountryFlagAll.RebuildIdIndexer();
    }
}