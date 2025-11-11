using System;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Dialog;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;
using Leap.Graphics.Tools;
using Leap.UI.Page;
using Leap.UI.Dialog.Gallery;
using System.Net;

public class AddressUpdateAction : MonoBehaviour
{
    [Title("Fields")]
    [SerializeField]
    ElementValue[] elementValuesAddress = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmUpdateAddress = null;

    [Title("Action")]
    [SerializeField]
    Button btnUpdateAddress = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    [Title("Message")]
    [SerializeField]
    String addressUpdatedTitle = "Información actualizada";
    [SerializeField, TextArea(2, 4)]
    String addressUpdatedMessage = "La información fue guardada exitosamente.";

    AddressService addressService = null;

    private void Awake()
    {
        addressService = GetComponent<AddressService>();
    }

    private void Start()
    {
        btnUpdateAddress?.AddAction(DoUpdateAddress);
    }
    
    private void DoUpdateAddress()
    {
        ScreenDialog.Instance.Display();
        Invoke(nameof(UpdateAddress), 0.2f);
    }
  
    public void ClearAddress()
    {
        for (int i = 0; i < elementValuesAddress.Length; i++)
            elementValuesAddress[i].Clear();
    }

    public void PopulateAddress()
    {
        Address address = StateManager.Instance.Address;

        dtmUpdateAddress.PopulateClass<Address>(address);
    }

    Address address;

    private void UpdateAddress()
    {
        if (!ElementHelper.Validate(elementValuesAddress))
            return;

        ScreenDialog.Instance.Display();

        address = dtmUpdateAddress.BuildClass<Address>();
        address.Id = StateManager.Instance.Address.Id;

        addressService.UpdateAddress(address);
    }

    public void ApplyAddress(int id)
    {
        address.Id = id;
        StateManager.Instance.Address = address;

        ChoiceDialog.Instance.Info(addressUpdatedTitle, addressUpdatedMessage, () => PageManager.Instance.ChangePage(pagNext));
    }
}