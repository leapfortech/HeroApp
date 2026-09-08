using System;
using System.Collections.Generic;

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

    public RegisterHappeningRequest(Post post, Contact contact, Link link, String[] images, Happening happening)
    {
        Post = post;

        Contact = contact;
        Links = new List<Link>() { link };

        Images = images;

        Happening = happening;
    }

    public RegisterHappeningRequest(Post post, Contact contact, List<Link> links, String[] images, Happening happening)
    {
        Post = post;

        Contact = contact;
        Links = links;

        Images = images;

        Happening = happening;
    }
}
