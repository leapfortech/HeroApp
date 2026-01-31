using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;

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
    DataMapper dtmLink = null;
    [SerializeField]
    DataMapper dtmNews = null;
    [SerializeField]
    DataMapper dtmTime = null;
    [SerializeField]
    DataMapper dtmImagesVLL = null;

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    NewsService newsService = null;

    long postId = -1, newsId = -1;
    Post post = null;
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
        dtmLink.ClearElements();
        dtmNews.ClearElements();
        dtmImagesVLL.ClearElements();
    }

    public void SetIds(long[] ids)
    {
        postId = ids[0];
        newsId = ids[1];
    }

    public void Populate()
    {
        NewsFull newsFull = StateManager.Instance.GetNewsFullById(newsId);

        post = new Post(newsFull);
        dtmPost.PopulateClass<Post>(post);

        dtmLink.PopulateClass<Link>(new Link(newsFull.LinkFulls[0]));

        news = new News(newsFull);
        dtmNews.PopulateClass<News>(news);

        String dateTimeStr = news.DateTime.Value.ToString("HH|mm", CultureInfo.InvariantCulture);
        dtmTime.PopulateBuiltIn<String>(dateTimeStr);

        List<Sprite> images = StateManager.Instance.GetNewsImagesById(newsId);
        dtmImagesVLL.PopulateBuiltInList<Sprite>(images);
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        post.Update(dtmPost.BuildClass<Post>());

        Link link = dtmLink.BuildClass<Link>();
        link.LinkTypeId = (long)LinkType.Url;

        news.Update(dtmNews.BuildClass<News>());

        if (news.DateTime.HasValue && news.DateTime.HasValue)
        {
            String startTimeStr = dtmTime.BuildBuiltIn<String>();
            String[] startTime = startTimeStr.Split('|');
            news.DateTime = new DateTime(news.DateTime.Value.Year, news.DateTime.Value.Month, news.DateTime.Value.Day,
                                         Convert.ToInt32(startTime[0]), Convert.ToInt32(startTime[1]), 0);
        }

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        newsService.UpdateNews(new RegisterNewsRequest(new RegisterPostRequest(post, null, new List<Link> { link }, strImages),
                               news));
    }

    public void ApplyUpdate(bool updated)
    {
        if (!updated)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
