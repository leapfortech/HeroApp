using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;

public class AddressCityUpdateAction : MonoBehaviour
{
    [Title("Data")]
    [SerializeField]
    DataMapper dtmAddressCity = null;

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

    AddressCity addressCity = null;
    AddressCity addressCityNew = null;

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
        dtmAddressCity.ClearElements();
        addressCity = null;
        addressCityNew = null;
    }

    public void Populate()
    {
        addressCity = new AddressCity(StateManager.Instance.AppUser.Id, StateManager.Instance.Address);

        if (addressCity == null)
            return;

        dtmAddressCity.PopulateClass<AddressCity>(addressCity);
    }

    private void DoUpdate()
    {
        addressCityNew = dtmAddressCity.BuildClass<AddressCity>();

        if (!IdentityChanged())
        {
            ChoiceDialog.Instance.Warning("Sin cambios", "No se detectaron cambios en la información.");
            return;
        }

        ScreenDialog.Instance.Display();

        addressCityNew.AppUserId = StateManager.Instance.AppUser.Id;
        addressCityNew.AddressId = StateManager.Instance.Address.Id;

        addressService.UpdateCity(addressCityNew);
    }

    public void ApplyAddress(long id)
    {
        if (id == -1)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        StateManager.Instance.UpdateAddressCity(id, addressCityNew);

        ChoiceDialog.Instance.Info("Información actualizada", updatedMessage, () => PageManager.Instance.ChangePage(pagNext));
    }

    private bool IdentityChanged()
    {
        if (addressCity == null || addressCityNew == null)
            return false;

        if (NormalizeId(addressCity.CountryId) != NormalizeId(addressCityNew.CountryId))
            return true;

        if (NormalizeId(addressCity.StateId) != NormalizeId(addressCityNew.StateId))
            return true;

        if (NormalizeId(addressCity.CityId) != NormalizeId(addressCityNew.CityId))
            return true;

        return false;
    }

    private long? NormalizeId(long? value)
    {
        if (!value.HasValue || value.Value <= 0)
            return null;

        return value.Value;
    }
}
