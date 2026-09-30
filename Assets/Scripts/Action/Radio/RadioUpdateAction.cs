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
using Leap.Data.Collections;

using Sirenix.OdinInspector;

public class RadioUpdateAction : MonoBehaviour
{
    [Serializable]
    class RadioFeedEvent : UnityEvent<RadioFeed> { }

    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmRadio = null;
    [SerializeField]
    DataMapper dtmRadioTypeVLL = null;
    [SerializeField]
    DataMapper dtmRadioLanguageVLL = null;
    [SerializeField]
    DataMapper dtmLink = null;
    [SerializeField]
    ValueList vllImages = null;

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
    RadioFeedEvent onRadioChanged = null;
    [SerializeField]
    UnityEvent onPopulated = null;

    RadioService radioService = null;

    RadioFull radioFull = null;

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
        dtmRadio.ClearElements();
        dtmRadioTypeVLL.ClearElements();
        dtmRadioLanguageVLL.ClearElements();
        dtmLink.ClearElements();
        vllImages.ClearRecords();
    }

    public void ApplyFull(RadioFull radioFull)
    {
        Clear();

        this.radioFull = radioFull;

        dtmRadio.PopulateClass<RadioFull>(this.radioFull);
        dtmLink.PopulateClass<LinkFull>(radioFull.LinkFulls[0]);

        long[] radioTypesIds = new long[radioFull.RadioTypeFulls.Count];
        for (int i = 0; i < radioFull.RadioTypeFulls.Count; i++)
            radioTypesIds[i] = radioFull.RadioTypeFulls[i].RadioTypeId;
        onRadioTypePopulated?.Invoke(radioTypesIds);

        long[] radioLanguageIds = new long[radioFull.RadioLanguageFulls.Count];
        for (int i = 0; i < radioFull.RadioLanguageFulls.Count; i++)
            radioLanguageIds[i] = radioFull.RadioLanguageFulls[i].LanguageId;
        onRadioLanguagePopulated?.Invoke(radioLanguageIds);

        for (int i = 0; i < radioFull.ImageSprites.Count; i++)
            vllImages.AddRecord(radioFull.ImageSprites[i].Clone($"Edt_{PostType.Names[PostType.Radio]}_{i}"));

        onPopulated.Invoke();
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        radioFull.Update(dtmRadio.BuildClass<RadioFull>());

        LinkFull linkFull = dtmLink.BuildClass<LinkFull>();
        linkFull.LinkTypeId = (long)LinkType.Url;

        List<RadioType> radioTypes = dtmRadioTypeVLL.BuildClassList<RadioType>();
        List<RadioLanguage> radioLanguages = dtmRadioLanguageVLL.BuildClassList<RadioLanguage>();

        String[] strImages = new String[vllImages.RecordCount];
        for (int i = 0; i < vllImages.RecordCount; i++)
            strImages[i] = vllImages[i].GetCellSprite(0).ToStrBase64(ImageType.JPG);

        radioFull.ImageCount = vllImages.RecordCount;
        radioFull.TitleSprite = vllImages.RecordCount == 0 ? null : vllImages[0].GetCellSprite(0);

        radioService.UpdateRadio(radioFull);
    }

    public void ApplyUpdate(bool updated)
    {
        if (!updated)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        onRadioChanged.Invoke(new RadioFeed(radioFull));

        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
