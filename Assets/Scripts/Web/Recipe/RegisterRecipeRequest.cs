using System;

public class RegisterRecipeRequest : RegisterPostRequest
{
    public Recipe Recipe { get; set; }

    public RegisterRecipeRequest()
    {
    }

    public RegisterRecipeRequest(Recipe recipe)
    {
        Recipe = recipe;
    }

    public RegisterRecipeRequest(Post post, String[] images, Recipe recipe)
    {
        Post = post;
        Images = images;

        Recipe = recipe;
    }
}
