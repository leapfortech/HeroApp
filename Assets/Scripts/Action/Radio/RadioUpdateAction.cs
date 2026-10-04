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
    DataMapper dtmRadioFull = null;
    [SerializeField]
    DataMapper dtmRadioTypeFullVLL = null;
    [SerializeField]
    DataMapper dtmRadioLanguageFullVLL = null;
    [SerializeField]
    DataMapper dtmLinkFull = null;
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
        radioFull = null;

        dtmRadioFull.ClearElements();
        dtmRadioTypeFullVLL.ClearElements();
        dtmRadioLanguageFullVLL.ClearElements();
        dtmLinkFull.ClearElements();
        vllImages.ClearRecords();
    }

    public void ApplyFull(RadioFull radioFull)
    {
        Clear();

        this.radioFull = radioFull;

        dtmRadioFull.PopulateClass<RadioFull>(radioFull);

        long[] radioTypesIds = new long[radioFull.RadioTypeFulls.Count];
        for (int i = 0; i < radioFull.RadioTypeFulls.Count; i++)
            radioTypesIds[i] = radioFull.RadioTypeFulls[i].RadioTypeId;
        onRadioTypePopulated?.Invoke(radioTypesIds);

        long[] radioLanguageIds = new long[radioFull.RadioLanguageFulls.Count];
        for (int i = 0; i < radioFull.RadioLanguageFulls.Count; i++)
            radioLanguageIds[i] = radioFull.RadioLanguageFulls[i].LanguageId;
        onRadioLanguagePopulated?.Invoke(radioLanguageIds);

        if (radioFull.LinkFulls != null)
            dtmLinkFull.PopulateClass<LinkFull>(radioFull.LinkFulls[0]);

        for (int i = 0; i < radioFull.ImageSprites.Count; i++)
            vllImages.AddRecord(radioFull.ImageSprites[i].Clone($"Edt_{PostType.Names[PostType.Radio]}_{i}"));

        onPopulated.Invoke();
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        radioFull.Update(dtmRadioFull.BuildClass<RadioFull>());

        LinkFull linkFull = dtmLinkFull.BuildClass<LinkFull>();
        linkFull.LinkTypeId = (long)LinkType.Url;

        List<RadioTypeFull> radioTypeFulls = dtmRadioTypeFullVLL.BuildClassList<RadioTypeFull>();
        List<RadioLanguageFull> radioLanguageFulls = dtmRadioLanguageFullVLL.BuildClassList<RadioLanguageFull>();
        radioFull.RadioTypeFulls = radioTypeFulls;
        radioFull.RadioLanguageFulls = radioLanguageFulls;

        String[] strImages = new String[vllImages.RecordCount];
        for (int i = 0; i < vllImages.RecordCount; i++)
            strImages[i] = vllImages[i].GetCellSprite(0).ToStrBase64(ImageType.JPG);
        radioFull.Images = strImages;
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
