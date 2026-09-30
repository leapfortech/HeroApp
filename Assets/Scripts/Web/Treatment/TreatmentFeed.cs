using System;
using UnityEngine;

using Leap.Graphics.Tools;

public class TreatmentFeed
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public String TitleImage
    {
        get => null;
        set => TitleSprite = value?.CreateSprite("Treatment_" + PostId.ToString("D02"));
    }
    public Sprite TitleSprite { get; set; } = null;
    public String Title { get; set; }
    public int Favorite { get; set; } = 0;

    public DateTime PublicationDateTime { get; set; } = new DateTime(1753, 1, 1);
}
