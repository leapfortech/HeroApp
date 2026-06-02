using Leap.UI.Elements;
using System;
using UnityEngine;

public class CountryFlag
{
    public String Name { get; set; }
    public String Code { get; set; }
    public String PhonePrefixName { get; set; }
    public Sprite Flag { get; set; }

    public CountryFlag()
    {
    }

    public CountryFlag(String name, String code, String phonePrefixName, Sprite flag)
    {
        Name = name;
        Code = code;
        PhonePrefixName = phonePrefixName;
        Flag = flag;
    }
}
