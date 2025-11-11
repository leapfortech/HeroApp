using System;

using hg.ApiWebKit.core.http;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.providers;
using hg.ApiWebKit.mappers;
using hg.ApiWebKit.authorizations;

using Leap.Data.Web;

// GET
[HttpGET]
[HttpPathExt(WebServiceType.Main, "/investment/FullsByStatus")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class FullsByStatusGetOperation : HttpOperation
{
    [HttpQueryString]
    public int status;

    [HttpResponseJsonBody]
    public InvestmentResponse investmentResponse;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/investment/FullsByAppUserId")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class FullsByAppUserIdGetOperation : HttpOperation
{
    [HttpQueryString]
    public int appUserId;

    [HttpResponseJsonBody]
    public InvestmentResponse investmentResponse;
}

// Fractionated
[HttpGET]
[HttpPathExt(WebServiceType.Main, "/investment/FractionatedFullById")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class FractionatedFullByIdGetOperation : HttpOperation
{
    [HttpQueryString]
    public int id;

    [HttpResponseJsonBody]
    public InvestmentFractionatedFull investmentFractionatedFull;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/investment/FractionatedFullsByStatus")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class FractionatedFullsByStatusGetOperation : HttpOperation
{
    [HttpQueryString]
    public int status;

    [HttpResponseJsonBody]
    public InvestmentFractionatedFull[] investmentFractionatedFulls;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/investment/FractionatedFullsByAppUserId")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class FractionatedFullsByAppUserIdGetOperation : HttpOperation
{
    [HttpQueryString]
    public int appUserId;

    [HttpQueryString]
    public int status;

    [HttpResponseJsonBody]
    public InvestmentFractionatedFull[] investmentIntallmentFulls;
}

// Financed
[HttpGET]
[HttpPathExt(WebServiceType.Main, "/investment/FinancedFullById")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class FinancedFullByIdGetOperation : HttpOperation
{
    [HttpQueryString]
    public int id;

    [HttpResponseJsonBody]
    public InvestmentFinancedFull investmentFinancedFull;
}


[HttpGET]
[HttpPathExt(WebServiceType.Main, "/investment/FinancedFullsByStatus")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class FinancedFullsByStatusGetOperation : HttpOperation
{
    [HttpQueryString]
    public int status;

    [HttpResponseJsonBody]
    public InvestmentFinancedFull[] investmentFinancedFulls;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/investment/FinancedFullsByAppUserId")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class FinancedFullsByAppUserIdGetOperation : HttpOperation
{
    [HttpQueryString]
    public int appUserId;

    [HttpQueryString]
    public int status;

    [HttpResponseJsonBody]
    public InvestmentFinancedFull[] investmentFinancedFulls;
}

// Prepaid
[HttpGET]
[HttpPathExt(WebServiceType.Main, "/investment/PrepaidFullById")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class PrepaidFullByIdGetOperation : HttpOperation
{
    [HttpQueryString]
    public int id;

    [HttpResponseJsonBody]
    public InvestmentPrepaidFull investmentPrepaidFull;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/investment/PrepaidFullsByStatus")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class PrepaidFullsByStatusGetOperation : HttpOperation
{
    [HttpQueryString]
    public int status;

    [HttpResponseJsonBody]
    public InvestmentPrepaidFull[] investmentPrepaidFulls;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/investment/PrepaidFullsByAppUserId")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class PrepaidFullsByAppUserIdGetOperation : HttpOperation
{
    [HttpQueryString]
    public int appUserId;

    [HttpQueryString]
    public int status;

    [HttpResponseJsonBody]
    public InvestmentPrepaidFull[] investmentPrepaidFulls;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/investment/TransactionReceipt")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class TransactionReceiptGetOperation : HttpOperation
{
    [HttpQueryString]
    public int transactionId;

    [HttpResponseTextBody]
    public String receipt;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/investment/RegisterFractionated")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class FractionatedRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Investment investment;

    [HttpResponseJsonBody]
    public InvestmentFractionatedFull investmentFractionatedFull;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/investment/RegisterFinanced")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class FinancedRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Investment investment;

    [HttpResponseJsonBody]
    public InvestmentFinancedFull investmentFinancedFull;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/investment/RegisterPrepaid")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class PrepaidRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Investment investment;

    [HttpResponseJsonBody]
    public InvestmentPrepaidFull investmentPrepaidFull;
}

// PAYMENT

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/investment/PaymentBank")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class InvestmentPaymentBankPostOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public InvestmentBankPayment request;

    [HttpResponseJsonBody]
    public InvestmentBankPayment investmentBankPayment;
}

//[HttpPOST]
//[HttpPathExt(WebServiceType.Main, "/investment/PaymentCard")]
//[HttpProvider(typeof(HttpUnityWebAzureClient))]
//[HttpContentType("application/json")]
//[HttpAccept("application/json")]
//[HttpFirebaseAuthorization]
//public class InvestmentPaymentCardPostOperation : HttpOperation
//{
//    [HttpRequestJsonBody]
//    public InvestmentCardPayment request;

//    [HttpResponseJsonBody]
//    public InvestmentPayment investmentPayment;
//}

// Docs Register

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/investment/RegisterDocRtu")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class DocRTURegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public InvestmentDocRequest investmentDocRequest;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/investment/RegisterDocBank")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class DocBankRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public InvestmentDocRequest investmentDocRequest;
}

// REFERENCES

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/investment/RegisterReference")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class ReferencesRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public InvestmentReference[] investmentReferences;

    [HttpResponseJsonBody]
    public int[] ids;
}

// SIGNATORY

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/investment/RegisterSignatory")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class SignatoryRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public InvestmentIdentityRequest[] investmentIdentityRequests;

    [HttpResponseJsonBody]
    public int[] ids;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/investment/CreateSignatory")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class SignatoryCreateOperation : HttpOperation
{
    [HttpQueryString]
    public int investmentId;
}

// BENEFICIARY

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/investment/RegisterBeneficiary")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class BeneficiaryRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public InvestmentIdentityRequest[] investmentIdentityRequests;

    [HttpResponseJsonBody]
    public int[] ids;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/investment/CreateBeneficiary")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class BeneficiaryCreateOperation : HttpOperation
{
    [HttpQueryString]
    public int investmentId;
}


// Docs Update

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/investment/UpdateDocRtu")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class DocRTUUpdateOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public InvestmentDocRequest investmentDocRequest;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/investment/UpdateDocBank")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class DocBankUpdateOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public InvestmentDocRequest investmentDocRequest;
}

// REFERENCES

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/investment/UpdateReference")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class ReferencesUpdateOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public InvestmentReference[] investmentReferences;

    [HttpResponseJsonBody]
    public int[] ids;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/investment/UpdateSignatory")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class SignatoryUpdateOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public InvestmentIdentityRequest[] investmentIdentityRequests;

    [HttpResponseJsonBody]
    public int[] ids;
}

// BENEFICIARY

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/investment/UpdateBeneficiary")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class BeneficiaryUpdateOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public InvestmentIdentityRequest[] investmentIdentityRequests;

    [HttpResponseJsonBody]
    public int[] ids;
}

