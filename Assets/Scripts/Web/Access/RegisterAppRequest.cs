using System;

public class RegisterAppRequest
{
    public String Alias { get; set; }
    public String Email { get; set; }
    public String Password { get; set; }
    public long PhoneCountryId { get; set; }
    public String Phone { get; set; }
    public String ReferredCode { get; set; }
    public Identity Identity { get; set; }
    public Address Address { get; set; }


    public RegisterAppRequest()
    {
    }

    public RegisterAppRequest(String alias, String email, String password, long phoneCountryId,
                              String phone, String referredCode, Identity identity, Address address)
    {
        Alias = alias;
        Email = email;
        Password = password;
        PhoneCountryId = phoneCountryId;
        Phone = phone;
        ReferredCode = referredCode;
        Identity = identity;
        Address = address;
    }
}
