using System;
using System.Collections.Generic;

using UnityEngine;
using Leap.Graphics.Tools;

public class TreatmentFull : PostFull
{
    public long Id { get; set; }
    public String Ingredients { get; set; }
    public String Preparation { get; set; }
    public String Usage { get; set; }
    public String Annotation { get; set; }
    public int Status { get; set; }
    public List<DiseaseFull> DiseaseFulls { get; set; }
    public String[] Images
    {
        get => null;
        set
        {
            ImageSprites = new List<Sprite>();
            for (int i = 0; i < value.Length; i++)
                if (value[i] != null)
                    ImageSprites.Add(value[i].CreateSprite("TreatmentImage_" + i.ToString("D02")));
        }
    }
    public List<Sprite> ImageSprites { get; set; }


    public TreatmentFull()
    {
    }

    public TreatmentFull(long id, long postId, long appUserId, String appUserAlias,
                            long postSubtypeId, long postCountryId, long postStateId,
                            String title, String titleImage, String summary, String description,
                            int imageCount, int favorite, int like, int likeCount, long reactionPhraseId,
                            DateTime publicationDateTime, int postStatus,
                            ContactFull contactFull, List<LinkFull> linkFulls, List<CommentFull> commentFulls,
                            String ingredients, String preparation, String usage, String annotation,
                            int status, List<DiseaseFull> diseaseFulls,
                            String[] images)
        : base(postId, appUserId, appUserAlias, postSubtypeId,
                postCountryId, postStateId, title, titleImage, summary, description,
                imageCount, favorite, like, likeCount, reactionPhraseId, publicationDateTime, postStatus,
                contactFull, linkFulls, commentFulls)
    {
        Id = id;
        Ingredients = ingredients;
        Preparation = preparation;
        Usage = usage;
        Annotation = annotation;
        Status = status;
        DiseaseFulls = diseaseFulls ?? new List<DiseaseFull>();
        Images = images;
    }
}
