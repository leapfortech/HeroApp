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
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmPost = null;
    [SerializeField]
    ValueList vllImages = null;
    [SerializeField]
    String spriteName = "Tale";

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

    TaleService taleService = null;

    Tale tale = null;

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
        dtmPost.ClearElements();
        vllImages.ClearRecords();
    }

    public void ApplyFull(TaleFull taleFull)
    {
        Clear();

        PostHelper.post = new Post(taleFull);
        dtmPost.PopulateClass<Post>(PostHelper.post);

        tale = new Tale(taleFull);

        for (int i = 0; i < taleFull.ImageSprites.Count; i++)
            vllImages.AddRecord(taleFull.ImageSprites[i].Clone($"Edt_{spriteName}_{i}"));

        onPopulated.Invoke();
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        PostHelper.post.Update(dtmPost.BuildClass<Post>());

        String[] strImages = new String[vllImages.RecordCount];
        for (int i = 0; i < vllImages.RecordCount; i++)
            strImages[i] = vllImages[i].GetCellSprite(0).ToStrBase64(ImageType.JPG);

        PostHelper.post.ImageCount = vllImages.RecordCount;
        PostHelper.titleSprite = vllImages.RecordCount == 0 ? null : vllImages[0].GetCellSprite(0);

        taleService.UpdateTale(new RegisterTaleRequest(PostHelper.post, strImages, tale));
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
