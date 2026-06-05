using System;
using System.Collections.Generic;

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

    public RegisterNewsRequest(Post post, Link link, String[] images, News news)
    {
        Post = post;
        Links = new List<Link>() { link };
        Images = images;

        News = news;
    }

    public RegisterNewsRequest(Post post, List<Link> links, String[] images, News news)
    {
        Post = post;
        Links = links;
        Images = images;

        News = news;
    }
}
