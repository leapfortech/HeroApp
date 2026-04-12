using System;

public class Phone
{
    public long PhoneCountryId { get; set; }
    public String PhoneNumber { get; set; }

    public Phone()
    {

    }

    public Phone(long phoneCountryId, String phoneNumber)
    {
        PhoneCountryId = phoneCountryId;
        PhoneNumber = phoneNumber;
    }
}
