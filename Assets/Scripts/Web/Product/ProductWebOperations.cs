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
[HttpPathExt(WebServiceType.Main, "/product")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class ProductGetFullOperation : HttpOperation
{
    [HttpQueryString]
    public long id;
    [HttpQueryString]
    public long likeAppUserId;

    [HttpResponseJsonBody]
    public ProductFull productFull;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/product/FullByPostId")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class ProductFullByPostIdGetFullOperation : HttpOperation
{
    [HttpQueryString]
    public long postId;
    [HttpQueryString]
    public long likeAppUserId;

    [HttpResponseJsonBody]
    public ProductFull productFull;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/product/FullsByStatus")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class ProductGetFullsOperation : HttpOperation
{
    [HttpQueryString]
    public int status;

    [HttpResponseJsonBody]
    public List<ProductFull> productFulls;
}

// REGISTER
[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/product/register")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class ProductRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public RegisterProductRequest registerProductRequest;

    [HttpResponseTextBody]
    public String id;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/product/registerReview")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class ReviewRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public ProductReview productReview;

    [HttpResponseTextBody]
    public String id;
}

//UPDATE
[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/product")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class ProductPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public RegisterProductRequest registerProductRequest;

    [HttpResponseTextBody]
    public bool response;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/product/Accept")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class ProductAcceptPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PostModerationRequest postModerationRequest;

    [HttpResponseTextBody]
    public bool response;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/product/Reject")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class ProductRejectPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PostModerationRequest postModerationRequest;

    [HttpResponseTextBody]
    public bool response;
}

