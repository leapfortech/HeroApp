using System.Collections.Generic;

public class RegisterRadioRequest : RegisterPostRequest
{
    public Radio Radio { get; set; }
    public List<RadioType> RadioTypes { get; set; }
    public List<RadioLanguage> RadioLanguages { get; set; }

    public RegisterRadioRequest()
    {
    }

    public RegisterRadioRequest(RegisterPostRequest registerPostRequest,
                                List<RadioType> radioTypes,
                                List<RadioLanguage> radioLanguages)
    {
        Post = registerPostRequest.Post;
        Contact = registerPostRequest.Contact;
        Links = registerPostRequest.Links;
        Images = registerPostRequest.Images;

        Radio = null;
        RadioTypes = radioTypes;
        RadioLanguages = radioLanguages;
    }

    public RegisterRadioRequest(RegisterPostRequest registerPostRequest,
                                Radio radio,
                                List<RadioType> radioTypes,
                                List<RadioLanguage> radioLanguages)
    {
        Post = registerPostRequest.Post;
        Contact = registerPostRequest.Contact;
        Links = registerPostRequest.Links;
        Images = registerPostRequest.Images;

        Radio = radio;
        RadioTypes = radioTypes;
        RadioLanguages = radioLanguages;
    }
}
