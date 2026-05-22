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
[HttpPathExt(WebServiceType.Main, "/tale")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class TaleGetFullOperation : HttpOperation
{
    [HttpQueryString]
    public long id;
    [HttpQueryString]
    public long likeAppUserId;

    [HttpResponseJsonBody]
    public TaleFull taleFull;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/tale/FullByPostId")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class TaleFullByPostIdGetFullOperation : HttpOperation
{
    [HttpQueryString]
    public long postId;
    [HttpQueryString]
    public long likeAppUserId;

    [HttpResponseJsonBody]
    public TaleFull taleFull;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/tale/FullsByStatus")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class TaleGetFullsOperation : HttpOperation
{
    [HttpQueryString]
    public int status;

    [HttpResponseJsonBody]
    public List<TaleFull> taleFulls;
}

// REGISTER
[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/tale/register")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class TaleRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public RegisterTaleRequest registerTaleRequest;

    [HttpResponseTextBody]
    public String id;
}

//UPDATE
[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/tale")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class TalePutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public RegisterTaleRequest registerTaleRequest;

    [HttpResponseTextBody]
    public bool response;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/tale/Accept")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class TaleAcceptPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PostModerationRequest postModerationRequest;

    [HttpResponseTextBody]
    public bool response;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/tale/Reject")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class TaleRejectPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PostModerationRequest postModerationRequest;

    [HttpResponseTextBody]
    public bool response;
}
