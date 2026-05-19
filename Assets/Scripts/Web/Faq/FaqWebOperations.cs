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
[HttpPathExt(WebServiceType.Main, "/faq/ById")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class FaqGetOperation : HttpOperation
{
    [HttpQueryString]
    public long id;

    [HttpResponseJsonBody]
    public Faq faq;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/faq/AllByType")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class FaqAllByTypeGetOperation : HttpOperation
{
    [HttpQueryString]
    public long faqTypeId;

    [HttpResponseJsonBody]
    public List<Faq> faqs;
}

// REGISTER
[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/faq/register")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class FaqRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Faq faq;

    [HttpResponseTextBody]
    public String id;
}

// UPDATE
[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/faq")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class FaqPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Faq faq;

    [HttpResponseJsonBody]
    public long id;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/faq/UpdateStatus")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class UpdateStatusPutOperation : HttpOperation
{
    [HttpQueryString]
    public long id;
    [HttpQueryString]
    public int status;

    [HttpResponseTextBody]
    public bool response;
}