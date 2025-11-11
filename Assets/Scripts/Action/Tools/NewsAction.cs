using UnityEngine;
using MPUIKIT;

using Leap.UI.Elements;
using Leap.Graphics.Tools;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;
using System.Collections.Generic;

public class NewsAction : MonoBehaviour
{
    [Space]
    [Title("Lists")]
    [SerializeField]
    ListScroller lstNews = null;

    [Header("Indicator")]
    [SerializeField]
    private GameObject indicatorPrefab;
    [SerializeField]
    private Transform indicatorParent;
    [SerializeField]
    private Color colorOn = Color.white;
    [SerializeField]
    private Color colorOff = Color.gray;

    [Header("Content")]
    [SerializeField]
    Text txtContent = null;

    private GameObject[] indicators;
    private int displayedMain = 0;
    private int displayedRegular = 0;

    NewsService newsService = null;
    List<NewsInfo> newsInfos = new List<NewsInfo>();


    private void Awake()
    {
        newsService = GetComponent<NewsService>();
    }

    public void Clear()
    {
        lstNews.Clear();
        foreach (Transform child in indicatorParent)
            Destroy(child.gameObject);
    }

    public void DisplayMain()
    {
        if (StateManager.Instance.NewsInfos == null || StateManager.Instance.NewsInfos.Count == 0)
        {
            Clear();
            displayedMain = 0;
            return;
        }

        if (displayedMain == 1)
            return;

        FillList(StateManager.Instance.NewsInfos);

        displayedMain = 1;
    }

    private void FillList(List<NewsInfo> newsInfos)
    {
        lstNews.Clear();

        CreateIndicators(newsInfos.Count);

        for (int i = 0; i < newsInfos.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(3, true);
            scrollerValue.SetText(0, newsInfos[i].News.Title);
            scrollerValue.SetText(1, newsInfos[i].News.Description);
            scrollerValue.SetSprite(2, newsInfos[i].Image != null && newsInfos[i].Image.Length != 0 ? newsInfos[i].Image.CreateSprite($"News_{i}") : null);
            lstNews.ApplyAddValue(scrollerValue);
        }
    }


    public void Selected(int idx)
    {
        if (txtContent == null)
            Application.OpenURL(StateManager.Instance.NewsInfos[idx].News.Link);
        else
            Application.OpenURL(newsInfos[idx].News.Link);
    }

    public void GetNews()
    {
        if (displayedRegular == 1)
            return;

        ScreenDialog.Instance.Display();

        newsService.GetNewsInfos(2);
    }

    public void DisplayRegular(List<NewsInfo> newsInfos)
    {
        this.newsInfos = newsInfos;

        ScreenDialog.Instance.Hide();

        if (this.newsInfos.Count == 0)
            return;

        FillList(newsInfos);

        displayedRegular = 1;

        UpdateDisplay(0);
    }

    public void UpdateDisplay(int currentIndex)
    {
        if (txtContent != null)
            txtContent.TextValue = newsInfos[currentIndex].News.Content;

        for (int i = 0; i < indicators.Length; i++)
        {
            MPImage indicatorImage = indicators[i].GetComponent<MPImage>();

            if (indicatorImage != null)
                indicatorImage.color = (i == currentIndex) ? colorOn : colorOff;
        }
    }

    private void CreateIndicators(int count)
    {
        foreach (Transform child in indicatorParent)
            Destroy(child.gameObject);

        indicators = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            GameObject indicator = Instantiate(indicatorPrefab, indicatorParent);
            indicators[i] = indicator;
        }
    }
}

