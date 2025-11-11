using System;

using hg.ApiWebKit.core.http;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.providers;
using hg.ApiWebKit.mappers;
using hg.ApiWebKit.authorizations;

using Leap.Data.Web;

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/access/LoginAppInfo")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
[HttpTimeout(50f)]
public class LoginAppInfoGetOperation : HttpOperation
{
    [HttpQueryString]
    public int appUserId;

    [HttpQueryString]
    public int webSysUserId;

    [HttpResponseJsonBody]
    public LoginAppInfo loginAppInfo;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/access/LoginApp")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class AccessLoginAppPostOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public LoginAppRequest loginRequest;

    [HttpResponseJsonBody]
    public LoginAppResponse loginResponse;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/access/RegisterApp")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class AccessRegisterAppPostOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public RegisterRequest registerRequest;

    [HttpResponseTextBody]
    public String registerResponse;
}