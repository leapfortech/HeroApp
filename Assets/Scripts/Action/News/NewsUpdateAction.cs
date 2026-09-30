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

public class NewsUpdateAction : MonoBehaviour
{
    [Serializable]
    class NewsFeedEvent : UnityEvent<NewsFeed> { }

    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmPost = null;
    [SerializeField]
    DataMapper dtmNews = null;
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
        dtmPost.ClearElements();
        dtmNews.ClearElements();
        vllImages.ClearRecords();
    }

    public void ApplyFull(NewsFull newsFull)
    {
        Clear();

        dtmPost.PopulateClass<PostFull>(newsFull);

        this.newsFull = newsFull;
        dtmNews.PopulateClass<NewsFull>(newsFull);

        //String dateTimeStr = news.DateTime.Value.ToString("HH|mm", CultureInfo.InvariantCulture);
        //dtmTime.PopulateBuiltIn<String>(dateTimeStr);

        for (int i = 0; i < newsFull.ImageSprites.Count; i++)
            vllImages.AddRecord(newsFull.ImageSprites[i].Clone($"Edt_{PostType.Names[PostType.News]}_{i}"));

        onPopulated.Invoke();
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        newsFull.Update(dtmNews.BuildClass<NewsFull>());

        //if (news.DateTime.HasValue && news.DateTime.HasValue)
        //{
        //    String startTimeStr = dtmTime.BuildBuiltIn<String>();
        //    String[] startTime = startTimeStr.Split('|');
        //    news.DateTime = new DateTime(news.DateTime.Value.Year, news.DateTime.Value.Month, news.DateTime.Value.Day,
        //                                 Convert.ToInt32(startTime[0]), Convert.ToInt32(startTime[1]), 0);
        //}

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
