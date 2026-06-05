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
    DataMapper dtmPost = null;
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
        dtmPost.ClearElements();
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

        Post post = dtmPost.BuildClass<Post>();
        post.AppUserId = StateManager.Instance.AppUser.Id;

        //RM REVIEW
        post.CountryId = StateManager.Instance.InterestLocality.CountryId;
        post.StateId = StateManager.Instance.InterestLocality.StateId;

        Link link = dtmLink.BuildClass<Link>();
        link.LinkTypeId = (long)LinkType.Url;

        List<RadioType> radioTypes = dtmRadioTypeVLL.BuildClassList<RadioType>();
        List<RadioLanguage> radioLanguages = dtmRadioLanguageVLL.BuildClassList<RadioLanguage>();

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        radioService.Register(new RegisterRadioRequest(post, new List<Link> { link }, strImages, null, radioTypes, radioLanguages));
    }

    public void ApplyRadio(long radioId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
