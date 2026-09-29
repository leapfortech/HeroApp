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


    public RecipeFull()
    {
    }

    public RecipeFull(long id, long postId, long appUserId, String appUserAlias,
                        long postSubtypeId, long postCountryId, long postStateId,
                        String title, String titleImage, String description,
                        int imageCount, int favoriteCount, int[] reactionCounts, long reactionPhraseId, int commentCount,
                        DateTime publicationDateTime, int postStatus,
                        AppUserInfo appUserInfo, ContactFull contactFull, List<LinkFull> linkFulls, List<CommentFull> commentFulls, String[] images,
                        long recipeTypeId, String ingredients, String preparation,
                        int portions, int cookingTime, int status)
        : base(postId, appUserId, appUserAlias, postSubtypeId, postCountryId, postStateId, title, titleImage, description,
               imageCount, favoriteCount, reactionCounts, reactionPhraseId, commentCount, publicationDateTime, postStatus,
               appUserInfo, contactFull, linkFulls, commentFulls, images)
    {
        Id = id;
        RecipeTypeId = recipeTypeId;
        Ingredients = ingredients;
        Preparation = preparation;
        Portions = portions;
        CookingTime = cookingTime;
        Status = status;
    }
}
