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

    long postId = -1, radioId = -1;
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

    public void SetIds(long[] ids)
    {
        postId = ids[0];
        radioId = ids[1];
    }

    public void Populate()
    {
        RadioFull radioFull = StateManager.Instance.GetRadioFullById(radioId);

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

        List<Sprite> images = StateManager.Instance.GetRadioImagesById(radioId);
        dtmImagesVLL.PopulateBuiltInList<Sprite>(images);
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        Post postNew = dtmPost.BuildClass<Post>();
        post.Title = postNew.Title;
        post.Summary = postNew.Summary;
        post.Description = postNew.Description;

        Link linkNew = dtmLink.BuildClass<Link>();
        linkNew.LinkTypeId = (long)LinkType.Url;

        List<RadioType> radioTypesNew = dtmRadioTypeVLL.BuildClassList<RadioType>();
        List<RadioLanguage> radioLanguagesNew = dtmRadioLanguageVLL.BuildClassList<RadioLanguage>();

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        radioService.UpdateRadio(new RegisterRadioRequest(new RegisterPostRequest(post, null, new List<Link> { linkNew }, strImages),
                                                       radio, radioTypesNew, radioLanguagesNew));
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
