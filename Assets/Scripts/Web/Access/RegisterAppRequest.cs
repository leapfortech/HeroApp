using System;

public class RegisterAppRequest
{
    public String Email { get; set; }
    public String Password { get; set; }
    public long PhoneCountryId { get; set; }
    public String Phone { get; set; }
    public long ReferredId { get; set; }


    public RegisterAppRequest()
    {
    }

    public RegisterAppRequest(String email, String password, long phoneCountryId, String phone, long referredId)
    {
        Email = email;
        Password = password;
        PhoneCountryId = phoneCountryId;
        Phone = phone;
        ReferredId = referredId;
    }
}
