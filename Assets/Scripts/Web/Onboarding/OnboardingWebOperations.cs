using System;

using hg.ApiWebKit.core.http;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.providers;
using hg.ApiWebKit.mappers;
using hg.ApiWebKit.authorizations;

using Leap.Data.Web;


[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/onboarding/DpiFront")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpFirebaseAuthorization]
public class ObdDpiFrontPutOperation : HttpOperation
{
    [HttpQueryString]
    public int appUserId;

    [HttpRequestTextBody]
    public String dpiPhotos;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/onboarding/DpiBack")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpFirebaseAuthorization]
public class ObdDpiBackPutOperation : HttpOperation
{
    [HttpQueryString]
    public int appUserId;

    [HttpRequestTextBody]
    public String dpiBack;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/onboarding/IdentityInfo")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class ObdIdentityInfoPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public IdentityInfo identityInfo;

    [HttpResponseTextBody]
    public String id;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/onboarding/Portrait")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpFirebaseAuthorization]
public class ObdPortraitPutOperation : HttpOperation
{
    [HttpQueryString]
    public int appUserId;

    [HttpRequestTextBody]
    public String portrait;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/onboarding/HouseholdBills")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpFirebaseAuthorization]
public class ObdHouseholdBillPutOperation : HttpOperation
{
    [HttpQueryString]
    public int appUserId;

    [HttpRequestJsonBody]
    public String[] householdBills;
}