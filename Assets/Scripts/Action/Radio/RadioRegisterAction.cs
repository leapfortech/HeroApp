using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;

using Sirenix.OdinInspector;

public class RadioRegisterAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmRadioFull = null;
    [SerializeField]
    DataMapper dtmRadioTypeFullVLL = null;
    [SerializeField]
    DataMapper dtmRadioLanguageFullVLL = null;
    [SerializeField]
    DataMapper dtmLinkFull = null;
    [SerializeField]
    DataMapper dtmImagesVLL = null;

    [Title("Action")]
    [SerializeField]
    Button btnRegister = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    RadioService radioService = null;

    private void Awake()
    {
        radioService = GetComponent<RadioService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(Register);
    }

    public void Clear()
    {
        dtmRadioFull.ClearElements();
        dtmRadioTypeFullVLL.ClearElements();
        dtmRadioLanguageFullVLL.ClearElements();
        dtmLinkFull.ClearElements();
        dtmImagesVLL.ClearElements();
    }

    private void Register()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        RadioFull radioFull = dtmRadioFull.BuildClass<RadioFull>();
        radioFull.AppUserId = StateManager.Instance.AppUser.Id;
        radioFull.PostCountryId = StateManager.Instance.InterestLocality.CountryId;
        radioFull.PostStateId = StateManager.Instance.InterestLocality.StateId;

        radioFull.LinkFulls = new List<LinkFull>() { dtmLinkFull.BuildClass<LinkFull>() };
        radioFull.LinkFulls[0].LinkTypeId = (long)LinkType.Url;

        radioFull.RadioTypeFulls = dtmRadioTypeFullVLL.BuildClassList<RadioTypeFull>();
        radioFull.RadioLanguageFulls = dtmRadioLanguageFullVLL.BuildClassList<RadioLanguageFull>();

        radioFull.ImageSprites = dtmImagesVLL.BuildBuiltInList<Sprite>();

        radioService.Register(radioFull);
    }

    public void ApplyRadio(long radioId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
