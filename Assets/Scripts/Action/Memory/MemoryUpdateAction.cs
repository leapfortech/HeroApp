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
    [SerializeField]
    String spriteName = "Memory";

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    [Title("Event")]
    [SerializeField]
    PostSpriteEvent onPostChanged = null;
    [SerializeField]
    UnityEvent onPopulated = null;

    MemoryService memoryService = null;

    Memory memory = null;

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

        PostHelper.post = new Post(memoryFull);
        dtmPost.PopulateClass<Post>(PostHelper.post);


        memory = new Memory(memoryFull);
        dtmMemory.PopulateClass<Memory>(memory);
        String timeStr = memory.DateTime.Value.ToString("HH|mm", CultureInfo.InvariantCulture);
        dtmTime.PopulateBuiltIn<String>(timeStr);

        for (int i = 0; i < memoryFull.ImageSprites.Count; i++)
            vllImages.AddRecord(memoryFull.ImageSprites[i].Clone($"Edt_{spriteName}_{i}"));

        onPopulated.Invoke();
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        PostHelper.post.Update(dtmPost.BuildClass<Post>());

        memory.Update(dtmMemory.BuildClass<Memory>());

        if (memory.DateTime.HasValue)
        {
            String[] time = dtmTime.BuildBuiltIn<String>().Split('|');
            memory.DateTime = new DateTime(memory.DateTime.Value.Year, memory.DateTime.Value.Month, memory.DateTime.Value.Day,
                                           Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
        }

        String[] strImages = new String[vllImages.RecordCount];
        for (int i = 0; i < vllImages.RecordCount; i++)
            strImages[i] = vllImages[i].GetCellSprite(0).ToStrBase64(ImageType.JPG);

        PostHelper.post.ImageCount = vllImages.RecordCount;
        PostHelper.titleSprite = vllImages.RecordCount == 0 ? null : vllImages[0].GetCellSprite(0);

        memoryService.UpdateMemory(new RegisterMemoryRequest(PostHelper.post, strImages, memory));
    }

    public void ApplyMemory(bool updated)
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
