using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;
using Leap.Data.Collections;

using Sirenix.OdinInspector;

public class TaleUpdateAction : MonoBehaviour
{
    [Serializable]
    class TaleFeedEvent : UnityEvent<TaleFeed> { }

    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmTaleFull = null;
    [SerializeField]
    ValueList vllImages = null;

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    [Title("Event")]
    [SerializeField]
    TaleFeedEvent onTaleChanged = null;
    [SerializeField]
    UnityEvent onPopulated = null;

    TaleService taleService = null;
    TaleFull taleFull = null;

    private void Awake()
    {
        taleService = GetComponent<TaleService>();
    }

    private void Start()
    {
        btnUpdate?.AddAction(DoUpdate);
    }

    public void Clear()
    {
        taleFull = null;

        dtmTaleFull.ClearElements();
        vllImages.ClearRecords();
    }

    public void ApplyFull(TaleFull taleFull)
    {
        Clear();

        this.taleFull = taleFull;

        dtmTaleFull.PopulateClass<TaleFull>(taleFull);

        for (int i = 0; i < taleFull.ImageSprites.Count; i++)
            vllImages.AddRecord(taleFull.ImageSprites[i].Clone($"Edt_{PostType.Names[PostType.Tale]}_{i}"));

        onPopulated.Invoke();
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        taleFull.Update(dtmTaleFull.BuildClass<TaleFull>());

        String[] strImages = new String[vllImages.RecordCount];
        for (int i = 0; i < vllImages.RecordCount; i++)
            strImages[i] = vllImages[i].GetCellSprite(0).ToStrBase64(ImageType.JPG);

        taleFull.Images = strImages;

        taleFull.ImageCount = vllImages.RecordCount;
        taleFull.TitleSprite = vllImages.RecordCount == 0 ? null : vllImages[0].GetCellSprite(0);

        taleService.UpdateTale(taleFull);
    }

    public void ApplyUpdate(bool updated)
    {
        if (!updated)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        onTaleChanged.Invoke(new TaleFeed(taleFull));

        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
