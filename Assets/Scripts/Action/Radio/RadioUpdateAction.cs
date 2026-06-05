using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;
using Leap.Core.Tools;

using Sirenix.OdinInspector;

public class RadioUpdateAction : MonoBehaviour
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
    Button btnUpdate = null;

    [Title("Event")]
    [SerializeField]
    UnityLongsEvent OnRadioTypePopulated = null;
    [SerializeField]
    UnityLongsEvent OnRadioLanguagePopulated = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    RadioService radioService = null;

    Post post = null;
    Radio radio = null;

    private void Awake()
    {
        radioService = GetComponent<RadioService>();
    }

    private void Start()
    {
        btnUpdate?.AddAction(DoUpdate);
    }

    public void Clear()
    {
        dtmPost.ClearElements();
        dtmRadioTypeVLL.ClearElements();
        dtmRadioLanguageVLL.ClearElements();
        dtmLink.ClearElements();
        dtmImagesVLL.ClearElements();
    }

    public void Display(RadioFull radioFull)
    {
        post = new Post(radioFull);
        dtmPost.PopulateClass<Post>(post);

        dtmLink.PopulateClass<Link>(new Link(radioFull.LinkFulls[0]));

        radio = new Radio(radioFull);

        long[] radioTypesIds = new long[radioFull.RadioTypeFulls.Count];
        for (int i = 0; i < radioFull.RadioTypeFulls.Count; i++)
            radioTypesIds[i] = radioFull.RadioTypeFulls[i].RadioTypeId;
        OnRadioTypePopulated?.Invoke(radioTypesIds);


        long[] radioLanguageIds = new long[radioFull.RadioLanguageFulls.Count];
        for (int i = 0; i < radioFull.RadioLanguageFulls.Count; i++)
            radioLanguageIds[i] = radioFull.RadioLanguageFulls[i].LanguageId;
        OnRadioLanguagePopulated?.Invoke(radioLanguageIds);

        List<Sprite> images = new List<Sprite>();
        for (int i = 0; i < radioFull.Images.Length; i++)
            images.Add(radioFull.Images[i].CreateSprite($"RadioImage_{i}"));
        dtmImagesVLL.PopulateBuiltInList<Sprite>(images);
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        post.Update(dtmPost.BuildClass<Post>());

        Link link = dtmLink.BuildClass<Link>();
        link.LinkTypeId = (long)LinkType.Url;

        List<RadioType> radioTypes = dtmRadioTypeVLL.BuildClassList<RadioType>();
        List<RadioLanguage> radioLanguages = dtmRadioLanguageVLL.BuildClassList<RadioLanguage>();

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        radioService.UpdateRadio(new RegisterRadioRequest(post, new List<Link> { link }, strImages, radio, radioTypes, radioLanguages));
    }

    public void ApplyUpdate(bool updated)
    {
        if (!updated)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
