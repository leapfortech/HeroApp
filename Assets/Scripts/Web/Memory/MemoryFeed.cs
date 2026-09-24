using System;
using UnityEngine;

using Leap.Graphics.Tools;

public class MemoryFeed
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
    public String Country { get; set; }
    public String State { get; set; }
    public DateTime? DateTime { get; set; }
    public int[] ReactionCounts { get; set; }
    public long ReactionPhraseId { get; set; }

    public DateTime PublicationDateTime { get; set; } = new DateTime(1753, 1, 1);
}
