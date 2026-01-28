using hg.ApiWebKit.core.http;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.providers;
using hg.ApiWebKit.mappers;
using hg.ApiWebKit.authorizations;

using Leap.Data.Web;
using Leap.Finance.Payment;

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/card/ByAppUserId")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class CardCustomerOperation : HttpOperation
{
    [HttpQueryString("appUserId")]
    public long appUserId;

    [HttpResponseJsonBody]
    public Card card;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/card/BillTo")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class BillToOperation : HttpOperation
{
    [HttpQueryString("appUserId")]
    public long appUserId;

    [HttpResponseJsonBody]
    public CSBillToCreate billToCreate;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/card")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class CardRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public CardRegister cardRegister;

    [HttpResponseJsonBody]
    public Card card;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/card/SetStatus")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpFirebaseAuthorization]
public class CardSetStatusOperation : HttpOperation
{
    [HttpQueryString("id")]
    public long id;

    [HttpQueryString("status")]
    public int status;
}
