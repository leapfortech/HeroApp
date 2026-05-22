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
[HttpPathExt(WebServiceType.Main, "/news")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class NewsGetFullOperation : HttpOperation
{
    [HttpQueryString]
    public long id;
    [HttpQueryString]
    public long likeAppUserId;

    [HttpResponseJsonBody]
    public NewsFull newsFull;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/news/FullByPostId")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class NewsFullByPostIdGetFullOperation : HttpOperation
{
    [HttpQueryString]
    public long postId;
    [HttpQueryString]
    public long likeAppUserId;

    [HttpResponseJsonBody]
    public NewsFull newsFull;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/news/FullsByStatus")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class NewsGetFullsOperation : HttpOperation
{
    [HttpQueryString]
    public int status;

    [HttpResponseJsonBody]
    public List<NewsFull> newsFulls;
}

// REGISTER
[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/news/register")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class NewsRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public RegisterNewsRequest registerNewsRequest;

    [HttpResponseTextBody]
    public String id;
}

//UPDATE
[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/news")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class NewsPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public RegisterNewsRequest registerNewsRequest;

    [HttpResponseTextBody]
    public bool response;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/news/Accept")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class NewsAcceptPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PostModerationRequest postModerationRequest;

    [HttpResponseTextBody]
    public bool response;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/news/Reject")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class NewsRejectPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PostModerationRequest postModerationRequest;

    [HttpResponseTextBody]
    public bool response;
}
