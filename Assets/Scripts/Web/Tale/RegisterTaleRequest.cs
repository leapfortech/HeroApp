public class RegisterTaleRequest : RegisterPostRequest
{
    public Tale Tale { get; set; }

    public RegisterTaleRequest()
    {
    }

    public RegisterTaleRequest(Tale tale)
    {
        Tale = tale;
    }

    public RegisterTaleRequest(RegisterPostRequest registerPostRequest)
    {
        Post = registerPostRequest.Post;
        Contact = registerPostRequest.Contact;
        Links = registerPostRequest.Links;
        Images = registerPostRequest.Images;

        Tale = null;
    }
}
