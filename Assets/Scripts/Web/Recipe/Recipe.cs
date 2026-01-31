using System;

public class Recipe
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public long RecipeTypeId { get; set; }
    public String Ingredients { get; set; }
    public String Preparation { get; set; }
    public int Portions { get; set; }
    public int CookingTime { get; set; }
    public int Status { get; set; }

    public Recipe() { }

    public Recipe(long id, long postId, long recipeTypeId, String ingredients, 
                  String preparation, int portions, int cookingTime, int status)
    {
        Id = id;
        PostId = postId;
        RecipeTypeId = recipeTypeId;
        Ingredients = ingredients;
        Preparation = preparation;
        Portions = portions;
        CookingTime = cookingTime;
        Status = status;
    }

    public Recipe(RecipeFull recipeFull)
    {
        Id = recipeFull.Id;
        PostId = recipeFull.PostId;
        RecipeTypeId = recipeFull.RecipeTypeId;
        Ingredients = recipeFull.Ingredients;
        Preparation = recipeFull.Preparation;
        Portions = recipeFull.Portions;
        CookingTime = recipeFull.CookingTime;
        Status = recipeFull.Status;
    }

    public void Update(Recipe recipe)
    {
        RecipeTypeId = recipe.RecipeTypeId;
        Ingredients = recipe.Ingredients;
        Preparation = recipe.Preparation;
        Portions = recipe.Portions;
        CookingTime = recipe.CookingTime;
    }
}
