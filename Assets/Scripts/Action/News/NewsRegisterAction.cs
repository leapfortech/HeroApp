using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;

using Sirenix.OdinInspector;

public class NewsRegisterAction : MonoBehaviour
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
    Button btnRegister = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    NewsService newsService = null;

    private void Awake()
    {
        newsService = GetComponent<NewsService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(Register);
    }

    public void Clear()
    {
        dtmPost.ClearElements();
        dtmLink.ClearElements();
        dtmNews.ClearElements();
        dtmImagesVLL.ClearElements();
    }

    private void Register()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        Post post = dtmPost.BuildClass<Post>();
        post.AppUserId = StateManager.Instance.AppUser.Id;
        post.CountryId = StateManager.Instance.Identity.OriginCountryId;
        post.StateId = StateManager.Instance.Identity.OriginStateId;

        Link link = dtmLink.BuildClass<Link>();
        link.LinkTypeId = (long)LinkType.Url;

        News news = dtmNews.BuildClass<News>();

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

        newsService.Register(new RegisterNewsRequest(new RegisterPostRequest(post, null, new List<Link> { link }, strImages),
                                                     news));
    }

    public void ApplyNews(long newsId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
