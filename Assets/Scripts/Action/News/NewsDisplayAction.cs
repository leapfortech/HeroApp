using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Core.Tools;

using Sirenix.OdinInspector;


public class NewsDisplayAction : MonoBehaviour
{
    [Serializable]
    public class ImagesEvent : UnityEvent<List<Sprite>> { }
    [Space]
    [Title("Details")]
    [SerializeField]
    Text txtTitle = null;

    [Title("Event")]
    [SerializeField]
    ImagesEvent onImagesDisplay = null;
    [SerializeField]
    UnityLongsEvent onDisplayed = null;

    [Title("Page")]
    [SerializeField]
    Page pagDetail;

    NewsService newsService;

    long postId = -1, newsId = -1;

    private void Awake()
    {
        newsService = GetComponent<NewsService>();
    }

    public void Display(long postId)
    {
        this.postId = postId;

        NewsFull newsFull = StateManager.Instance.GetNewsFullByPostId(postId);
        if (newsFull != null)
        {
            newsId = newsFull.Id;
            Display(newsFull);
            return;
        }

        ScreenDialog.Instance.Display();
        newsService.GetFullByPostId(postId);
    }

    public void ApplyFull(NewsFull newsFull)
    {
        newsId = newsFull.Id;
        StateManager.Instance.AddNewsFull(newsFull);
        StateManager.Instance.AddNewsImages(newsFull.Id, newsFull.Images);
        Display(newsFull);
    }

    private void Display(NewsFull newsFull)
    {       
        if (newsFull == null)
            return;

        txtTitle.TextValue = newsFull.Title;

        List<Sprite> images = StateManager.Instance.GetNewsImagesById(newsId);

        onImagesDisplay.Invoke(images);
        onDisplayed.Invoke(new long[2] {newsFull.PostId, newsFull.Id});

        PageManager.Instance.ChangePage(pagDetail);
    }
}