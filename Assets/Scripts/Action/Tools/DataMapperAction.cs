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

    public void FillBankLogoTmp()
    {
        ValueList valueBank = (ValueList)parameters[0];
        ValueList valueBankLogo = (ValueList)parameters[1];

        List<ValueRecord> bankRecords = valueBank.GetRecords();
        BankLogo[] bankLogos = new BankLogo[bankRecords.Count];
        
        for (int i = 0; i < bankLogos.Length; i++)
        {
            bankLogos[i] = new BankLogo();
            bankLogos[i].Name = bankRecords[i].GetCellString("Name");
            bankLogos[i].Logo = valueBankLogo.FindRecordCellSprite(valueBankLogo[i].Id, "Logo");
        }

        dataMappers[0].ClearRecords();
        dataMappers[0].PopulateClassArray(bankLogos);

        ValueList valueBankLogoTmp = (ValueList)parameters[2];

        for (int i = 0; i < valueBankLogoTmp.RecordCount; i++)
            valueBankLogoTmp[i].Id = i + 1;
        valueBankLogoTmp.RebuildIdIndexer();
    }

    public void FillAccountHpbTmp()
    {
        ValueList valueAccoutHpb = (ValueList)parameters[0];
        ValueList valueBankLogo = (ValueList)parameters[1];

        List<ValueRecord> accountHpbRecords = valueAccoutHpb.GetRecords();
        BankAccountHpb[] accountsHpb = new BankAccountHpb[accountHpbRecords.Count];

        for (int i = 0; i < accountsHpb.Length; i++)
        {
            accountsHpb[i] = new BankAccountHpb();
            accountsHpb[i].BankId = Convert.ToInt32(accountHpbRecords[i].GetCellString("BankId"));
            accountsHpb[i].BankName = valueBankLogo.GetRecordCellString(accountsHpb[i].BankId - 1, "Name");
            accountsHpb[i].TypeId = Convert.ToInt32(accountHpbRecords[i].GetCellString("TypeId"));
            accountsHpb[i].CurrencyId = Convert.ToInt32(accountHpbRecords[i].GetCellString("CurrencyId"));
            accountsHpb[i].Number = accountHpbRecords[i].GetCellString("Number");
            accountsHpb[i].Logo = valueBankLogo.FindRecordCellSprite(accountsHpb[i].BankId, "Logo");
        }
        
        dataMappers[0].ClearRecords();
        dataMappers[0].PopulateClassArray(accountsHpb);

        ValueList valueAccountHpbTmp = (ValueList)parameters[2];

        for (int i = 0; i < valueAccountHpbTmp.RecordCount; i++)
            valueAccountHpbTmp[i].Id = i + 1;
        valueAccountHpbTmp.RebuildIdIndexer();
    }
}