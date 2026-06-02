using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;

public class IdentityPlaceUpdateAction : MonoBehaviour
{
    [Title("Data")]
    [SerializeField]
    DataMapper dtmIdentityPlace = null;

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    [Space]
    [SerializeField, TextArea(2, 4)]
    String updatedMessage = "La información fue guardada exitosamente.";

    IdentityService identityService = null;

    IdentityPlace identityPlace = null;
    IdentityPlace identityPlaceNew = null;

    private void Awake()
    {
        identityService = GetComponent<IdentityService>();
    }

    private void Start()
    {
        btnUpdate?.AddAction(DoUpdate);
    }

    public void Clear()
    {
        dtmIdentityPlace.ClearElements();
        identityPlace = null;
        identityPlaceNew = null;
    }

    public void Populate()
    {
        identityPlace = new IdentityPlace(StateManager.Instance.AppUser.Id, StateManager.Instance.Identity);

        if (identityPlace == null)
            return;

        dtmIdentityPlace.PopulateClass<IdentityPlace>(identityPlace);
    }

    private void DoUpdate()
    {

        identityPlaceNew = dtmIdentityPlace.BuildClass<IdentityPlace>();

        if (!IdentityChanged())
        {
            //ChoiceDialog.Instance.Warning("Sin cambios", "No se detectaron cambios en la información.");
            ChangeNextPage();
            return;
        }

        ScreenDialog.Instance.Display();

        identityPlaceNew.AppUserId = StateManager.Instance.AppUser.Id;
        identityPlaceNew.IdentityId = StateManager.Instance.Identity.Id;

        identityService.UpdatePlace(identityPlaceNew);

    }

    public void ApplyIdentity(long id)
    {
        if (id == -1)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        StateManager.Instance.UpdateIdentityPlace(id, identityPlaceNew);

        ChangeNextPage();
    }

    private void ChangeNextPage()
    {
        ChoiceDialog.Instance.Info("Información actualizada", updatedMessage, () => PageManager.Instance.ChangePage(pagNext));
    }

    private bool IdentityChanged()
    {
        if (identityPlace == null || identityPlaceNew == null)
            return false;

        if (identityPlace.BirthCountryId != identityPlaceNew.BirthCountryId)
            return true;

        if (identityPlace.BirthStateId != identityPlaceNew.BirthStateId)
            return true;

        if (identityPlace.BirthCityId != identityPlaceNew.BirthCityId)
            return true;

        return false;
    }
}
