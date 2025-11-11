using Leap.UI.Elements;
using System;
using UnityEngine;

public class BankLogo
{
    public String Name { get; set; }
    public Sprite Logo { get; set; }

    public BankLogo()
    {
    }

    public BankLogo(string name, Sprite logo)
    {
        Name = name;
        Logo = logo;
    }
}
