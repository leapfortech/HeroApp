using System;
using System.Collections.Generic;

public class RegisterPostRequest
{
    public Post Post { get; set; }
    public Contact Contact { get; set; }
    public List<Link> Links { get; set; }

    public String[] Images { get; set; }

    public RegisterPostRequest()
    {
    }

    public RegisterPostRequest(Post post, Contact contact, List<Link> links, String[] images)
    {
        Post = post;
        Contact = contact;
        Links = links;

        Images = images;
    }
}
