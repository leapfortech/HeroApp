using System;
using System.Collections.Generic;

using UnityEngine;
using Leap.Graphics.Tools;

public class PostFull
{
    public long PostId { get; set; } = -1L;
    public long AppUserId { get; set; } = -1L;
    public String AppUserAlias { get; set; } = null;
    public long PostTypeId { get; set; } = -1L;
    public long PostCountryId { get; set; } = -1L;
    public long PostStateId { get; set; } = -1L;
    public String Title { get; set; } = null;
    public String TitleImage 
    {
        get => null;
        set => TitleSprite = value?.CreateSprite("Title_" + PostId.ToString("D02"));
    }
    public Sprite TitleSprite { get; set; } = null;
    public String Description { get; set; } = null;
    public int ImageCount { get; set; } = 0;
    public int FavoriteCount { get; set; } = 0;
    public int[] ReactionCounts { get; set; } = null;
    public long ReactionPhraseId { get; set; } = -1L;
    public int CommentCount { get; set; } = 0;
    public DateTime PublicationDateTime { get; set; } = new DateTime(1753, 1, 1);
    public int PostStatus { get; set; } = -1;

    public AppUserInfo AppUserInfo { get; set; }
    public ContactFull ContactFull { get; set; } = null;
    public List<LinkFull> LinkFulls { get; set; } = null;
    public List<CommentFull> CommentFulls { get; set; } = null;

    public String[] Images
    {
        get => null;
        set
        {
            ImageSprites = new List<Sprite>();
            for (int i = 0; i < value.Length; i++)
                if (value[i] != null)
                    ImageSprites.Add(value[i].CreateSprite($"{PostType.Names[PostTypeId]}_{i:D02}"));
        }
    }
    public List<Sprite> ImageSprites { get; set; }

    public PostFull()
    {
    }

    public PostFull(long postId, long appUserId, String appUserAlias, long postTypeId, long postCountryId, long postStateId, String title, String titleImage, String description,
                    int imageCount, int favoriteCount, int[] reactionCounts, long reactionPhraseId, int commentCount, DateTime publicationDateTime, int postStatus,
                    AppUserInfo appUserInfo, ContactFull contactFull, List<LinkFull> linkFulls, List<CommentFull> commentFulls, String[] images)
    {
        PostId = postId;
        AppUserId = appUserId;
        AppUserAlias = appUserAlias;
        PostTypeId = postTypeId;
        PostCountryId = postCountryId;
        PostStateId = postStateId;
        Title = title;
        TitleImage = titleImage;
        Description = description;
        ImageCount = imageCount;
        FavoriteCount = favoriteCount;
        ReactionCounts = reactionCounts;
        ReactionPhraseId = reactionPhraseId;
        CommentCount = commentCount;
        PublicationDateTime = publicationDateTime;
        PostStatus = postStatus;

        AppUserInfo = appUserInfo;
        ContactFull = contactFull;
        LinkFulls = linkFulls;
        CommentFulls = commentFulls;

        Images = images;
    }

    public void Update(PostFull postFull)
    {
        Title = postFull.Title;
        Description = postFull.Description;
    }
}
