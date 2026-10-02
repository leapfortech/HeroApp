using System;
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

public class NewsUpdateAction : MonoBehaviour
{
    [Serializable]
    class NewsFeedEvent : UnityEvent<NewsFeed> { }

    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmNewsFull = null;
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
    NewsFeedEvent onNewsChanged = null;
    [SerializeField]
    UnityEvent onPopulated = null;

    NewsService newsService = null;

    NewsFull newsFull = null;

    private void Awake()
    {
        newsService = GetComponent<NewsService>();
    }

    private void Start()
    {
        btnUpdate?.AddAction(DoUpdate);
    }

    public void Clear()
    {
        newsFull = null;

        dtmNewsFull.ClearElements();
        vllImages.ClearRecords();
    }

    public void ApplyFull(NewsFull newsFull)
    {
        Clear();

        this.newsFull = newsFull;
        dtmNewsFull.PopulateClass<PostFull>(newsFull);

        for (int i = 0; i < newsFull.ImageSprites.Count; i++)
            vllImages.AddRecord(newsFull.ImageSprites[i].Clone($"Edt_{PostType.Names[PostType.News]}_{i}"));

        onPopulated.Invoke();
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        newsFull.Update(dtmNewsFull.BuildClass<NewsFull>());

        String[] strImages = new String[vllImages.RecordCount];
        for (int i = 0; i < vllImages.RecordCount; i++)
            strImages[i] = vllImages[i].GetCellSprite(0).ToStrBase64(ImageType.JPG);

        newsFull.ImageCount = vllImages.RecordCount;
        newsFull.TitleSprite = vllImages.RecordCount == 0 ? null : vllImages[0].GetCellSprite(0);

        newsService.UpdateNews(newsFull);
    }

    public void ApplyUpdate(bool updated)
    {
        if (!updated)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        onNewsChanged.Invoke(new NewsFeed(newsFull));

        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
