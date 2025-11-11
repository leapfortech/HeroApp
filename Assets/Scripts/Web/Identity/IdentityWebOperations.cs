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
    public int id;

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
    public int appUserId;
    [HttpQueryString]
    public int status;

    [HttpResponseJsonBody]
    public Identity identity;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/identity/InfoByAppUserId")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class IdentityInfoAppUserGetOperation : HttpOperation
{
    [HttpQueryString]
    public int appUserId;
    [HttpQueryString]
    public int status;

    [HttpResponseJsonBody]
    public IdentityInfo identityInfo;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/identity/DpiPhotoByAppUserId")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class DpiPhotoAppUserGetOperation : HttpOperation
{
    [HttpQueryString]
    public int appUserId;


    [HttpResponseJsonBody]
    public DpiPhoto dpiPhoto;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/identity/PortraitByAppUserId")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class PortraitAppUserGetOperation : HttpOperation
{
    [HttpQueryString]
    public int appUserId;

    [HttpResponseTextBody]
    public String portrait;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/identity/Signature")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class SignatureGetOperation : HttpOperation
{
    [HttpQueryString]
    public int signatureId;

    [HttpResponseTextBody]
    public String strokes;
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
[HttpPathExt(WebServiceType.Main, "/identity/DpiFront")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpFirebaseAuthorization]
public class DpiFrontPutOperation : HttpOperation
{
    [HttpQueryString]
    public int appUserId;

    [HttpRequestTextBody]
    public String dpiPhotos;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/identity/DpiBack")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpFirebaseAuthorization]
public class DpiBackPutOperation : HttpOperation
{
    [HttpQueryString]
    public int appUserId;

    [HttpRequestTextBody]
    public String dpiBack;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/identity/Info")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class IdentityInfoPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public IdentityInfo identityFull;

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
    public int appUserId;

    [HttpRequestTextBody]
    public String portrait;
}