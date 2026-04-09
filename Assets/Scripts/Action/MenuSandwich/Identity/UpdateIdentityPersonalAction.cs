using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;

public class UpdateIdentityPersonalAction : MonoBehaviour
{
    [Title("Data")]
    [SerializeField]
    DataMapper dtmIdentityPersonal = null;

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

    IdentityPersonal identityPersonal = null;
    IdentityPersonal identityPersonalNew = null;

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
        dtmIdentityPersonal.ClearElements();
        identityPersonal = null;
        identityPersonalNew = null;
    }

    public void Populate()
    {
        identityPersonal = new IdentityPersonal(StateManager.Instance.AppUser.Id, StateManager.Instance.Identity);

        if (identityPersonal == null)
            return;

        dtmIdentityPersonal.PopulateClass<IdentityPersonal>(identityPersonal);
    }

    private void DoUpdate()
    {
        DateTime sqlMinDate = new DateTime(1753, 1, 1);

        identityPersonalNew = dtmIdentityPersonal.BuildClass<IdentityPersonal>();

        if (identityPersonalNew.BirthDate != sqlMinDate && CalculateAge(identityPersonalNew.BirthDate) < 18)
        {
            ChoiceDialog.Instance.Error("Error de fecha", minorError);
            return;
        }

        if (!IdentityChanged())
        {
            ChoiceDialog.Instance.Warning("Sin cambios", "No se detectaron cambios en la información.");
            return;
        }

        ScreenDialog.Instance.Display();

        identityPersonalNew.AppUserId = StateManager.Instance.AppUser.Id;
        identityPersonalNew.IdentityId = StateManager.Instance.Identity.Id;

        identityService.UpdatePersonal(identityPersonalNew);

    }

    public void ApplyIdentity(long id)
    {
        if (id == -1)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        StateManager.Instance.UpdateIdentityPersonal(id, identityPersonalNew);

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
        if (identityPersonal == null || identityPersonalNew == null)
            return false;

        if (!string.Equals(identityPersonal.FirstName1 ?? "", identityPersonalNew.FirstName1 ?? ""))
            return true;

        if (!string.Equals(identityPersonal.FirstName2 ?? "", identityPersonalNew.FirstName2 ?? ""))
            return true;

        if (!string.Equals(identityPersonal.LastName1 ?? "", identityPersonalNew.LastName1 ?? ""))
            return true;

        if (!string.Equals(identityPersonal.LastName2 ?? "", identityPersonalNew.LastName2 ?? ""))
            return true;

        if (identityPersonal.GenderId != identityPersonalNew.GenderId)
            return true;

        if (identityPersonal.BirthDate.Date != identityPersonalNew.BirthDate.Date)
            return true;


        return false;
    }
}
