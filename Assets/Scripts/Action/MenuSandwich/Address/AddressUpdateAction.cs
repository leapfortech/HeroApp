using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;
using Leap.Data.Web;

public class AddressUpdateAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmAddress = null;

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    [Title("Message")]
    [Space]
    [SerializeField, TextArea(2, 4)]
    String updatedMessage = "La información fue guardada exitosamente.";

    AddressService addressService = null;

    Address address = null;
    Address addressNew = null;

    private void Awake()
    {
        addressService = GetComponent<AddressService>();
    }

    private void Start()
    {
        btnUpdate?.AddAction(DoUpdate);
    }

    public void Clear()
    {
        dtmAddress.ClearElements();
        address = null;
        addressNew = null;
    }

    public void Populate()
    {
        address = StateManager.Instance.Address;

        if (address == null)
            return;

        dtmAddress.PopulateClass<Address>(address);
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        addressNew = dtmAddress.BuildClass<Address>();

        addressNew.Id = address.Id;

        addressService.UpdateAddress(StateManager.Instance.AppUser.Id, addressNew);
    }

    public void ApplyAddress(long id)
    {
        if (id == -1)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        addressNew.Id = id;
        StateManager.Instance.Address = addressNew;

        Clear();

        ChoiceDialog.Instance.Info("Información actualizada", updatedMessage, () => PageManager.Instance.ChangePage(pagNext));
    }
}
