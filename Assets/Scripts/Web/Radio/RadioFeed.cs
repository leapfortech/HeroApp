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

    public RadioFeed()
    {
    }

    public RadioFeed(long id, long postId, string titleImage, Sprite titleSprite, string title, string radioType, string postCountry, string postState, string url, int favorite, DateTime publicationDateTime)
    {
        Id = id;
        PostId = postId;
        TitleImage = titleImage;
        TitleSprite = titleSprite;
        Title = title;
        RadioType = radioType;
        PostCountry = postCountry;
        PostState = postState;
        Url = url;
        Favorite = favorite;
        PublicationDateTime = publicationDateTime;
    }

    public RadioFeed(RadioFull radioFull)
    {
        Id = radioFull.Id;
        PostId = radioFull.PostId;
        TitleSprite = radioFull.TitleSprite;
        Title = radioFull.Title;
        RadioType = FeedHelper.Instance.GetRadioType(radioFull.RadioTypeFulls[0].RadioTypeId);
        PostCountry = FeedHelper.Instance.GetCountry(radioFull.PostCountryId);
        PostState = FeedHelper.Instance.GetState(radioFull.PostStateId);
        Url = radioFull.LinkFulls[0].Url;
        Favorite = radioFull.Favorite;
        PublicationDateTime = radioFull.PublicationDateTime;
    }
}