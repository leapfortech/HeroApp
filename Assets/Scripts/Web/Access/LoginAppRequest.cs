using System;

public class LoginAppRequest
{
    public String Email { get; set; }
    public String Version { get; set; }

    public LoginAppRequest()
    {
    }

    public LoginAppRequest(String email, String version)
    {
        Email = email;
        Version = version;
    }
}
