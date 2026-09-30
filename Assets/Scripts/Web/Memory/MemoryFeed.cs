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
        set => TitleSprite = value?.CreateSprite("Memory_" + PostId.ToString("D02"));
    }
    public Sprite TitleSprite { get; set; } = null;
    public String Title { get; set; }
    public String Country { get; set; }
    public String State { get; set; }
    public DateTime? DateTime { get; set; }
    public int[] ReactionCounts { get; set; }
    public long ReactionPhraseId { get; set; }

    public DateTime PublicationDateTime { get; set; } = new DateTime(1753, 1, 1);

    public MemoryFeed()
    {
    }

    public MemoryFeed(long id, long postId, string titleImage, Sprite titleSprite, string title, string country, string state,
                      DateTime? dateTime, int[] reactionCounts, long reactionPhraseId, DateTime publicationDateTime)
    {
        Id = id;
        PostId = postId;
        TitleImage = titleImage;
        TitleSprite = titleSprite;
        Title = title;
        Country = country;
        State = state;
        DateTime = dateTime;
        ReactionCounts = reactionCounts;
        ReactionPhraseId = reactionPhraseId;
        PublicationDateTime = publicationDateTime;
    }

    public MemoryFeed(MemoryFull memoryFull)
    {
        Id = memoryFull.Id;
        PostId = memoryFull.PostId;
        TitleSprite = memoryFull.TitleSprite;
        Title = memoryFull.Title;
        Country = memoryFull.CountryId;
        State = memoryFull.StateId;
        DateTime = memoryFull.DateTime;
        ReactionCounts = memoryFull.ReactionCounts;
        ReactionPhraseId = memoryFull.ReactionPhraseId;
        PublicationDateTime = memoryFull.PublicationDateTime;
    }
}
