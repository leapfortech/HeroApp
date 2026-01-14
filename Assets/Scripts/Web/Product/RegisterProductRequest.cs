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

    public RegisterProductRequest(RegisterPostRequest registerPostRequest, Product product)
    {
        Post = registerPostRequest.Post;
        Contact = registerPostRequest.Contact;
        Links = registerPostRequest.Links;
        Images = registerPostRequest.Images;

        Product = product;
    }
}
