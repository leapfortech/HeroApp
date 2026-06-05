using System;

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

    public RegisterTaleRequest(Post post, String[] images, Tale tale)
    {
        Post = post;
        Images = images;

        Tale = tale;
    }
}
