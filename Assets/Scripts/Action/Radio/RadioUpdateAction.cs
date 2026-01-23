using System;
using System.Collections.Generic;
using UnityEngine;

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

    RadioService radioService = null;

    RadioFull radioFull = null;
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

    public void SetRadioFull(RadioFull radioFull)
    {
        this.radioFull = radioFull;
    }

    public void Populate()
    {
        post = new Post(radioFull);
        dtmPost.PopulateClass<Post>(post);

        dtmLink.PopulateBuiltIn<String>(new Link(radioFull.LinkFulls[0]).Url);

        radio = new Radio(radioFull);

        List<RadioType> radioTypes = new List<RadioType>();
        for (int i = 0; i < radioFull.RadioTypeFulls.Count; i++)
            radioTypes.Add(new RadioType(radioFull.Id, radioFull.RadioTypeFulls[i]));
        dtmRadioTypeVLL.PopulateClassList<RadioType>(radioTypes);

        List<RadioLanguage> radioLanguages = new List<RadioLanguage>();
        for (int i = 0; i < radioFull.RadioLanguageFulls.Count; i++)
            radioLanguages.Add(new RadioLanguage(radioFull.Id, radioFull.RadioLanguageFulls[i]));
        dtmRadioLanguageVLL.PopulateClassList<RadioLanguage>(radioLanguages);

        //List<Sprite> images = StateManager.Instance.GetRadioImagesById(treatmentFull.Id);
        //dtmImagesVLL.PopulateBuiltInList<Sprite>(images);
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

        radioService.Register(new RegisterRadioRequest(new RegisterPostRequest(post, null, new List<Link> { linkNew }, strImages),
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
