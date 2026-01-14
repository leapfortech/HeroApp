public class RegisterHappeningRequest : RegisterPostRequest
{
    public Happening Happening { get; set; }

    public RegisterHappeningRequest()
    {
    }

    public RegisterHappeningRequest(Happening happening)
    {
        Happening = happening;
    }

    public RegisterHappeningRequest(RegisterPostRequest registerPostRequest, Happening happening)
    {
        Post = registerPostRequest.Post;
        Contact = registerPostRequest.Contact;
        Links = registerPostRequest.Links;
        Images = registerPostRequest.Images;

        Happening = happening;
    }
}
