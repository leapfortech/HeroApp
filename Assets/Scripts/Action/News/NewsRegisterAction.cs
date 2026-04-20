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
    [SerializeField]
    Button btnRegisterTest = null;


    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    NewsService newsService = null;

    private int testCounter = 0;
    private bool isTest = false;

    private void Awake()
    {
        newsService = GetComponent<NewsService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(Register);
        btnRegisterTest?.AddAction(RegisterTest);
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

        isTest = false;

        ScreenDialog.Instance.Display();

        Post post = dtmPost.BuildClass<Post>();
        post.AppUserId = StateManager.Instance.AppUser.Id;

        //RM REVIEW
        post.CountryId = StateManager.Instance.InterestLocality.CountryId;
        post.StateId = StateManager.Instance.InterestLocality.StateId;

        Link link = dtmLink.BuildClass<Link>();
        link.LinkTypeId = (long)LinkType.Url;

        News news = dtmNews.BuildClass<News>();

        if (news.DateTime.HasValue)
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

    private void RegisterTest()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        if (testCounter == 8)
        {
            testCounter = 0;
            ScreenDialog.Instance.Hide();
            return;
        }

        ScreenDialog.Instance.Display();

        isTest = true;

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();

        if (images.Count == 0)
        {
            ChoiceDialog.Instance.Error("Imágenes", "Debes agregar al menos una imagen.");
            return;
        }

        Post post = dtmPost.BuildClass<Post>();

        post.AppUserId = StateManager.Instance.AppUser.Id;
        post.CountryId = StateManager.Instance.InterestLocality.CountryId;
        post.StateId = StateManager.Instance.InterestLocality.StateId;

        testCounter++;

        post.Title = $"{post.Title} {testCounter}";
        post.Summary = $"{post.Summary} {testCounter}";
        post.Description = $"{post.Description} {testCounter}";

        Link link = dtmLink.BuildClass<Link>();
        link.LinkTypeId = (long)LinkType.Url;

        News news = dtmNews.BuildClass<News>();

        if (news.DateTime.HasValue)
        {
            String startTimeStr = dtmTime.BuildBuiltIn<String>();
            String[] startTime = startTimeStr.Split('|');
            news.DateTime = new DateTime(news.DateTime.Value.Year, news.DateTime.Value.Month, news.DateTime.Value.Day,
                                         Convert.ToInt32(startTime[0]), Convert.ToInt32(startTime[1]), 0);
        }

        Sprite selectedImage = images[(testCounter - 1) % images.Count];

        String[] strImages = new String[1];
        strImages[0] = selectedImage.ToStrBase64(ImageType.JPG);

        newsService.Register(new RegisterNewsRequest(new RegisterPostRequest(post, null, new List<Link> { link }, strImages),
                                                      news));
    }

    public void ApplyNews(long newsId)
    {
        if (!isTest)
        {
            Clear();
            PageManager.Instance.ChangePage(pagNext);
        }
        else
            RegisterTest();
    }
}
