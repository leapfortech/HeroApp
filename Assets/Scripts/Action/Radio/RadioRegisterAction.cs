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
    DataMapper dtmRadioTypeVLL = null;
    [SerializeField]
    DataMapper dtmRadioLanguageVLL = null;
    [SerializeField]
    DataMapper dtmLink = null;
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
        dtmRadioTypeVLL.ClearElements();
        dtmRadioLanguageVLL.ClearElements();
        dtmLink.ClearElements();
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

        radioFull.LinkFulls = new List<LinkFull>() { dtmLink.BuildClass<LinkFull>() };
        radioFull.LinkFulls[0].LinkTypeId = (long)LinkType.Url;

        radioFull.RadioTypeFulls = dtmRadioTypeVLL.BuildClassList<RadioTypeFull>();
        radioFull.RadioLanguageFulls = dtmRadioLanguageVLL.BuildClassList<RadioLanguageFull>();

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        radioFull.Images = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            radioFull.Images[i] = images[i].ToStrBase64(ImageType.JPG);

        radioService.Register(radioFull);
    }

    public void ApplyRadio(long radioId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
