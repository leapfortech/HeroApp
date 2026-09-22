using System;
using UnityEngine;

using Leap.Graphics.Tools;

public class RadioFeed
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public String TitleImage
    {
        get => null;
        set => TitleSprite = value?.CreateSprite("Radio_" + PostId.ToString("D02"));
    }
    public Sprite TitleSprite { get; set; } = null;
    public String Title { get; set; }
    public String RadioType { get; set; }
    public String PostCountry { get; set; }
    public String PostState { get; set; }
    public String Url { get; set; }
    public int Favorite { get; set; } = 0;

    public DateTime PublicationDateTime { get; set; } = new DateTime(1753, 1, 1);
}