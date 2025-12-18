using System;
using System.Collections.Generic;

using hg.ApiWebKit.core.http;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.providers;
using hg.ApiWebKit.mappers;
using hg.ApiWebKit.authorizations;

using Leap.Data.Web;

// GET
[HttpGET]
[HttpPathExt(WebServiceType.Main, "/happening/FullsByStatus")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class HappeningGetFullsOperation : HttpOperation
{
    [HttpQueryString]
    public int status;

    [HttpResponseJsonBody]
    public List<HappeningFull> happeningFulls;
}

// REGISTER
[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/happening/register")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class HappeningRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public RegisterHappeningRequest registerHappeningRequest;

    [HttpResponseTextBody]
    public String id;
}

//UPDATE
[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/happening")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class HappeningPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Happening happening;

    [HttpResponseJsonBody]
    public bool response;
}
