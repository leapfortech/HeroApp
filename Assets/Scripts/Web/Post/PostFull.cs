
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
        set => TitleSprite = value?.CreateSprite("Title" + PostId.ToString("D02"));
    }
    public Sprite TitleSprite { get; set; } = null;
    public String Summary { get; set; } = null;
    public String Description { get; set; } = null;
    public int ImageCount { get; set; } = 0;
    public int LikeCount { get; set; } = 0;
    public DateTime PublicationDateTime { get; set; } = new DateTime(1753, 1, 1);
    public int PostStatus { get; set; } = -1;

    public ContactFull ContactFull { get; set; } = null;
    public List<LinkFull> LinkFulls { get; set; } = null;
    public List<CommentFull> CommentFulls { get; set; } = null;

    public PostFull()
    {
    }

    public PostFull(long postId, long appUserId, String appUserAlias, long postTypeId, long postCountryId, long postStateId, String title, String titleImage,
                    String summary, String description, int imageCount, int likeCount, DateTime publicationDateTime, int postStatus,
                    ContactFull contactFull, List<LinkFull> linkFulls, List<CommentFull> commentFulls)
    {
        PostId = postId;
        AppUserId = appUserId;
        AppUserAlias = appUserAlias;
        PostTypeId = postTypeId;
        PostCountryId = postCountryId;
        PostStateId = postStateId;
        Title = title;
        TitleImage = titleImage;
        Summary = summary;
        Description = description;
        ImageCount = imageCount;
        LikeCount = likeCount;
        PublicationDateTime = publicationDateTime;
        PostStatus = postStatus;

        ContactFull = contactFull;
        LinkFulls = linkFulls;
        CommentFulls = commentFulls;
    }
}
