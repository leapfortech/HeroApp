using System;

public class RegisterRequest
{
    public String Email { get; set; }
    public String Password { get; set; }
    public int PhoneCountryId { get; set; }
    public String Phone { get; set; }
    public int News { get; set; }
    public String ReferredCode { get; set; }


    public RegisterRequest()
    {
    }

    public RegisterRequest(String email, String password, int phoneCountryId, String phone, int news, String referredCode)
    {
        Email = email;
        Password = password;
        PhoneCountryId = phoneCountryId;
        Phone = phone;
        News = news;
        ReferredCode = referredCode;
    }
}
