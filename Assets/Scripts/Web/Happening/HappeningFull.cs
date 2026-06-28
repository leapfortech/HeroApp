using System;
using System.Collections.Generic;

using UnityEngine;
using Leap.Graphics.Tools;

public class HappeningFull : PostFull
{
    public long Id { get; set; }
    public long HappeningTypeId { get; set; }
    public long CountryId { get; set; }
    public long StateId { get; set; }
    public int IsPublic { get; set; }
    public int HasSignup { get; set; }
    public int HasPayment { get; set; }
    public String PaymentDetails { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public String Location { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
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

    public HappeningFull()
    {
    }

    public HappeningFull(long id, long postId, long appUserId, String appUserAlias,
                            long postSubtypeId,
                            long postCountryId, long postStateId,
                            String title, String titleImage, String summary, String description,
                            int imageCount, int favorite, int like, int likeCount, long reactionPhraseId,
                            DateTime publicationDateTime, int postStatus,
                            ContactFull contactFull, List<LinkFull> linkFulls, List<CommentFull> commentFulls,
                            long happeningTypeId, long countryId, long stateId,
                            int isPublic, int hasSignup, int hasPayment, String paymentDetails,
                            DateTime? startDateTime, DateTime? endDateTime,
                            String location, double? latitude, double? longitude,
                            int status,
                            String[] images)
        : base(postId, appUserId, appUserAlias, postSubtypeId,
                countryId, stateId, title, titleImage, summary, description,
                imageCount, favorite, like, likeCount, reactionPhraseId, publicationDateTime, postStatus,
                contactFull, linkFulls, commentFulls)
    {
        Id = id;
        HappeningTypeId = happeningTypeId;
        CountryId = countryId;
        StateId = stateId;
        IsPublic = isPublic;
        HasSignup = hasSignup;
        HasPayment = hasPayment;
        PaymentDetails = paymentDetails;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        Location = location;
        Latitude = latitude;
        Longitude = longitude;
        Status = status;
        Images = images;
    }
}

