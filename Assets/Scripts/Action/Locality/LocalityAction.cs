using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;

public class LocalityAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmInterestLocality = null;
    [SerializeField]
    DataMapper dtmCurrentLocality = null;

    [Title("Action")]
    [SerializeField]
    Button btnRegister = null;
    //[SerializeField]
    //Button btnExit = null;

    [Title("Page")]
    //[SerializeField]
    //Page pagStart = null;

    [SerializeField]
    Page pagDone = null;

    [SerializeField]
    Page pagExit = null;

    [Title("Messages")]
    [SerializeField, TextArea(2, 4)]
    String exitMessage = "Si sales ahora no se guardarán los datos.";

    AppUserService appUserService = null;
    
    Locality interestLocality = null;
    Locality currentLocality = null;

    private void Awake()
    {
        appUserService = GetComponent<AppUserService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(Register);
        //btnExit?.AddAction(Exit);
    }

    public void Clear()
    {
        dtmInterestLocality.ClearElements();
        dtmCurrentLocality.ClearElements();
    }

    public bool Exit()
    {
        ChoiceDialog.Instance.Warning("¿Estas seguro?", exitMessage, () => ChangeExitPage(), null, "Si, deseo salir", "Continuar con configuración");

        return false;
    }

    private void Register()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        interestLocality = dtmInterestLocality.BuildClass<Locality>();
        currentLocality = dtmCurrentLocality.BuildClass<Locality>();

        long appUserId = StateManager.Instance.AppUser.Id;

        interestLocality.AppUserId = appUserId;
        interestLocality.LocalityType = 1;

        currentLocality.AppUserId = appUserId;
        currentLocality.LocalityType = 2;

        appUserService.RegisterLocality(new LocalityRequest(interestLocality, currentLocality));
    }

    public void ApplyLocality(LocalityResponse localityResponse)
    {
        interestLocality.Id = localityResponse.InterestLocalityId;
        interestLocality.Status = 1;

        currentLocality.Id = localityResponse.CurrentLocalityId;
        currentLocality.Status = 1;

        StateManager.Instance.InterestLocality = interestLocality;
        StateManager.Instance.CurrentLocality = currentLocality;

        Clear();
        PageManager.Instance.ChangePage(pagDone);
    }

    public void ChangeExitPage()
    {
        Clear();
        PageManager.Instance.ChangePage(pagExit);
    }
}
