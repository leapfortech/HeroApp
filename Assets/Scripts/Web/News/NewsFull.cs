using System;
using System.Collections.Generic;

using UnityEngine;
using Leap.Graphics.Tools;

public class NewsFull : PostFull
{
    public long Id { get; set; }
    public long NewsTypeId { get; set; }
    public String Place { get; set; }
    public String Source { get; set; }
    public DateTime? DateTime { get; set; }
    public int Status { get; set; }
    public String[] Images
    {
        get => null;
        set
        {
            ImageSprites = new List<Sprite>();
            for (int i = 0; i < value.Length; i++)
                if (value[i] != null)
                    ImageSprites.Add(value[i].CreateSprite("News_" + i.ToString("D02")));
        }
    }
    public List<Sprite> ImageSprites { get; set; }

    public NewsFull()
    {
    }

    public NewsFull(long id, long postId, long appUserId, String appUserAlias,
                    long postSubtypeId,
                    long postCountryId, long postStateId,
                    String title, String titleImage, String summary, String description,
                    int imageCount, int favorite, int like, int likeCount, long reactionPhraseId,
                    DateTime publicationDateTime, int postStatus,
                    ContactFull contactFull, List<LinkFull> linkFulls, List<CommentFull> commentFulls,
                    long newsTypeId, String place,
                    String source, DateTime? dateTime,
                    int status,
                    String[] images)
        : base(postId, appUserId, appUserAlias, postSubtypeId,
                postCountryId, postStateId, title, titleImage, summary, description,
                imageCount, favorite, like, likeCount, reactionPhraseId, publicationDateTime, postStatus,
                contactFull, linkFulls, commentFulls)
    {
        Id = id;
        NewsTypeId = newsTypeId;
        Place = place;
        Source = source;
        DateTime = dateTime;
        Status = status;
        Images = images;
    }
}
