using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Extensions;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;

public class TaleRegisterAction : MonoBehaviour
{
    [Title("Test Images")]
    [SerializeField]
    private List<Sprite> testImages;

    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmTaleFull = null;
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

    TaleService taleService = null;

    private void Awake()
    {
        taleService = GetComponent<TaleService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(Register);
    }

    public void Clear()
    {
        dtmTaleFull.ClearElements();
        dtmImagesVLL.ClearElements();
        FillCombos();
    }

    public void FillCombos()
    {
        cmbCountry.Select(StateManager.Instance.CurrentLocality.CountryId);
        cmbState.Select(StateManager.Instance.CurrentLocality.StateId);
    }

    private void Register()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        TaleFull taleFull = dtmTaleFull.BuildClass<TaleFull>();
        taleFull.AppUserId = StateManager.Instance.AppUser.Id;

        taleFull.ImageSprites = dtmImagesVLL.BuildBuiltInList<Sprite>();

        taleService.Register(taleFull);
    }

    public void ApplyTale(long taleId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }

    // Locality

    bool interestLocality = true;

    public void ApplyLocality(bool interestLocality)
    {
        this.interestLocality = interestLocality;
    }
}
