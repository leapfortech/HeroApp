using System;
using System.Collections.Generic;

public class RegisterRadioRequest : RegisterPostRequest
{
    public Radio Radio { get; set; }
    public List<RadioType> RadioTypes { get; set; }
    public List<RadioLanguage> RadioLanguages { get; set; }

    public RegisterRadioRequest()
    {
    }

    public RegisterRadioRequest(Post post, List<Link> links, String[] images,
                                Radio radio, List<RadioType> radioTypes, List<RadioLanguage> radioLanguages)
    {
        Post = post;
        Links = links;
        Images = images;

        Radio = radio;
        RadioTypes = radioTypes;
        RadioLanguages = radioLanguages;
    }
}
