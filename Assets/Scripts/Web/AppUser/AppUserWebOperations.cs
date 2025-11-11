using System;

using hg.ApiWebKit.core.http;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.providers;
using hg.ApiWebKit.mappers;
using hg.ApiWebKit.authorizations;

using Leap.Data.Web;

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/appUser/ById")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class AppUserGetOperation : HttpOperation
{
    [HttpQueryString]
    public int id;

    [HttpResponseJsonBody]
    public AppUser appUser;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/appUser")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class AppUserPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public AppUser appUser;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/appUser/UpdatePhone")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class AppUserPhonePutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PhoneRequest phoneRequest;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/appUser/UpdateOptions")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class AppUserOptionsPutOperation : HttpOperation
{
    [HttpQueryString]
    public int id;

    [HttpQueryString]
    public int options;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/appUser/UpdateStatus")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class AppUserStatusPutOperation : HttpOperation
{
    [HttpQueryString]
    public int id;

    [HttpQueryString]
    public int status;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/appUser/UpdateReferred")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class AppUserReferredPutOperation : HttpOperation
{
    [HttpQueryString]
    public int id;

    [HttpQueryString]
    public String referredCode;

    [HttpResponseTextBody]
    public String referredAppUserId;
}
