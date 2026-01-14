public class RegisterNewsRequest : RegisterPostRequest
{
    public News News { get; set; }

    public RegisterNewsRequest()
    {
    }

    public RegisterNewsRequest(News news)
    {
        News = news;
    }

    public RegisterNewsRequest(RegisterPostRequest registerPostRequest, News news)
    {
        Post = registerPostRequest.Post;
        Contact = registerPostRequest.Contact;
        Links = registerPostRequest.Links;
        Images = registerPostRequest.Images;

        News = news;
    }
}
