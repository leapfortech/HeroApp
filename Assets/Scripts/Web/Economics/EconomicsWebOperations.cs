using System.Collections.Generic;

using hg.ApiWebKit.core.http;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.providers;
using hg.ApiWebKit.mappers;
using hg.ApiWebKit.authorizations;

using Leap.Data.Web;
using System;


[HttpGET]
[HttpPathExt(WebServiceType.Main, "/economics/InfoByInvestmentId")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class EconomicsInfoGetOperation : HttpOperation
{
    [HttpQueryString]
    public int investmentId;

    [HttpResponseJsonBody]
    public EconomicsInfo economicsInfo;
}


// REGISTER
[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/economics/Register")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class EconomicsRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public EconomicsInfo economicsInfo;

    [HttpResponseJsonBody]
    public int[] infoIds;
}

// ADD
[HttpPOST]
[HttpPathExt(WebServiceType.Main, "economics/income")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class IncomePostOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Income income;

    [HttpResponseTextBody]
    public String id;
}

// UPDATE
[HttpPUT]
[HttpPathExt(WebServiceType.Main, "economics/Update")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class EconomicsUpdateOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public EconomicsInfo economicsInfo;

    [HttpResponseJsonBody]
    public int[] infoIds;
}
