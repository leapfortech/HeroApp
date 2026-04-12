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
[HttpPathExt(WebServiceType.Main, "/precheck/ValidatePhoneSmsCode")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class ValidatePhoneSmsCodePostOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PhoneCodeRequest phoneCodeRequest;

    [HttpResponseTextBody]
    public String result;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/precheck/RegisterPhoneWA")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class RegisterPhoneWAPostOperation : HttpOperation
{
    [HttpQueryString]
    public long phoneCountryId;

    [HttpQueryString]
    public String phoneNumber;

    [HttpResponseTextBody]
    public String result;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/precheck/ValidatePhoneWACode")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class ValidatePhoneWACodePostOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PhoneCodeRequest phoneCodeRequest;

    [HttpResponseTextBody]
    public String result;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/precheck/RegisterEmail")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class RegisterEmailPostOperation : HttpOperation
{
    [HttpQueryString]
    public String email;

    [HttpResponseTextBody]
    public String result;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/precheck/ValidateEmailCode")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class ValidateEmailCodePostOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public EmailCodeRequest emailCodeRequest;

    [HttpResponseTextBody]
    public String result;
}