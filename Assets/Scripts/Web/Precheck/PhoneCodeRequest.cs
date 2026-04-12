using System;

public class PhoneCodeRequest
{
    public long PhoneCountryId { get; set; }
    public String PhoneNumber { get; set; }
    public String Code { get; set; }

    public PhoneCodeRequest()
    {

    }

    public PhoneCodeRequest(long phoneCountryId, String phoneNumber, String code)
    {
        PhoneCountryId = phoneCountryId;
        PhoneNumber = phoneNumber;
        Code = code;
    }
}
