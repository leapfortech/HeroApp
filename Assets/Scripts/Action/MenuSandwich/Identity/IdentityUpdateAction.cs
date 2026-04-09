using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;
using Leap.Data.Web;

public class IdentityUpdateAction : MonoBehaviour
{
    [Title("Page")]
    [SerializeField]
    bool isPersonal = true;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmIdentity = null;

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    [Title("Message")]
    [SerializeField, TextArea(2, 4)]
    String minorError = "No se permite el registro de menores de edad.";

    [Space]
    [SerializeField, TextArea(2, 4)]
    String updatedMessage = "La información fue guardada exitosamente.";

    IdentityService identityService = null;

    Identity identity = null;
    Identity identityNew = null;

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
        dtmIdentity.ClearElements();
        identity = null;
        identityNew = null;
    }

    public void Populate()
    {
        identity = StateManager.Instance.Identity;

        if (identity == null)
            return;

        dtmIdentity.PopulateClass<Identity>(identity);
    }

    private void DoUpdate()
    {
        DateTime sqlMinDate = new DateTime(1753, 1, 1);

        identityNew = dtmIdentity.BuildClass<Identity>();

        if (isPersonal)
        {
            if (identityNew.BirthDate == new DateTime(0001, 1, 1))
            {
                identityNew.BirthDate = sqlMinDate;
            }
            else
            {
                if (CalculateAge(identityNew.BirthDate) < 18)
                {
                    ChoiceDialog.Instance.Error("Error de fecha", minorError);
                    return;
                }
            }

            identityNew.BirthCountryId = identity.BirthCountryId;
            identityNew.BirthStateId = identity.BirthStateId;
            identityNew.BirthCityId = identity.BirthCityId;
        }
        else
        {
            identityNew.FirstName1 = identity.FirstName1;
            identityNew.FirstName2 = identity.FirstName2;
            identityNew.LastName1 = identity.LastName1;
            identityNew.LastName2 = identity.LastName2;
            identityNew.BirthDate = identity.BirthDate;
            identityNew.GenderId = identity.GenderId;
        }

        if (!IdentityChanged())
        {
            ChoiceDialog.Instance.Warning("Sin cambios", "No se detectaron cambios en la información.");
            return;
        }

        ScreenDialog.Instance.Display();

        identityNew.Id = identity.Id;

        identityService.UpdateIdentity(StateManager.Instance.AppUser.Id, identityNew);
    }

    public void ApplyIdentity(long id)
    {
        if (id == -1)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        identityNew.Id = id;
        StateManager.Instance.Identity = identityNew;

        ChoiceDialog.Instance.Info("Información actualizada", updatedMessage, () => PageManager.Instance.ChangePage(pagNext));
    }

    public int CalculateAge(DateTime birthDate)
    {
        int age = DateTime.Today.Year - birthDate.Year;

        if (DateTime.Today.Month < birthDate.Month)
            return age - 1;

        if (DateTime.Today.Month == birthDate.Month && DateTime.Today.Day < birthDate.Day)
            return age - 1;

        return age;
    }

    private bool IdentityChanged()
    {
        if (identity == null || identityNew == null)
            return false;

        if (!string.Equals(identity.FirstName1 ?? "", identityNew.FirstName1 ?? ""))
            return true;

        if (!string.Equals(identity.FirstName2 ?? "", identityNew.FirstName2 ?? ""))
            return true;

        if (!string.Equals(identity.LastName1 ?? "", identityNew.LastName1 ?? ""))
            return true;

        if (!string.Equals(identity.LastName2 ?? "", identityNew.LastName2 ?? ""))
            return true;

        if (identity.GenderId != identityNew.GenderId)
            return true;

        if (identity.BirthDate.Date != identityNew.BirthDate.Date)
            return true;

        if (identity.BirthCountryId != identityNew.BirthCountryId)
            return true;

        if (identity.BirthStateId != identityNew.BirthStateId)
            return true;

        if (identity.BirthCityId != identityNew.BirthCityId)
            return true;

        return false;
    }
}
