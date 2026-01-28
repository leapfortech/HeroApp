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
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

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
    String birthDateErrorMessage = "La fecha de nacimiento es incorrecta. Revisa e intenta de nuevo.";

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
        if (!ElementHelper.Validate(elementValues))
            return;

        identityNew = dtmIdentity.BuildClass<Identity>();

        if (!IdentityChanged())
        {
            ChoiceDialog.Instance.Info("Sin cambios", "No se detectaron cambios en la información.");
            return;
        }

        if (identityNew.BirthDate == new DateTime(0001, 1, 1))
        {
            ChoiceDialog.Instance.Error("Error de fecha", birthDateErrorMessage);
            return;
        }

        if (CalculateAge(identityNew.BirthDate) < 18)
        {
            ChoiceDialog.Instance.Error("Error de fecha", birthDateErrorMessage);
            return;
        }

        ScreenDialog.Instance.Display();

        identityNew.PhoneCountryId = WebManager.Instance.WebSysUser.PhoneCountryId;
        identityNew.Phone = WebManager.Instance.WebSysUser.Phone;
        identityNew.Email = WebManager.Instance.WebSysUser.Email;

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

        if (!string.Equals(identity.FirstName1, identityNew.FirstName1))
            return true;

        if (!string.Equals(identity.FirstName2, identityNew.FirstName2))
            return true;

        if (!string.Equals(identity.LastName1, identityNew.LastName1))
            return true;

        if (!string.Equals(identity.LastName2, identityNew.LastName2))
            return true;

        if (identity.GenderId != identityNew.GenderId)
            return true;

        if (identity.BirthDate.Date != identityNew.BirthDate.Date)
            return true;

        if (identity.OriginCountryId != identityNew.OriginCountryId)
            return true;

        if (identity.OriginStateId != identityNew.OriginStateId)
            return true;

        return false;
    }
}
