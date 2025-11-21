using System;

using hg.ApiWebKit.core.http;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.providers;
using hg.ApiWebKit.mappers;
using hg.ApiWebKit.authorizations;

using Leap.Data.Web;

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/identity/ById")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class IdentityGetOperation : HttpOperation
{
    [HttpQueryString]
    public long id;

    [HttpResponseJsonBody]
    public Identity identity;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/identity/ByAppUserId")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class IdentityAppUserGetOperation : HttpOperation
{
    [HttpQueryString]
    public long appUserId;
    [HttpQueryString]
    public int status;

    [HttpResponseJsonBody]
    public Identity identity;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/identity/PortraitByAppUserId")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class PortraitAppUserGetOperation : HttpOperation
{
    [HttpQueryString]
    public long appUserId;

    [HttpResponseTextBody]
    public String portrait;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/identity/Register")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class IdentityRegisterPostOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public IdentityRegister identityRegister;

    [HttpResponseTextBody]
    public String id;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/identity/Portrait")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpFirebaseAuthorization]
public class PortraitPutOperation : HttpOperation
{
    [HttpQueryString]
    public long appUserId;

    [HttpRequestTextBody]
    public String portrait;
}