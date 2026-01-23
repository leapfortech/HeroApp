using System;
using System.Collections.Generic;
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
        NewsFull newsFull = null; // StateManager.Instance.GetNewsFullById(newsId);

        post = new Post(newsFull);
        dtmPost.PopulateClass<Post>(post);

        dtmLink.PopulateBuiltIn<String>(new Link(newsFull.LinkFulls[0]).Url);

        news = new News(newsFull);
        dtmNews.PopulateClass<News>(news);
        dtmTime.PopulateBuiltIn<String>(news.DateTime != null ? news.DateTime.Value.ToString("HH|mm") : null);

        //List<Sprite> images = StateManager.Instance.GetNewsImagesById(newsId);
        //dtmImagesVLL.PopulateBuiltInList<Sprite>(images);
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        Post postNew = dtmPost.BuildClass<Post>();
        post.Title = postNew.Title;
        post.Summary = postNew.Summary;
        post.Description = postNew.Description;

        Link linkNew = dtmLink.BuildClass<Link>();
        linkNew.LinkTypeId = (long)LinkType.Url;

        News newsNew = dtmNews.BuildClass<News>();
        news.NewsTypeId = newsNew.NewsTypeId;
        news.Place = newsNew.Place;
        news.Source = newsNew.Source;

        if (newsNew.DateTime.HasValue && newsNew.DateTime.HasValue)
        {
            String startTimeStr = dtmTime.BuildBuiltIn<String>();
            String[] startTime = startTimeStr.Split('|');
            news.DateTime = new DateTime(newsNew.DateTime.Value.Year, newsNew.DateTime.Value.Month, newsNew.DateTime.Value.Day,
                                         Convert.ToInt32(startTime[0]), Convert.ToInt32(startTime[1]), 0);
        }

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        newsService.UpdateNews(new RegisterNewsRequest(new RegisterPostRequest(post, null, new List<Link> { linkNew }, strImages),
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
