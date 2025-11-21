using System;

using Leap.Data.Web;

public class LoginAppResponse
{
    public AppUser AppUser { get; set; }
    public WebSysUser WebSysUser { get; set; }
    public int Granted { get; set; }
    public String Message { get; set; }
    public String Link { get; set; }

    public LoginAppResponse()
    {
    }

    public LoginAppResponse(AppUser appUser, WebSysUser webSysUser, int granted, String message, String link)
    {
        AppUser = appUser;
        WebSysUser = webSysUser;
        Granted = granted;
        Message = message;
        Link = link;
    }
}