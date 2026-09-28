using System;
using UnityEngine;

using Leap.Graphics.Tools;

public class HappeningFeed
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public String TitleImage
    {
        get => null;
        set => TitleSprite = value?.CreateSprite("News_" + PostId.ToString("D02"));
    }
    public Sprite TitleSprite { get; set; } = null;
    public String Title { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public String HappeningType { get; set; }
    public String Country { get; set; }
    public String State { get; set; }
    public String Location { get; set; }
    public int Selected { get; set; }
    public int Favorite { get; set; }
    public int FavoriteCount { get; set; }

    public DateTime PublicationDateTime { get; set; } = new DateTime(1753, 1, 1);
}
