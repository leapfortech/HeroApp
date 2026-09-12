using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.Core.Tools;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;

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

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    [Title("Events")]
    [SerializeField]
    UnityLongsEvent onRadioTypePopulated = null;
    [SerializeField]
    UnityLongsEvent onRadioLanguagePopulated = null;
    [SerializeField]
    PostSpriteEvent onPostChanged = null;
    [SerializeField]
    UnityEvent onPopulated = null;

    RadioService radioService = null;

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

    public void ApplyFull(RadioFull radioFull)
    {
        Clear();

        PostHelper.post = new Post(radioFull);
        dtmPost.PopulateClass<Post>(PostHelper.post);

        dtmLink.PopulateClass<Link>(new Link(radioFull.LinkFulls[0]));

        radio = new Radio(radioFull);

        long[] radioTypesIds = new long[radioFull.RadioTypeFulls.Count];
        for (int i = 0; i < radioFull.RadioTypeFulls.Count; i++)
            radioTypesIds[i] = radioFull.RadioTypeFulls[i].RadioTypeId;
        onRadioTypePopulated?.Invoke(radioTypesIds);


        long[] radioLanguageIds = new long[radioFull.RadioLanguageFulls.Count];
        for (int i = 0; i < radioFull.RadioLanguageFulls.Count; i++)
            radioLanguageIds[i] = radioFull.RadioLanguageFulls[i].LanguageId;
        onRadioLanguagePopulated?.Invoke(radioLanguageIds);

        dtmImagesVLL.PopulateBuiltInList<Sprite>(radioFull.ImageSprites);

        onPopulated.Invoke();
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        PostHelper.post.Update(dtmPost.BuildClass<Post>());

        Link link = dtmLink.BuildClass<Link>();
        link.LinkTypeId = (long)LinkType.Url;

        List<RadioType> radioTypes = dtmRadioTypeVLL.BuildClassList<RadioType>();
        List<RadioLanguage> radioLanguages = dtmRadioLanguageVLL.BuildClassList<RadioLanguage>();

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        PostHelper.post.ImageCount = images.Count;
        PostHelper.titleSprite = images.Count == 0 ? null : images[0];

        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        radioService.UpdateRadio(new RegisterRadioRequest(PostHelper.post, new List<Link> { link }, strImages, radio, radioTypes, radioLanguages));
    }

    public void ApplyUpdate(bool updated)
    {
        if (!updated)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        onPostChanged.Invoke(PostHelper.post, PostHelper.titleSprite);

        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
