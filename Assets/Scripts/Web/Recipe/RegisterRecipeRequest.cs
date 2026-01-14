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

    public RegisterRecipeRequest(RegisterPostRequest registerPostRequest, Recipe recipe)
    {
        Post = registerPostRequest.Post;
        Contact = registerPostRequest.Contact;
        Links = registerPostRequest.Links;
        Images = registerPostRequest.Images;

        Recipe = recipe;
    }
}
