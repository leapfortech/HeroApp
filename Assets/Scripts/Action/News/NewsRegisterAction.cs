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
    [Title("Test Images")]
    [SerializeField]
    private List<Sprite> testImages;

    [Space]
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmNewsFull = null;
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
        dtmNewsFull.ClearElements();
        dtmImagesVLL.ClearElements();
    }

    private void Register()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        NewsFull newsFull = dtmNewsFull.BuildClass<NewsFull>();
        newsFull.AppUserId = StateManager.Instance.AppUser.Id;
        newsFull.PostCountryId =  StateManager.Instance.InterestLocality.CountryId;
        newsFull.PostStateId = StateManager.Instance.InterestLocality.StateId;

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        newsFull.Images = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            newsFull.Images[i] = images[i].ToStrBase64(ImageType.JPG);

        newsService.Register(newsFull);
    }

    public void ApplyNews(long newsId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
