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
        set => TitleSprite = value?.CreateSprite("Tale_" + PostId.ToString("D02"));
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

    public TaleFeed()
    {
    }

    public TaleFeed(long id, long postId, string titleImage, string title, string description,
                    int[] reactionCounts, long reactionPhraseId, int commentCount, string alias, string interestLocality, string currentLocality, DateTime publicationDateTime)
    {
        Id = id;
        PostId = postId;
        TitleImage = titleImage;
        Title = title;
        Description = description;
        ReactionCounts = reactionCounts;
        ReactionPhraseId = reactionPhraseId;
        CommentCount = commentCount;
        Alias = alias;
        InterestLocality = interestLocality;
        CurrentLocality = currentLocality;
        PublicationDateTime = publicationDateTime;
    }

    public TaleFeed(TaleFull taleFull)
    {
        Id = taleFull.Id;
        PostId = taleFull.PostId;
        TitleSprite = taleFull.TitleSprite;
        Title = taleFull.Title;
        Description = taleFull.Description;
        ReactionCounts = taleFull.ReactionCounts;
        ReactionPhraseId = taleFull.ReactionPhraseId;
        CommentCount = taleFull.CommentCount;
        Alias = taleFull.AppUserAlias;
        //InterestLocality = interestLocality;
        //CurrentLocality = currentLocality;
        PublicationDateTime = taleFull.PublicationDateTime;
    }
}

