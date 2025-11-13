using System;
using System.Collections.Generic;
using Leap.Data.Web;

public class LoginAppInfo
{
    public List<NewsInfo> NewsInfos { get; set; }
    public ReferredCount ReferredCount { get; set; }
    public Identity Identity { get; set; }
    public Address Address { get; set; }
    public String Portrait { get; set; }
    public Card Card { get; set; }
    public Notification[] Notifications { get; set; }

    public List<int> ProjectLikeIds { get; set; }

    public LoginAppInfo()
    {
    }

    public LoginAppInfo(List<NewsInfo> newsInfos, ReferredCount referredCount, Identity identity, Address address, String portrait, Card card, Notification[] notifications,
                        List<int> projectLikeIds)
    {
        NewsInfos = newsInfos;
        ReferredCount = referredCount;
        Identity = identity;
        Address = address;
        Portrait = portrait;
        Card = card;
        Notifications = notifications;

        ProjectLikeIds = projectLikeIds;
    }
}