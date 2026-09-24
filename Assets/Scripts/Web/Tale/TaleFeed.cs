using System;
using UnityEngine;

using Leap.Graphics.Tools;

public class TaleFeed
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
    public String Description { get; set; }
    public int[] ReactionCounts { get; set; }
    public long ReactionPhraseId { get; set; }
    public int CommentCount { get; set; }
    public String Alias { get; set; }
    public String InterestLocality { get; set; }
    public String CurrentLocality { get; set; }

    public DateTime PublicationDateTime { get; set; } = new DateTime(1753, 1, 1);
}

