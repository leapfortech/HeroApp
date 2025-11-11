using System;
public class PepIdentityRequest
{
    public PepIdentity PepIdentity { get; set; }
    public Identity Identity { get; set; }


    public PepIdentityRequest()
    {
    }

    public PepIdentityRequest(PepIdentity pepIdentity, Identity identity)
    {
        PepIdentity = pepIdentity;
        Identity = identity;
    }
}
