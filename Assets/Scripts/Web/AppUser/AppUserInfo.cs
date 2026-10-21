using Leap.Graphics.Tools;
using System;
using UnityEngine;

public class AppUserInfo
{
    public long Id { get; set; }
    public String Alias { get; set; }
    public String Thumbnail
    {
        get => null;
        set => ThumbnailSprite = value?.CreateSprite("Thumbnail");
    }
    public Sprite ThumbnailSprite { get; set; } = null;
    public LocalityFull InterestLocality { get; set; }
    public LocalityFull CurrentLocality { get; set; }

    public AppUserInfo()
    {

    }

    public AppUserInfo(long id, String alias, LocalityFull interestLocality, LocalityFull currentLocality)
    {
        Id = id;
        Alias = alias;
        InterestLocality = interestLocality;
        CurrentLocality = currentLocality;
    }
}
