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
    public long id;

    [HttpResponseJsonBody]
    public AppUser appUser;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/appUser/Portrait")]
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
[HttpPathExt(WebServiceType.Main, "/appUser/ValidateAlias")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class ValidateAliasPostOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public AliasRequest aliasRequest;

    [HttpResponseJsonBody]
    public AliasResponse aliasResponse;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/appUser/RegisterLocality")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class LocalityPostOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public LocalityRequest localityRequest;

    [HttpResponseJsonBody]
    public LocalityResponse localityResponse;
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
[HttpPathExt(WebServiceType.Main, "/appUser/UpdateOptions")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class AppUserOptionsPutOperation : HttpOperation
{
    [HttpQueryString]
    public long id;

    [HttpQueryString]
    public long options;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/appUser/UpdateStatus")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class AppUserStatusPutOperation : HttpOperation
{
    [HttpQueryString]
    public long id;

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
    public long id;

    [HttpQueryString]
    public String referredCode;

    [HttpResponseTextBody]
    public String referredAppUserId;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/appUser/Portrait")]
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

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/appUser/UpdateLocality")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class LocalityPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Locality locality;

    [HttpResponseTextBody]
    public String localityId;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/appUser/UpdateAlias")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class AliasPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public AliasRequest aliasRequest;
}

[HttpDELETE]
[HttpPathExt(WebServiceType.Main, "/appUser/Portrait")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpFirebaseAuthorization]
public class PortraitDeleteOperation : HttpOperation
{
    [HttpQueryString]
    public long appUserId;
}
