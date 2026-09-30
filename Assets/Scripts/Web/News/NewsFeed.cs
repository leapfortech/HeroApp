using System;
using UnityEngine;

using Leap.Graphics.Tools;

public class NewsFeed
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
    public DateTime? DateTime { get; set; }
    public String NewsType { get; set; }
    public String Source { get; set; }
    public String Alias { get; set; }
    public int[] ReactionCounts { get; set; }
    public long ReactionPhraseId { get; set; }
    public int CommentCount { get; set; }

    public DateTime PublicationDateTime { get; set; } = new DateTime(1753, 1, 1);

    public NewsFeed()
    {
    }

    public NewsFeed(long id, long postId, string titleImage, Sprite titleSprite, string title, string description, DateTime? dateTime,
                    string newsType, string source, string alias, int[] reactionCounts, long reactionPhraseId, int commentCount, DateTime publicationDateTime)
    {
        Id = id;
        PostId = postId;
        TitleImage = titleImage;
        TitleSprite = titleSprite;
        Title = title;
        Description = description;
        DateTime = dateTime;
        NewsType = newsType;
        Source = source;
        Alias = alias;
        ReactionCounts = reactionCounts;
        ReactionPhraseId = reactionPhraseId;
        CommentCount = commentCount;
        PublicationDateTime = publicationDateTime;
    }

    public NewsFeed(NewsFull newsFull)
    {
        Id = newsFull.Id;
        PostId = newsFull.PostId;
        TitleSprite = newsFull.TitleSprite;
        Title = newsFull.Title;
        Description = newsFull.Description;
        DateTime = newsFull.DateTime;
        NewsType = newsFull.NewsTypeId;
        Source = newsFull.Source;
        Alias = newsFull.AppUserAlias;
        ReactionCounts = newsFull.ReactionCounts;
        ReactionPhraseId = newsFull.ReactionPhraseId;
        CommentCount = newsFull.CommentCount;
        PublicationDateTime = newsFull.PublicationDateTime;
    }
}