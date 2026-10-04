using System;
using UnityEngine;

using Leap.Core.Tools;
using Leap.Data.Collections;

using Sirenix.OdinInspector;
using Leap.Data.Mapper;

public class FeedHelper : SingletonBehaviour<FeedHelper>
{
    [Title("General")]
    [SerializeField]
    private ValueList vllCountry;

    [SerializeField]
    private ValueList vllState;

    [SerializeField]
    private ValueList vllCurrency;

    [Title("Types")]
    [SerializeField]
    private ValueList vllNewsType;

    [SerializeField]
    private ValueList vllHappeningType;

    [SerializeField]
    private ValueList vllProductSubtype;

    [SerializeField]
    private ValueList vllRadioType;

    [Title("Orphans")]
#pragma warning disable 414 
    [SerializeField]
    DataClass dataClass = null;
#pragma warning restore 414 

    // General
    public String GetCountry(long countryId)
    {
        return vllCountry.FindRecordCellString(countryId, "Name");
    }

    public String GetState(long stateId)
    {
        return vllState.FindRecordCellString(stateId, "Name");
    }

    public String GetCurrency(long currencyId)
    {
        return vllCurrency.FindRecordCellString(currencyId, "Symbol");
    }

    // Types
    public String GetNewsType(long newsTypeId)
    {
        return vllNewsType.FindRecordCellString(newsTypeId, "Name");
    }

    public String GetHappeningType(long happeningTypeId)
    {
        return vllHappeningType.FindRecordCellString(happeningTypeId, "Name");
    }

    public String GetProductSubtype(long productSubtypeId)
    {
        return vllProductSubtype.FindRecordCellString(productSubtypeId, "Name");
    }

    public String GetRadioType(long radioTypeId)
    {
        return vllRadioType.FindRecordCellString(radioTypeId, "Name");
    }

    // Delay
    public String GetFeedDelay(TimeSpan timeSpan)
    {
        String sDelay = "hace ";
        int delay = 0;
        if (timeSpan.TotalDays >= 365)
        {
            delay = (int)(timeSpan.TotalDays / 365);
            sDelay += delay.ToString() + (delay > 1 ? " años" : " año");
        }
        else if (timeSpan.TotalDays > 30)
        {
            delay = (int)(timeSpan.TotalDays / 30);
            sDelay += delay.ToString() + (delay > 1 ? " meses" : " mes");
        }
        else if (timeSpan.TotalDays >= 7)
        {
            delay = (int)(timeSpan.TotalDays / 7);
            sDelay += delay.ToString() + (delay > 1 ? " semanas" : " semana");
        }
        else if (timeSpan.TotalDays >= 1)
        {
            delay = (int)timeSpan.TotalDays;
            sDelay += delay.ToString() + (delay > 1 ? " días" : " día");
        }
        else if (timeSpan.TotalHours >= 1)
        {
            delay = (int)timeSpan.TotalHours;
            sDelay += delay.ToString() + (delay > 1 ? " horas" : " hora");
        }
        else if (timeSpan.TotalMinutes >= 1)
        {
            delay = (int)timeSpan.TotalMinutes;
            sDelay += delay.ToString() + (delay > 1 ? " minutos" : " minuto");
        }
        else
            sDelay = "ahora";
        return sDelay;
    }
}
