using System;

using hg.ApiWebKit.core.http;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.providers;
using hg.ApiWebKit.mappers;
using hg.ApiWebKit.authorizations;

using Leap.Data.Web;

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/precheck/RegisterPhoneSms")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class RegisterPhoneSmsPostOperation : HttpOperation
{
    [HttpQueryString]
    public long phoneCountryId;

    [HttpQueryString]
    public String phoneNumber;

    [HttpResponseTextBody]
    public String result;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/precheck/ValidatePhoneCodeSms")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class ValidatePhoneCodeSmsPostOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PhoneCodeRequest phoneCodeRequest;

    [HttpResponseTextBody]
    public String result;
}