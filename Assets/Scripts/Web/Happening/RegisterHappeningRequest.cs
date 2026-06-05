using System;

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

    public RegisterHappeningRequest(Post post, String[] images, Happening happening)
    {
        Post = post;
        Images = images;

        Happening = happening;
    }
}
