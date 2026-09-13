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
    [SerializeField]
    String spriteName = "News";

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

    NewsService newsService = null;

    News news = null;

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

        PostHelper.post = new Post(newsFull);
        dtmPost.PopulateClass<Post>(PostHelper.post);

        news = new News(newsFull);
        dtmNews.PopulateClass<News>(news);

        //String dateTimeStr = news.DateTime.Value.ToString("HH|mm", CultureInfo.InvariantCulture);
        //dtmTime.PopulateBuiltIn<String>(dateTimeStr);

        for (int i = 0; i < newsFull.ImageSprites.Count; i++)
            vllImages.AddRecord(newsFull.ImageSprites[i].Clone($"Edt_{spriteName}_{i}"));

        onPopulated.Invoke();
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        PostHelper.post.Update(dtmPost.BuildClass<Post>());

        news.Update(dtmNews.BuildClass<News>());

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

        PostHelper.post.ImageCount = vllImages.RecordCount;
        PostHelper.titleSprite = vllImages.RecordCount == 0 ? null : vllImages[0].GetCellSprite(0);

        newsService.UpdateNews(new RegisterNewsRequest(PostHelper.post, (Link)null, strImages, news));
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
