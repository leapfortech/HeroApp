using System.Collections.Generic;

using hg.ApiWebKit.core.http;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.providers;
using hg.ApiWebKit.mappers;
using hg.ApiWebKit.authorizations;

using Leap.Data.Web;
using System;

// GET
[HttpGET]
[HttpPathExt(WebServiceType.Main, "/address/ByAppUserId")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class AddressGetOperation : HttpOperation
{
    [HttpQueryString]
    public long appUserId;

    [HttpResponseJsonBody]
    public Address address;
}

// REGISTER
[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/address/RegisterByAppUser")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class AddressAppUserRegisterOperation : HttpOperation
{
    [HttpQueryString]
    public long appUserId;

    [HttpRequestJsonBody]
    public Address address;

    [HttpResponseTextBody]
    public String id;
}

// ADD
[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/address")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class AddressPostOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Address Address;

    [HttpResponseTextBody]
    public String id;
}

// UPDATE
[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/address")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class AddressPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Address address;

    [HttpResponseTextBody]
    public String id;
}