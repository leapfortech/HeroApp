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
        set => TitleSprite = value?.CreateSprite("Happening_" + PostId.ToString("D02"));
    }
    public Sprite TitleSprite { get; set; } = null;
    public String Title { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public String HappeningType { get; set; }
    public String Country { get; set; }
    public String State { get; set; }
    public String Location { get; set; }
    public int Favorite { get; set; }
    public int Selected { get; set; }
    public int FavoriteCount { get; set; }

    public DateTime PublicationDateTime { get; set; } = new DateTime(1753, 1, 1);

    public HappeningFeed()
    {
    }

    public HappeningFeed(long id, long postId, string titleImage, Sprite titleSprite, string title, DateTime? startDateTime, DateTime? endDateTime,
                         string happeningType, string country, string state, string location, int favorite, int selected, int favoriteCount, DateTime publicationDateTime)
    {
        Id = id;
        PostId = postId;
        TitleImage = titleImage;
        TitleSprite = titleSprite;
        Title = title;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        HappeningType = happeningType;
        Country = country;
        State = state;
        Location = location;
        Favorite = favorite;
        Selected = selected;
        FavoriteCount = favoriteCount;
        PublicationDateTime = publicationDateTime;
    }

    public HappeningFeed(HappeningFull happeningFull)
    {
        Id = happeningFull.Id;
        PostId = happeningFull.PostId;
        TitleSprite = happeningFull.TitleSprite;
        Title = happeningFull.Title;
        StartDateTime = happeningFull.StartDateTime;
        EndDateTime = happeningFull.EndDateTime;
        HappeningType = FeedHelper.Instance.GetHappeningType(happeningFull.HappeningTypeId);
        Country = FeedHelper.Instance.GetCountry(happeningFull.CountryId);
        State = FeedHelper.Instance.GetState(happeningFull.StateId);
        Location = happeningFull.Location;
        Favorite = happeningFull.Favorite;
        Selected = happeningFull.Selected;
        FavoriteCount = happeningFull.FavoriteCount;
        PublicationDateTime = happeningFull.PublicationDateTime;
    }
}
