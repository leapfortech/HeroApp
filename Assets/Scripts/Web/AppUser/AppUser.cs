using System;

using Sirenix.OdinInspector;

public class AppUser
{
    [ShowInInspector]
    public long Id { get; set; }
    public long WebSysUserId { get; set; }
    public String Alias { get; set; }
    public String CSToken { get; set; }
    public long Options { get; set; } = 11;
    public long ReferrerAppUserId { get; set; }
    public int AppUserStatusId { get; set; }


    public AppUser()
    {
    }

    public AppUser(long id, long webSysUserId, String alias, String csToken, long options, long referrerAppUserId,
                   int appUserStatusId)
    {
        Id = id;
        WebSysUserId = webSysUserId;
        Alias = alias;
        CSToken = csToken;
        Options = options;
        ReferrerAppUserId = referrerAppUserId;
        AppUserStatusId = appUserStatusId;
    }
}
