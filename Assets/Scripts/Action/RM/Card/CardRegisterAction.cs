using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Collections;
using Leap.Finance.Payment;

public class CardRegisterAction : MonoBehaviour
{
    [Header("Fields")]
    //[SerializeField]
    //Image imgCardBrand = null;
    [SerializeField]
    InputField ifdCardNumber = null;
    [SerializeField]
    InputField ifdCardHolder = null;
    [SerializeField]
    MultiWheel mwlExpirationDate = null;
    [SerializeField]
    InputField ifdSecurityCode = null;

    [Header("Params")]
    [SerializeField]
    String CurrencyCode = null;
    [SerializeField]
    String AuthorizationAmount = null;

    [Space]
    [Header("Data")]
    [SerializeField]
    ValueList vllCardBrand = null;

    [Header("Action")]
    [SerializeField]
    Button btnRegister = null;

    [Header("Result")]
    [SerializeField]
    Page nextPage = null;

    [Header("Messages")]
    [SerializeField, TextArea(2, 4)]
    String expiredCardError = "La Tarjeta que deseas registrar está Vencida.\r\nPor favor, revisa la fecha de Vencimiento o usa otra Tarjeta";

    [SerializeField, TextArea(2, 4)]
    String rejectedCardError = "Tu Tarjeta fue rechazada.\r\nPor favor, verifica tus datos\r\no comunícate con la Entidad Emisora.";

    [SerializeField, TextArea(2, 4)]
    String addCardError = "En este momento, no es posible Agregar tu Tarjeta.\r\nPor favor, intenta de nuevo más tarde.";

    [SerializeField, TextArea(2, 4)]
    String cardIssuerError = "El Número Ingresado no corresponde\r\nal de una Tarjeta {0}";

    CardService cardService;
    CybersourceService cybersourceService;

    ElementValue[] elementValues = null;

    public int CardIssuerIdx { get; set; } = 0;

    String number = null;
    String instrumentIdentifierId;

    private void Awake()
    {
        cardService = GetComponent<CardService>();
        cybersourceService = GetComponent<CybersourceService>();
    }

    private void Start()
    {
        Initialize();

        btnRegister?.AddAction(Do);
    }

    private void Initialize()
    {
        if (elementValues != null)
            return;

        elementValues = new ElementValue[3];
        elementValues[0] = ifdCardNumber;
        elementValues[1] = ifdCardHolder;
        elementValues[2] = ifdSecurityCode;
    }

    public void Clear()
    {
        Initialize();

        for (int i = 0; i < elementValues.Length; i++)
            elementValues[i].Clear();
    }

    // Populate

    //public void Populate()
    //{
    //    imgCardBrand.Sprite = (Sprite)vllCardBrand[CardIssuerIdx]["Brand"];
    //}

    // Add

    public void Do()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        DateTime date = new DateTime(DateTime.Today.Year + mwlExpirationDate.GetSelectedIndexes()[1], mwlExpirationDate.GetSelectedIndexes()[0] + 1, 1).AddMonths(1).AddDays(-1);
        if (date < DateTime.Today)
        {
            ChoiceDialog.Instance.Error(expiredCardError);
            return;
        }

        ScreenDialog.Instance.Display();

        GetBillTo();
    }

    public void GetBillTo()
    {
        cardService.GetBillTo(StateManager.Instance.AppUser.Id);
    }

    public void AddInstrumentIdentifier(CSBillTo billTo)
    {
        number = ifdCardNumber.Text.Replace(" ", "");

        CSSimplePaymentRequest paymentRequest = new CSSimplePaymentRequest();

        paymentRequest.processingInformation = new CSProcessingInformation("TOKEN_CREATE", "instrumentIdentifier");
        paymentRequest.paymentInformation = new CSPaymentInformation(new CSCard(number, (mwlExpirationDate.GetSelectedIndexes()[0] + 1).ToString(),
                                                                                        (DateTime.Today.Year + mwlExpirationDate.GetSelectedIndexes()[1]).ToString(), ifdSecurityCode.Text));
        paymentRequest.orderInformation = new CSOrderInformation(new CSAmountDetails(AuthorizationAmount, CurrencyCode), billTo);

        cybersourceService.SimplePayment(paymentRequest);
    }

    public void ReverseAuthorization(CSSimplePaymentResponse simplePaymentResponse)
    {
        if (simplePaymentResponse.status != "AUTHORIZED")
        {
            ChoiceDialog.Instance.Error(rejectedCardError);
            return;
        }

        instrumentIdentifierId = simplePaymentResponse.tokenInformation.instrumentIdentifier.id;

        CSAuthorizationReversalRequest authorizationReversalRequest = new CSAuthorizationReversalRequest();

        String reference = $"A{StateManager.Instance.AppUser.Id.ToString("D06")}{number.Length.ToString("D02")}{number.Substring(number.Length - 4, 4)}{DateTime.Now:yyyyMMddHHmmss}";
        authorizationReversalRequest.clientReferenceInformation = new CSClientReferenceInformation(reference);

        authorizationReversalRequest.reversalInformation = new CSReversalInformation(new CSAmountDetails(AuthorizationAmount, CurrencyCode), "");

        cybersourceService.AuthorizationReversal(simplePaymentResponse.id, authorizationReversalRequest);
    }

    public void AddCard(CSAuthorizationReversalResponse authorizationReversalResponse)
    {
        //if (authorizationReversalResponse.status != "REVERSED")
        //{
        //    ChoiceDialog.Instance.Error(rejectedCardError);
        //    return;
        //}

        CardRegister cardRegister = new CardRegister();
        cardRegister.InstrumentIdentifierId = instrumentIdentifierId;
        cardRegister.AppUserId = StateManager.Instance.AppUser.Id;
        cardRegister.TypeId = vllCardBrand[CardIssuerIdx].Id;
        cardRegister.Number = number.Substring(number.Length - 4, 4);
        cardRegister.Digits = number.Length;
        cardRegister.ExpirationMonth = mwlExpirationDate.GetSelectedIndexes()[0] + 1;
        cardRegister.ExpirationYear = DateTime.Today.Year + mwlExpirationDate.GetSelectedIndexes()[1];
        cardRegister.Holder = ifdCardHolder.Text;
        cardRegister.UtcOffset = (float)(DateTime.Now - DateTime.UtcNow).TotalHours;

        cardService.RegisterCard(cardRegister);
    }

    public void AddCardLocal(Card card)
    {
        StateManager.Instance.Card = card;

        Clear();

        PageManager.Instance.ChangePage(nextPage);
    }

    // Messages

    public void AddCardError(CSErrors csErrors)
    {
        for (int i = 0; i < csErrors.errors.Length; i++)
            for (int k = 0; k < csErrors.errors[i].details.Length; k++)
                if (csErrors.errors[i].details[k].location == "card" && csErrors.errors[i].details[k].name == "type")
                {
                    ChoiceDialog.Instance.Error(String.Format(cardIssuerError, vllCardBrand[CardIssuerIdx]["Name"]));
                    return;
                }

        ChoiceDialog.Instance.Error(addCardError);

        //String message = "";
        //for (int i = 0; i < csErrors.errors.Length; i++)
        //{
        //    message += csErrors.errors[i].message + "\r\n";
        //    for (int k = 0; k < csErrors.errors[i].details.Length; k++)
        //        message += csErrors.errors[i].details[k].location + " : " + csErrors.errors[i].details[k].name + "\r\n";
        //}
        //message = message.Substring(0, message.Length - 2);

        //ChoiceDialog.Instance.Error(message);
    }

    public void RejectedCardError()
    {
        ChoiceDialog.Instance.Error(rejectedCardError);
    }
}
