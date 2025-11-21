using System;

using hg.ApiWebKit.core.http;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.providers;
using hg.ApiWebKit.mappers;
using hg.ApiWebKit.authorizations;

using Leap.Data.Web;

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/phone/RegisterPhone")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class RegisterPhonePostOperation : HttpOperation
{
    [HttpQueryString]
    public long phoneCountryId;

    [HttpQueryString]
    public String phoneNumber;

    [HttpResponseTextBody]
    public String result;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/phone/ValidateCode")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class ValidateCodePostOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PhoneCodeRequest phoneCodeRequest;

    [HttpResponseTextBody]
    public String result;
}