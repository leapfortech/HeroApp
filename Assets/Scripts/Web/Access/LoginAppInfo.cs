using System;
using System.Collections.Generic;
using Leap.Data.Web;

public class LoginAppInfo
{
    public ReferredCount ReferredCount { get; set; }
    public Identity Identity { get; set; }
    public Address Address { get; set; }
    public String Portrait { get; set; }
    public Locality InterestLocality { get; set; }
    public Locality CurrentLocality { get; set; }
    public Card Card { get; set; }
    public Notification[] Notifications { get; set; }


    public LoginAppInfo()
    {
    }

    public LoginAppInfo(ReferredCount referredCount, Identity identity, Address address, String portrait,
                        Locality interestLocality, Locality currentLocality, Card card, Notification[] notifications)
    {
        ReferredCount = referredCount;
        Identity = identity;
        Address = address;
        Portrait = portrait;
        InterestLocality = interestLocality;
        CurrentLocality = currentLocality;
        Card = card;
        Notifications = notifications;
    }
}