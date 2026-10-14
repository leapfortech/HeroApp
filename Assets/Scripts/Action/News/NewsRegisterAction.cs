using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Extensions;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;

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
    [SerializeField]
    ComboAdapter cmbCountry = null;
    [SerializeField]
    ComboAdapter cmbState = null;

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
        FillCombos();
    }

    public void FillCombos()
    {
        cmbCountry.Select(StateManager.Instance.InterestLocality.CountryId);
        cmbState.Select(StateManager.Instance.InterestLocality.StateId);
    }

    private void Register()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        NewsFull newsFull = dtmNewsFull.BuildClass<NewsFull>();
        newsFull.AppUserId = StateManager.Instance.AppUser.Id;

        newsFull.ImageSprites = dtmImagesVLL.BuildBuiltInList<Sprite>();

        newsService.Register(newsFull);
    }

    public void ApplyNews(long newsId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
