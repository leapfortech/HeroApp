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
[HttpPathExt(WebServiceType.Main, "/radio/FullsByStatus")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class RadioGetFullsOperation : HttpOperation
{
    [HttpQueryString]
    public int status;

    [HttpResponseJsonBody]
    public List<RadioFull> radioFulls;
}

// REGISTER
[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/radio/register")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class RadioRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public RegisterRadioRequest registerRadioRequest;

    [HttpResponseTextBody]
    public String id;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/radio/RegisterRadioListen")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class RadioListenRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public RadioListen radioListen;

    [HttpResponseTextBody]
    public String radioListenId;
}

//UPDATE
[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/radio")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class RadioPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public RegisterRadioRequest registerRadioRequest;

    [HttpResponseJsonBody]
    public bool response;
}
