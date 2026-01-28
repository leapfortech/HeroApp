using System;
using UnityEngine;
using UnityEngine.Events;

using Leap.Core.Tools;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Collections;

using Sirenix.OdinInspector;

public class CardAction : MonoBehaviour
{
    [Title("Fields")]
    [SerializeField]
    Text txtValid = null;
    [SerializeField]
    Text txtOverdue = null;
    [SerializeField]
    Text txtCardNumber = null;
    [SerializeField]
    Text txtExpirationDate = null;
    [SerializeField]
    Text txtCardHolder = null;
    [SerializeField]
    Image imgCardBrand = null;

    [Title("Data")]
    [SerializeField]
    ValueList vllCardBrand = null;

    [Title("Texts")]
    [SerializeField]
    String valid = "Valid";
    [SerializeField]
    String overdue = "Expired";

    [Title("Action")]
    [SerializeField]
    Button btnRemove = null;
    [SerializeField]
    Button btnUpdate = null;

    [Title("Pages")]
    [SerializeField]
    Page pagCardNone = null; 
    [SerializeField]
    Page pagRepayCard = null;
    [SerializeField]
    Page pagCardDetail = null;
    [SerializeField]
    Page pagCardUpdate = null;

    [Space]
    [SerializeField]
    Page nextPage = null;

    [Title("Flow")]
    [SerializeField]
    bool sandwich = false;

    CardService cardService;

    private void Awake()
    {
        cardService = GetComponent<CardService>();
    }

    private void Start()
    {
        btnRemove?.AddAction(AskRemoveCard);
        btnUpdate?.AddAction(UpdateCard);
    }

    public void Populate()
    {
        Card card = StateManager.Instance.Card;

        txtValid.TextValue = DateTime.Today > card.ExpirationDate ? "" : valid;
        txtOverdue.TextValue = DateTime.Today > card.ExpirationDate ? overdue : "";
        txtCardNumber.TextValue = (new String('x', card.Digits - 4) + card.Number).FormatCard();
        txtExpirationDate.TextValue = $"{card.ExpirationDate:MM/yy}";
        txtCardHolder.TextValue = card.Holder;
        imgCardBrand.Sprite = vllCardBrand.FindRecordCellSprite(card.TypeId, "Brand");
    }

    // Remove

    public void AskRemoveCard()
    {
        ChoiceDialog.Instance.Info("Eliminar tarjeta", "¿Estás seguro que deseas eliminar la tarjeta **** " + StateManager.Instance.Card.Number + "?", RemoveCard, (UnityAction)null);
    }

    public void RemoveCard()
    {
        ScreenDialog.Instance.Display();
        cardService.SetStatus(StateManager.Instance.Card.Id, 0);
    }

    public void RemoveCardLocal()
    {
        StateManager.Instance.Card = null;
        PageManager.Instance.ChangePage(nextPage);
    }

    // Update

    public void UpdateCard()
    {
        ChoiceDialog.Instance.Info("Verificación de tarjeta", "Cuando agregues una nueva tarjeta se realizará una validación de seguridad realizando un cargo a la tarjeta. Este cargo se revertirá una vez que se complete la validación.", ChangePageUpdate);
    }

    public void ChangePageUpdate()
    {
        PageManager.Instance.ChangePage(pagCardUpdate);
    }

    // Open

    public void ChangePageCard(bool update)
    {
        if (StateManager.Instance.Card == null)
        {
            PageManager.Instance.ChangePage(pagCardNone);
            pagCardUpdate.HeaderPage = pagCardNone;
        }
        else
        {
            if (update)
            {
                PageManager.Instance.ChangePage(pagCardDetail);
                pagCardUpdate.HeaderPage = pagCardDetail;
            }
            else
            {
                PageManager.Instance.ChangePage(pagRepayCard);
                pagCardUpdate.HeaderPage = pagRepayCard;
            }
        }

        if (sandwich)
            SandwichMenu.Instance.Close();
    }
}