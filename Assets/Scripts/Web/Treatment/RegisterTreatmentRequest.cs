using System;
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

    public RegisterTreatmentRequest(Post post, String[] images, Treatment treatment, List<Disease> diseases)
    {
        Post = post;
        Images = images;

        Treatment = treatment;
        Diseases = diseases;
    }
}
