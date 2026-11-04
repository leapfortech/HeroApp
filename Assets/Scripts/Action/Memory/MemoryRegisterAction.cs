using System;
using UnityEngine;

using Leap.UI.Extensions;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;

public class MemoryRegisterAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmMemoryFull = null;
    [SerializeField]
    DataMapper dtmTime = null;

    [SerializeField]
    DataMapper dtmImagesVLL = null;
    [SerializeField]
    ComboAdapter cmbCountry = null;
    [SerializeField]
    ComboAdapter cmbState = null;

    [Title("Action")]
    [SerializeField]
    Button btnRegister = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    [Title("Message")]
    [SerializeField]
    String disclaimerTitle = "Aviso importante";
    [Space, SerializeField, TextArea(2, 4)]
    String disclaimerMessage = "Te recomendamos";

    MemoryService memoryService = null;

    private void Awake()
    {
        memoryService = GetComponent<MemoryService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(Register);
    }

    public void Clear()
    {
        dtmMemoryFull.ClearElements();
        dtmTime.ClearElements();
        dtmImagesVLL.ClearElements();
        FillCombos();
    }

    public void FillCombos()
    {
        cmbCountry.Select(StateManager.Instance.InterestLocality.CountryId);
        cmbState.Select(StateManager.Instance.InterestLocality.StateId);
    }


    private void Register()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        MemoryFull memoryFull = dtmMemoryFull.BuildClass<MemoryFull>();
        memoryFull.AppUserId = StateManager.Instance.AppUser.Id;
        memoryFull.PostCountryId = StateManager.Instance.InterestLocality.CountryId;
        memoryFull.PostStateId = StateManager.Instance.InterestLocality.StateId;

        if (memoryFull.DateTime.HasValue)
        {
            String timeStr = dtmTime.BuildBuiltIn<String>();
            String[] time = timeStr.Split('|');
            memoryFull.DateTime = new DateTime(memoryFull.DateTime.Value.Year, memoryFull.DateTime.Value.Month, memoryFull.DateTime.Value.Day,
                                               Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
        }

        if (memoryFull.DateTime.Value > DateTime.Now)
        {
            ChoiceDialog.Instance.Error("Fecha inválida", "La fecha no puede ser mayor a la fecha actual.");
            return;
        }

        // Images
        memoryFull.ImageSprites = dtmImagesVLL.BuildBuiltInList<Sprite>();

        ChoiceDialog.Instance.Warning(disclaimerTitle, disclaimerMessage, () => DoRegister(memoryFull), null, "De acuerdo", "Regresar");
    }

    private void DoRegister(MemoryFull memoryFull)
    {
        ScreenDialog.Instance.Display();
        memoryService.Register(memoryFull);
    }

    public void ApplyMemory(long memoryId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
