using System;

public class RegisterMemoryRequest : RegisterPostRequest
{
    public Memory Memory { get; set; }

    public RegisterMemoryRequest()
    {
    }

    public RegisterMemoryRequest(Memory memory)
    {
        Memory = memory;
    }

    public RegisterMemoryRequest(Post post, String[] images, Memory memory)
    {
        Post = post;

        Images = images;

        Memory = memory;
    }
}
