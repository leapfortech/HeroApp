using System;
using System.Collections.Generic;

public class RegisterProductRequest : RegisterPostRequest
{
    public Product Product { get; set; }

    public RegisterProductRequest()
    {
    }

    public RegisterProductRequest(Product product)
    {
        Product = product;
    }

    public RegisterProductRequest(Post post, Contact contact, Link link, String[] images, Product product)
    {
        Post = post;
        Contact = contact;
        Links = new List<Link>() { link };
        Images = images;

        Product = product;
    }

    public RegisterProductRequest(Post post, Contact contact, List<Link> links, String[] images, Product product)
    {
        Post = post;
        Contact = contact;
        Links = links;
        Images = images;

        Product = product;
    }
}
