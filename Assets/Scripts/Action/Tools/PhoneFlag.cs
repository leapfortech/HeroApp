using Leap.UI.Elements;
using System;
using UnityEngine;

public class PhoneFlag
{
    public String PhonePrefixName { get; set; }
    public String Name { get; set; }
    public String Code { get; set; }
    public Sprite Flag { get; set; }

    public PhoneFlag()
    {
    }

    public PhoneFlag(String phonePrefixName, String name, String code, Sprite flag)
    {
        PhonePrefixName = phonePrefixName;
        Name = name;
        Code = code;
        Flag = flag;
    }
}
