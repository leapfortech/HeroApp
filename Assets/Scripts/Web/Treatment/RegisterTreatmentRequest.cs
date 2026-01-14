using System.Collections.Generic;

public class RegisterTreatmentRequest : RegisterPostRequest
{
    public Treatment Treatment { get; set; }
    public List<Disease> Diseases { get; set; }

    public RegisterTreatmentRequest()
    {
    }

    public RegisterTreatmentRequest(Treatment treatment, List<Disease> diseases)
    {
        Treatment = treatment;
        Diseases = diseases;
    }

    public RegisterTreatmentRequest(RegisterPostRequest registerPostRequest, Treatment treatment, List<Disease> diseases)
    {
        Post = registerPostRequest.Post;
        Contact = registerPostRequest.Contact;
        Links = registerPostRequest.Links;
        Images = registerPostRequest.Images;

        Treatment = treatment;
        Diseases = diseases;
    }
}
