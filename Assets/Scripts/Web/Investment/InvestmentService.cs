using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class InvestmentService : MonoBehaviour
{
    [Serializable]
    public class FullsEvent : UnityEvent<InvestmentResponse> { }

    [Serializable]
    public class FractionatedFullsEvent : UnityEvent<InvestmentFractionatedFull[]> { }
    [Serializable]
    public class FinancedFullsEvent : UnityEvent<InvestmentFinancedFull[]> { }
    [Serializable]
    public class PrepaidFullsEvent : UnityEvent<InvestmentPrepaidFull[]> { }
    [Serializable]
    public class FractionatedFullEvent : UnityEvent<InvestmentFractionatedFull> { }
    [Serializable]
    public class FinancedFullEvent : UnityEvent<InvestmentFinancedFull> { }
    [Serializable]
    public class PrepaidFullEvent : UnityEvent<InvestmentPrepaidFull> { }

    [Serializable]
    public class InvestmentBankPaymentEvent : UnityEvent<InvestmentBankPayment> { }

    //[Serializable]
    //public class PaymentEvent : UnityEvent<InvestmentPayment> { }


    [SerializeField]
    private FullsEvent onFullsRetreived = null;

    [SerializeField]
    private FractionatedFullEvent onFractionatedFullRetreived = null;
    [SerializeField]
    private FinancedFullEvent onFinancedFullRetreived = null;
    [SerializeField]
    private PrepaidFullEvent onPrepaidFullRetreived = null;

    [SerializeField]
    private FractionatedFullsEvent onFractionatedFullsRetreived = null;
    [SerializeField]
    private FinancedFullsEvent onFinancedFullsRetreived = null;
    [SerializeField]
    private PrepaidFullsEvent onPrepaidFullsRetreived = null;

    [SerializeField]
    private FractionatedFullEvent onFractionatedRegistered = null;
    [SerializeField]
    private FinancedFullEvent onFinancedRegistered = null;
    [SerializeField]
    private PrepaidFullEvent onPrepaidRegistered = null;

    [SerializeField]
    private UnityStringEvent onReceiptRetreived = null;

    [SerializeField]
    private InvestmentBankPaymentEvent onPaid = null;

    [SerializeField]
    private UnityEvent onDocRegistered = null;

    [SerializeField]
    private UnityIntsEvent onReferencesRegistered = null;

    [SerializeField]
    private UnityIntsEvent onIdentitiesRegistered = null;

    [SerializeField]
    private UnityEvent onIdentitiesEmptyRegistered = null;

    [SerializeField]
    private UnityEvent onDocUpdate = null;

    [SerializeField]
    private UnityIntsEvent onReferencesUpdated = null;

    [SerializeField]
    private UnityIntsEvent onIdentitiesUpdated = null;


    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;


    // GET
    public void GetFullsByStatus(int status = -1)
    {
        FullsByStatusGetOperation fullsByStatusGetOp = new FullsByStatusGetOperation();
        try
        {
            fullsByStatusGetOp.status = status;
            fullsByStatusGetOp["on-complete"] = (Action<FullsByStatusGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullsRetreived.Invoke(op.investmentResponse);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            fullsByStatusGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFullsByAppUserId(int appUserId)
    {
        FullsByAppUserIdGetOperation fullsByAppUserIdGetOp = new FullsByAppUserIdGetOperation();
        try
        {
            fullsByAppUserIdGetOp.appUserId = appUserId;
            fullsByAppUserIdGetOp["on-complete"] = (Action<FullsByAppUserIdGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullsRetreived.Invoke(op.investmentResponse);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            fullsByAppUserIdGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFractionatedFullById(int id)
    {
        FractionatedFullByIdGetOperation fractionatedFullByIdGetOp = new FractionatedFullByIdGetOperation();
        try
        {
            fractionatedFullByIdGetOp.id = id;
            fractionatedFullByIdGetOp["on-complete"] = (Action<FractionatedFullByIdGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFractionatedFullRetreived.Invoke(op.investmentFractionatedFull);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            fractionatedFullByIdGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFractionatedFullsByStatus(int status)
    {
        FractionatedFullsByStatusGetOperation fractionatedFullsByStatusGetOp = new FractionatedFullsByStatusGetOperation();
        try
        {
            fractionatedFullsByStatusGetOp.status = status;
            fractionatedFullsByStatusGetOp["on-complete"] = (Action<FractionatedFullsByStatusGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFractionatedFullsRetreived.Invoke(op.investmentFractionatedFulls);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            fractionatedFullsByStatusGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFractionatedFullsByAppUserId(int appUserId, int status = -1)
    {
        FractionatedFullsByAppUserIdGetOperation fractionatedFullsByAppUserIdGetOp = new FractionatedFullsByAppUserIdGetOperation();
        try
        {
            fractionatedFullsByAppUserIdGetOp.appUserId = appUserId;
            fractionatedFullsByAppUserIdGetOp.status = status;
            fractionatedFullsByAppUserIdGetOp["on-complete"] = (Action<FractionatedFullsByAppUserIdGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFractionatedFullsRetreived.Invoke(op.investmentIntallmentFulls);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            fractionatedFullsByAppUserIdGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFinancedFullById(int id)
    {
        FinancedFullByIdGetOperation financedFullByIdGetOp = new FinancedFullByIdGetOperation();
        try
        {
            financedFullByIdGetOp.id = id;
            financedFullByIdGetOp["on-complete"] = (Action<FinancedFullByIdGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFinancedFullRetreived.Invoke(op.investmentFinancedFull);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            financedFullByIdGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFinancedFulls(int status)
    {
        FinancedFullsByStatusGetOperation financedFullsByStatusGetOp = new FinancedFullsByStatusGetOperation();
        try
        {
            financedFullsByStatusGetOp.status = status;
            financedFullsByStatusGetOp["on-complete"] = (Action<FinancedFullsByStatusGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFinancedFullsRetreived.Invoke(op.investmentFinancedFulls);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            financedFullsByStatusGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFinancedFullsByAppUserId(int appUserId, int status = -1)
    {
        FinancedFullsByAppUserIdGetOperation financedFullsByAppUserIdGetOp = new FinancedFullsByAppUserIdGetOperation();
        try
        {
            financedFullsByAppUserIdGetOp.appUserId = appUserId;
            financedFullsByAppUserIdGetOp.status = status;
            financedFullsByAppUserIdGetOp["on-complete"] = (Action<FinancedFullsByAppUserIdGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFinancedFullsRetreived.Invoke(op.investmentFinancedFulls);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            financedFullsByAppUserIdGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetPrepaidFullById(int id)
    {
        PrepaidFullByIdGetOperation prepaidFullByIdGetOp = new PrepaidFullByIdGetOperation();
        try
        {
            prepaidFullByIdGetOp.id = id;
            prepaidFullByIdGetOp["on-complete"] = (Action<PrepaidFullByIdGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onPrepaidFullRetreived.Invoke(op.investmentPrepaidFull);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            prepaidFullByIdGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetPrepaidFullsByStatus(int status)
    {
        PrepaidFullsByStatusGetOperation prepaidFullsByStatusGetOp = new PrepaidFullsByStatusGetOperation();
        try
        {
            prepaidFullsByStatusGetOp.status = status;
            prepaidFullsByStatusGetOp["on-complete"] = (Action<PrepaidFullsByStatusGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onPrepaidFullsRetreived.Invoke(op.investmentPrepaidFulls);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            prepaidFullsByStatusGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetPrepaidFullsByAppUserId(int appUserId, int status = -1)
    {
        PrepaidFullsByAppUserIdGetOperation prepaidFullsByAppUserIdGetOp = new PrepaidFullsByAppUserIdGetOperation();
        try
        {
            prepaidFullsByAppUserIdGetOp.appUserId = appUserId;
            prepaidFullsByAppUserIdGetOp.status = status;
            prepaidFullsByAppUserIdGetOp["on-complete"] = (Action<PrepaidFullsByAppUserIdGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onPrepaidFullsRetreived.Invoke(op.investmentPrepaidFulls);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            prepaidFullsByAppUserIdGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetTransactionReceipt(int transactionId)
    {
        TransactionReceiptGetOperation transactionReceiptGetOp = new TransactionReceiptGetOperation();
        try
        {
            transactionReceiptGetOp.transactionId = transactionId;
            transactionReceiptGetOp["on-complete"] = (Action<TransactionReceiptGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onReceiptRetreived.Invoke(op.receipt[1..^1]);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            transactionReceiptGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void RegisterFractionated(Investment investment)
    {
        FractionatedRegisterOperation fractionatedRegisterOp = new FractionatedRegisterOperation();
        try
        {
            fractionatedRegisterOp.investment = investment;
            fractionatedRegisterOp["on-complete"] = (Action<FractionatedRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFractionatedRegistered.Invoke(op.investmentFractionatedFull);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            fractionatedRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void RegisterFinanced(Investment investment)
    {
        FinancedRegisterOperation financedRegisterOp = new FinancedRegisterOperation();
        try
        {
            financedRegisterOp.investment = investment;
            financedRegisterOp["on-complete"] = (Action<FinancedRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFinancedRegistered.Invoke(op.investmentFinancedFull);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            financedRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void RegisterPrepaid(Investment investment)
    {
        PrepaidRegisterOperation prepaidRegisterOp = new PrepaidRegisterOperation();
        try
        {
            prepaidRegisterOp.investment = investment;
            prepaidRegisterOp["on-complete"] = (Action<PrepaidRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onPrepaidRegistered.Invoke(op.investmentPrepaidFull);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            prepaidRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // PAYMENT
    public void BankPayment(InvestmentBankPayment request)
    {
        InvestmentPaymentBankPostOperation investmentBankPaymentPostOp = new InvestmentPaymentBankPostOperation();
        try
        {
            investmentBankPaymentPostOp.request = request;
            investmentBankPaymentPostOp["on-complete"] = (Action<InvestmentPaymentBankPostOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onPaid.Invoke(op.investmentBankPayment);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            investmentBankPaymentPostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    //public void CardPayment(InvestmentCardPayment request)
    //{
    //    InvestmentPaymentCardPostOperation investmentCardPaymentPostOp = new InvestmentPaymentCardPostOperation();
    //    try
    //    {
    //        investmentCardPaymentPostOp.request = request;
    //        investmentCardPaymentPostOp["on-complete"] = (Action<InvestmentPaymentCardPostOperation, HttpResponse>)((op, response) =>
    //        {
    //            if (response != null && !response.HasError)
    //                onPaid.Invoke(op.investmentPayment);
    //            else
    //                onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
    //        });
    //        investmentCardPaymentPostOp.Send();
    //    }
    //    catch (Exception ex)
    //    {
    //        WebManager.Instance.OnSendError(ex.Message);
    //    }
    //}

    // SUPPORT DOCS REGISTER
    public void RegisterDocRTU(InvestmentDocRequest supportDocRequest)
    {
        DocRTURegisterOperation docRTURegisterOp = new DocRTURegisterOperation();
        try
        {
            docRTURegisterOp.investmentDocRequest = supportDocRequest;
            docRTURegisterOp["on-complete"] = (Action<DocRTURegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onDocRegistered.Invoke();
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            docRTURegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void RegisterDocBank(InvestmentDocRequest supportDocRequest)
    {
        DocBankRegisterOperation docBankRegisterOp = new DocBankRegisterOperation();
        try
        {
            docBankRegisterOp.investmentDocRequest = supportDocRequest;
            docBankRegisterOp["on-complete"] = (Action<DocBankRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onDocRegistered.Invoke();
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            docBankRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REFERENCES

    public void RegisterReferences(InvestmentReference[] investmentReferences)
    {
        ReferencesRegisterOperation referencesRegisterOp = new ReferencesRegisterOperation();
        try
        {
            referencesRegisterOp.investmentReferences = investmentReferences;
            referencesRegisterOp["on-complete"] = (Action<ReferencesRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onReferencesRegistered.Invoke(op.ids);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            referencesRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // SIGNATORY

    public void RegisterSignatories(InvestmentIdentityRequest[] investmentIdentityRequests)
    {
        SignatoryRegisterOperation signatoriesRegisterOp = new SignatoryRegisterOperation();
        try
        {
            signatoriesRegisterOp.investmentIdentityRequests = investmentIdentityRequests;
            signatoriesRegisterOp["on-complete"] = (Action<SignatoryRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onIdentitiesRegistered.Invoke(op.ids);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            signatoriesRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void RegisterSignatoryEmpty(int investmentId)
    {
        SignatoryCreateOperation signatoryEmptyRegisterOp = new SignatoryCreateOperation();
        try
        {
            signatoryEmptyRegisterOp.investmentId = investmentId;
            signatoryEmptyRegisterOp["on-complete"] = (Action<SignatoryCreateOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onIdentitiesEmptyRegistered.Invoke();
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            signatoryEmptyRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // BENEFICIARY

    public void RegisterBeneficiaries(InvestmentIdentityRequest[] investmentIdentityRequests)
    {
        BeneficiaryRegisterOperation beneficiariesRegisterOp = new BeneficiaryRegisterOperation();
        try
        {
            beneficiariesRegisterOp.investmentIdentityRequests = investmentIdentityRequests;
            beneficiariesRegisterOp["on-complete"] = (Action<BeneficiaryRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onIdentitiesRegistered.Invoke(op.ids);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            beneficiariesRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void RegisterBeneficiaryEmpty(int investmentId)
    {
        BeneficiaryCreateOperation beneficiaryEmptyRegisterOp = new BeneficiaryCreateOperation();
        try
        {
            beneficiaryEmptyRegisterOp.investmentId = investmentId;
            beneficiaryEmptyRegisterOp["on-complete"] = (Action<BeneficiaryCreateOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onIdentitiesEmptyRegistered.Invoke();
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            beneficiaryEmptyRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // SUPPORT DOCS UPDATE
    public void UpdateDocRTU(InvestmentDocRequest supportDocRequest)
    {
        DocRTUUpdateOperation docRTUUpdateOp = new DocRTUUpdateOperation();
        try
        {
            docRTUUpdateOp.investmentDocRequest = supportDocRequest;
            docRTUUpdateOp["on-complete"] = (Action<DocRTUUpdateOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onDocUpdate.Invoke();
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            docRTUUpdateOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void UpdateDocBank(InvestmentDocRequest supportDocRequest)
    {
        DocBankUpdateOperation docBankUpdateOp = new DocBankUpdateOperation();
        try
        {
            docBankUpdateOp.investmentDocRequest = supportDocRequest;
            docBankUpdateOp["on-complete"] = (Action<DocBankUpdateOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onDocUpdate.Invoke();
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            docBankUpdateOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REFERENCES

    public void UpdateReferences(InvestmentReference[] investmentReferences)
    {
        ReferencesUpdateOperation referencesUpdateOp = new ReferencesUpdateOperation();
        try
        {
            referencesUpdateOp.investmentReferences = investmentReferences;
            referencesUpdateOp["on-complete"] = (Action<ReferencesUpdateOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onReferencesUpdated.Invoke(op.ids);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            referencesUpdateOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // SIGNATORY

    public void UpdateSignatories(InvestmentIdentityRequest[] investmentIdentityRequests)
    {
        SignatoryUpdateOperation signatoriesUpdateOp = new SignatoryUpdateOperation();
        try
        {
            signatoriesUpdateOp.investmentIdentityRequests = investmentIdentityRequests;
            signatoriesUpdateOp["on-complete"] = (Action<SignatoryUpdateOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onIdentitiesUpdated.Invoke(op.ids);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            signatoriesUpdateOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // BENEFICIARY

    public void UpdateBeneficiaries(InvestmentIdentityRequest[] investmentIdentityRequests)
    {
        BeneficiaryUpdateOperation beneficiariesUpdateOp = new BeneficiaryUpdateOperation();
        try
        {
            beneficiariesUpdateOp.investmentIdentityRequests = investmentIdentityRequests;
            beneficiariesUpdateOp["on-complete"] = (Action<BeneficiaryUpdateOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onIdentitiesUpdated.Invoke(op.ids);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            beneficiariesUpdateOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}
