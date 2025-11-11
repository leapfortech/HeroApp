using System;

using Leap.Data.Web;

public class LoginAppResponse
{
    public AppUser AppUser { get; set; }
    public WebSysUser WebSysUser { get; set; }
    public int Granted { get; set; }
    public int OnboardingStage { get; set; }
    public String Message { get; set; }
    public String Link { get; set; }

    public LoginAppResponse()
    {
    }

    public LoginAppResponse(AppUser appUser, WebSysUser webSysUser, int granted, int onboardingStage, String message, String link)
    {
        AppUser = appUser;
        WebSysUser = webSysUser;
        Granted = granted;
        OnboardingStage = onboardingStage;
        Message = message;
        Link = link;
    }
}