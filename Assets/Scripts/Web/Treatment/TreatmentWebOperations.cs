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
[HttpPathExt(WebServiceType.Main, "/treatment")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class TreatmentGetFullOperation : HttpOperation
{
    [HttpQueryString]
    public long id;

    [HttpResponseJsonBody]
    public TreatmentFull treatmentFull;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/treatment/FullByPostId")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class TreatmentFullByPostIdGetFullOperation : HttpOperation
{
    [HttpQueryString]
    public long postId;

    [HttpResponseJsonBody]
    public TreatmentFull treatmentFull;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/treatment/FullsByStatus")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class TreatmentGetFullsOperation : HttpOperation
{
    [HttpQueryString]
    public int status;

    [HttpResponseJsonBody]
    public List<TreatmentFull> treatmentFulls;
}

// REGISTER
[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/treatment/register")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class TreatmentRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public RegisterTreatmentRequest registerTreatmentRequest;

    [HttpResponseTextBody]
    public String id;
}

//UPDATE
[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/treatment")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class TreatmentPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public RegisterTreatmentRequest registerTreatmentRequest;

    [HttpResponseTextBody]
    public bool response;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/treatment/Accept")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class TreatmentAcceptPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PostModerationRequest postModerationRequest;

    [HttpResponseTextBody]
    public bool response;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/treatment/Reject")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class TreatmentRejectPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PostModerationRequest postModerationRequest;

    [HttpResponseTextBody]
    public bool response;
}

