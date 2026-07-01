using System;
using UnityEngine;

using Leap.Core.Tools;
using Leap.Data.Mapper;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;

public class LocalityUpdateAction : MonoBehaviour
{
    [Title("Params")]
    [SerializeField]
    bool isInterest = true;

    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmLocality = null;

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    [Title("Message")]
    [SerializeField, TextArea(2, 4)]
    String updatedMessage = "La información fue guardada exitosamente.";

    [Title("Event")]
    [SerializeField]
    UnityBoolEvent onLocalityChanged = null;

    AppUserService appUserService = null;

    Locality locality = null;
    Locality localityNew = null;

    private void Awake()
    {
        appUserService = GetComponent<AppUserService>();
    }

    private void Start()
    {
        btnUpdate?.AddAction(DoUpdate);
    }

    public void Clear()
    {
        dtmLocality.ClearElements();
        locality = null;
        localityNew = null;
    }

    public void Populate()
    {
        Clear();

        locality = isInterest ? StateManager.Instance.InterestLocality : StateManager.Instance.CurrentLocality;

        if (locality == null)
            return;

        dtmLocality.PopulateClass<Locality>(locality);
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        localityNew = dtmLocality.BuildClass<Locality>();

        if (!LocalityChanged())
        {
            //ChoiceDialog.Instance.Warning("Sin cambios", "No se detectaron cambios en la información.");
            ChangeNextPage();
            return;
        }

        ScreenDialog.Instance.Display();

        localityNew.Id = isInterest ? StateManager.Instance.InterestLocality.Id : StateManager.Instance.CurrentLocality.Id;
        localityNew.AppUserId = StateManager.Instance.AppUser.Id;
        localityNew.LocalityType = isInterest ? 1 : 2;

        appUserService.UpdateLocality(localityNew);
    }

    public void ApplyLocality(long id)
    {       
        if (id == -1)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        localityNew.Id = id;
        localityNew.Status = 1;

        if (isInterest)
            StateManager.Instance.InterestLocality = localityNew;
        else
            StateManager.Instance.CurrentLocality = localityNew;

        onLocalityChanged.Invoke(isInterest);

        Clear();
        ChangeNextPage();
    }

    private void ChangeNextPage()
    {
        ChoiceDialog.Instance.Info("Información actualizada", updatedMessage, () => PageManager.Instance.ChangePage(pagNext));
    }

    private bool LocalityChanged()
    {
        if (locality == null)
            return localityNew.CountryId != -1
                || localityNew.StateId != -1
                || localityNew.CityId != -1;

        return locality.CountryId != localityNew.CountryId
            || locality.StateId != localityNew.StateId
            || locality.CityId != localityNew.CityId;
    }
}
