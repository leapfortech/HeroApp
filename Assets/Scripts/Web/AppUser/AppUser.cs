using System;

using Sirenix.OdinInspector;

public class AppUser
{
    [ShowInInspector]
    public int Id { get; set; }
    [ShowInInspector]
    public String Code { get; set; }
    public int WebSysUserId { get; set; }
    public String CSToken { get; set; }
    public int Options { get; set; } = 11;
    public int ReferrerAppUserId { get; set; }
    public int AppUserStatusId { get; set; }


    public AppUser()
    {
    }

    public AppUser(int id, String code, int webSysUserId, String phone, String csToken, int options, int referrerAppUserId, int appUserStatusId)
    {
        Id = id;
        Code = code;
        WebSysUserId = webSysUserId;
        CSToken = csToken;
        Options = options;
        ReferrerAppUserId = referrerAppUserId;
        AppUserStatusId = appUserStatusId;
    }
}
