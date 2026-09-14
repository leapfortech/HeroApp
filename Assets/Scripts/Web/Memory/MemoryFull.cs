using System;
using System.Collections.Generic;

using UnityEngine;
using Leap.Graphics.Tools;

public class MemoryFull : PostFull
{
    public long Id { get; set; }
    public long MemoryTypeId { get; set; }
    public long CountryId { get; set; }
    public long StateId { get; set; }
    public DateTime? DateTime { get; set; }
    public String Location { get; set; }
    public int Status { get; set; }
    public String[] Images
    {
        get => null;
        set
        {
            ImageSprites = new List<Sprite>();
            for (int i = 0; i < value.Length; i++)
                if (value[i] != null)
                    ImageSprites.Add(value[i].CreateSprite("Happening_" + i.ToString("D02")));
        }
    }
    public List<Sprite> ImageSprites { get; set; }

    public MemoryFull()
    {
    }

    public MemoryFull(long id, long postId, long appUserId, String appUserAlias,
                      long postSubtypeId,
                      long postCountryId, long postStateId,
                      String title, String titleImage, String summary, String description,
                      int imageCount, int[] reactionCounts, int commentCount, int favorite, int like, int likeCount, long reactionPhraseId,
                      DateTime publicationDateTime, int postStatus,
                      AppUserInfo appUserInfo, ContactFull contactFull, List<LinkFull> linkFulls, List<CommentFull> commentFulls,
                      long memoryTypeId, long countryId, long stateId,
                      DateTime? dateTime,
                      String location,
                      int status,
                      String[] images)
        : base(postId, appUserId, appUserAlias, postSubtypeId,
               countryId, stateId, title, titleImage, summary, description,
               imageCount, reactionCounts, commentCount, favorite, like, likeCount, reactionPhraseId, publicationDateTime, postStatus,
               appUserInfo, contactFull, linkFulls, commentFulls)
    {
        Id = id;
        MemoryTypeId = memoryTypeId;
        CountryId = countryId;
        StateId = stateId;
        DateTime = dateTime;
        Location = location;
        Status = status;
        Images = images;
    }
}

