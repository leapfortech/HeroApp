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

    [Space]
    [Title("Images")]
    [SerializeField]
    int maxCount = 4;
    [SerializeField]
    String spriteName = "News";
    [SerializeField]
    ListScroller lstImage = null;
    [SerializeField]
    Text txtEmpty;

    [Title("Action")]
    [SerializeField]
    Button btnAddImage = null;

    [SerializeField]
    Button btnRegister = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    NewsService newsService = null;
    List<Texture2D> images = new List<Texture2D>();

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
        images.Clear();
        lstImage.Clear();
    }

    public void RefreshImages()
    {
        lstImage.Clear();

        for (int i = 0; i < images.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(1, true);
            scrollerValue.SetSprite(0, images[i].CreateSprite($"{spriteName}_{i}"));
            lstImage.ApplyAddValue(scrollerValue);
        }

        if (images.Count > 0)
            txtEmpty.gameObject.SetActive(false);
        else
            txtEmpty.gameObject.SetActive(true);

        if (images.Count < maxCount)
            btnAddImage.gameObject.SetActive(true);
        else
            btnAddImage.gameObject.SetActive(false);
    }

    public void AddImage(Texture2D image)
    {
        images.Add(image);
        RefreshImages();
    }

    public void RemoveImage(int idx)
    {
        images.RemoveAt(idx);
        RefreshImages();
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
        // RM WIP Fill All Params

        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].CreateSprite($"{spriteName}_{i}").ToStrBase64(ImageType.JPG);

        newsService.Register(new RegisterNewsRequest(new RegisterPostRequest(post, null, new List<Link> { link }, strImages),
                                                     news));
    }

    public void ApplyNews(long newsId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
