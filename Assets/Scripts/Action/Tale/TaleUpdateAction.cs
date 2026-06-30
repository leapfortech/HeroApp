using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;

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
    DataMapper dtmImagesVLL = null;

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    [Title("Event")]
    [SerializeField]
    PostSpriteEvent onPostChanged = null;

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
        dtmImagesVLL.ClearElements();
    }

    public void ApplyFull(TaleFull taleFull)
    {
        PostHelper.post = new Post(taleFull);
        dtmPost.PopulateClass<Post>(PostHelper.post);

        tale = new Tale(taleFull);

        dtmImagesVLL.PopulateBuiltInList<Sprite>(taleFull.ImageSprites);
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        PostHelper.post.Description = dtmPost.BuildClass<Post>().Description;
        
        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        PostHelper.post.ImageCount = images.Count;
        PostHelper.titleSprite = images.Count == 0 ? null : images[0];

        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

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
