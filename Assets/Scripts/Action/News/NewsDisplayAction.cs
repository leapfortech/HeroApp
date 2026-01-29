using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Core.Tools;

using Sirenix.OdinInspector;


public class NewsDisplayAction : MonoBehaviour
{
    [Space]
    [Title("Details")]
    [SerializeField]
    Text txtTitle = null;

    [Space]
    [Title("Images")]
    [SerializeField]
    ListScroller lstImage = null;

    //[Title("Values")]
    //[SerializeField]
    //ValueList vllProjectDescriptionType = null;
    [Title("Event")]
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

    public void Clear()
    {
        
    }

    // Display

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

        lstImage.Clear();

        for (int i = 0; i < images.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(1, true);
            scrollerValue.SetSprite(0, images[i]);
            lstImage.ApplyAddValue(scrollerValue);
        }

        PageManager.Instance.ChangePage(pagDetail);

        onDisplayed.Invoke(new long[2] {newsFull.PostId, newsFull.Id});
    }
}