using System;
using System.Collections.Generic;

public class RecipeFull : PostFull
{
    public long Id { get; set; }
    public long RecipeTypeId { get; set; }
    public String Ingredients { get; set; }
    public String Preparation { get; set; }
    public int Portions { get; set; }
    public int CookingTime { get; set; }
    public int Status { get; set; }
    public String[] Images { get; set; }

    public RecipeFull()
    {
    }

    public RecipeFull(long id, long postId, long appUserId, String appUserAlias,
                        long postSubtypeId, long postCountryId, long postStateId,
                        String title, String titleImage, String summary, String description,
                        int imageCount, int favorite, int like, int likeCount, long reactionPhraseId,
                        DateTime publicationDateTime, int postStatus,
                        ContactFull contactFull, List<LinkFull> linkFulls, List<CommentFull> commentFulls,
                        long recipeTypeId, String ingredients, String preparation,
                        int portions, int cookingTime, int status,
                        String[] images)
        : base(postId, appUserId, appUserAlias, postSubtypeId,
                postCountryId, postStateId, title, titleImage, summary, description,
                imageCount, favorite, like, likeCount, reactionPhraseId, publicationDateTime, postStatus,
                contactFull, linkFulls, commentFulls)
    {
        Id = id;
        RecipeTypeId = recipeTypeId;
        Ingredients = ingredients;
        Preparation = preparation;
        Portions = portions;
        CookingTime = cookingTime;
        Status = status;
        Images = images;
    }
}
