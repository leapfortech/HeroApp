using Leap.UI.Elements;
using System;
using UnityEngine;

public class CountryFlag
{
    public String Name { get; set; }
    public String Code { get; set; }
    public Sprite Flag { get; set; }

    public CountryFlag()
    {
    }

    public CountryFlag(string name, string code, Sprite flag)
    {
        Name = name;
        Code = code;
        Flag = flag;
    }
}
