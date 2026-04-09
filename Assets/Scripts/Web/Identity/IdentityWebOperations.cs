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

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/identity/Register")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class IdentityRegisterPostOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Identity identity;

    [HttpResponseTextBody]
    public String id;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/identity/Personal")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class PersonalPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public IdentityPersonal identityPersonal;

    [HttpResponseTextBody]
    public String id;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/identity/Place")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class PlacePutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public IdentityPlace identityPlace;

    [HttpResponseTextBody]
    public String id;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/identity/ByAppUser")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class IdentityPutOperation : HttpOperation
{
    [HttpQueryString]
    public long appUserId;
    [HttpRequestJsonBody]
    public Identity identity;

    [HttpResponseTextBody]
    public String id;
}