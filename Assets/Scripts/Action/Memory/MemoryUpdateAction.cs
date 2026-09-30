using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;
using Leap.Data.Collections;

using Sirenix.OdinInspector;

public class MemoryUpdateAction : MonoBehaviour
{
    [Serializable]
    class MemoryFeedEvent : UnityEvent<MemoryFeed> { }

    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmPost = null;

    [SerializeField]
    DataMapper dtmMemory = null;
    [SerializeField]
    DataMapper dtmTime = null;

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
    MemoryFeedEvent onMemoryChanged = null;
    [SerializeField]
    UnityEvent onPopulated = null;

    MemoryService memoryService = null;
    MemoryFull memoryFull = null;

    private void Awake()
    {
        memoryService = GetComponent<MemoryService>();
    }

    private void Start()
    {
        btnUpdate?.AddAction(DoUpdate);
    }

    public void Clear()
    {
        dtmPost.ClearElements();

        dtmMemory.ClearElements();
        dtmTime.ClearElements();

        vllImages.ClearRecords();
    }

    public void ApplyFull(MemoryFull memoryFull)
    {
        Clear();

        this.memoryFull = memoryFull;

        dtmMemory.PopulateClass<MemoryFull>(memoryFull);

        String timeStr = memoryFull.DateTime.Value.ToString("HH|mm", CultureInfo.InvariantCulture);
        dtmTime.PopulateBuiltIn<String>(timeStr);

        for (int i = 0; i < memoryFull.ImageSprites.Count; i++)
            vllImages.AddRecord(memoryFull.ImageSprites[i].Clone($"Edt_{PostType.Names[PostType.Memory]}_{i}"));

        onPopulated.Invoke();
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        memoryFull.Update(dtmMemory.BuildClass<MemoryFull>());

        if (memoryFull.DateTime.HasValue)
        {
            String[] time = dtmTime.BuildBuiltIn<String>().Split('|');
            memoryFull.DateTime = new DateTime(memoryFull.DateTime.Value.Year, memoryFull.DateTime.Value.Month, memoryFull.DateTime.Value.Day,
                                               Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
        }

        String[] strImages = new String[vllImages.RecordCount];
        for (int i = 0; i < vllImages.RecordCount; i++)
            strImages[i] = vllImages[i].GetCellSprite(0).ToStrBase64(ImageType.JPG);

        memoryFull.ImageCount = vllImages.RecordCount;
        memoryFull.TitleSprite = vllImages.RecordCount == 0 ? null : vllImages[0].GetCellSprite(0);

        memoryService.UpdateMemory(memoryFull);
    }

    public void ApplyMemory(bool updated)
    {
        if (!updated)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        onMemoryChanged.Invoke(new MemoryFeed(memoryFull));

        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
