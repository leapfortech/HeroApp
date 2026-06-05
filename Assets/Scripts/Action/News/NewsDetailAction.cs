using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.Core.Tools;
using Leap.Graphics.Tools;
using Leap.Data.Collections;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;

public class NewsDetailAction : MonoBehaviour
{
    [Serializable]
    public class ImagesEvent : UnityEvent<List<Sprite>> { }

    [Space, Title("Details")]
    [SerializeField]
    Text txtAlias = null;
    [SerializeField]
    Text txtDateTime = null;
    [SerializeField]
    Text txtTitle = null;
    [SerializeField]
    Text txtSummary = null;
    [SerializeField]
    Text txtDescription = null;

    [SerializeField]
    Text txtNewsType = null;
    [SerializeField]
    Text txtPlace = null;
    [SerializeField]
    Text txtSource = null;
    [SerializeField]
    Text txtNewsDateTime = null;

    [Space, Title("Contents")]
    [SerializeField]
    int charsPerLine = 40;
    [SerializeField]
    int lineHeight = 15;
    [SerializeField]
    float contentPadding = 40f;
    [Space, SerializeField]
    RectTransform[] contents = null;

    [Space, Title("Action")]
    [SerializeField]
    Button btnLink = null;

    [Space, Title("Values")]
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    //[SerializeField]
    //ValueList vllCity = null;
    [SerializeField]
    ValueList vllNewsType = null;

    [Space, Title("Event")]
    [SerializeField]
    ImagesEvent onImagesDisplay = null;
    [SerializeField]
    UnityLongsEvent onDisplayed = null;

    [Space, Title("Page")]
    [SerializeField]
    Page pagDetail;

    NewsService newsService;

    String url = null;

    private void Awake()
    {
        newsService = GetComponent<NewsService>();
    }

    private void Start()
    {
        btnLink?.AddAction(OpenLink);
    }

    private void OpenLink()
    {
        if (String.IsNullOrWhiteSpace(url))
        {
            ChoiceDialog.Instance.Info("Link de noticia", "No se registró ninguna fuente externa.");
            return;
        }

        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            ChoiceDialog.Instance.Info("Link de noticia", "La URL no es válida.");
            return;
        }

        Application.OpenURL(url);
    }

    public void Display(long postId)
    {
        ScreenDialog.Instance.Display();
        newsService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);
    }

    public void ApplyFull(NewsFull newsFull)
    {
        url = newsFull.LinkFulls[0].Url;

        // Post
        txtAlias.TextValue = $"@{newsFull.AppUserAlias}";
        txtTitle.TextValue = String.IsNullOrWhiteSpace(newsFull.Title) ? "-" : newsFull.Title;
        txtDateTime.TextValue = newsFull.PublicationDateTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        if (txtSummary != null)
            txtSummary.TextValue = String.IsNullOrWhiteSpace(newsFull.Summary) ? "-" : newsFull.Summary;

        txtDescription.TextValue = String.IsNullOrWhiteSpace(newsFull.Description) ? "-" : newsFull.Description;

        String country = newsFull.PostCountryId == -1 ? "" : vllCountry.FindRecordCellString(newsFull.PostCountryId, "Name");
        String state = newsFull.PostStateId == -1 ? "" : vllState.FindRecordCellString(newsFull.PostStateId, "Name");
        txtPlace.TextValue = country + (!String.IsNullOrWhiteSpace(country) && !String.IsNullOrWhiteSpace(state) ? ", " : "") + state;

        txtNewsType.TextValue = newsFull.NewsTypeId == -1 ? "-" : vllNewsType.FindRecordCellString(newsFull.NewsTypeId, "Name");
        txtPlace.TextValue = String.IsNullOrWhiteSpace(newsFull.Place) ? "-" : newsFull.Place;
        txtSource.TextValue = String.IsNullOrWhiteSpace(newsFull.Source) ? "-" : newsFull.Source;
        txtNewsDateTime.TextValue = newsFull.DateTime == null ? "-" : newsFull.DateTime.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        // Images
        List<Sprite> images = new List<Sprite>();
        for (int i = 0; i < newsFull.Images.Length; i++)
            images.Add(newsFull.Images[i].CreateSprite($"NewsImage_{i}"));
        onImagesDisplay.Invoke(images);
        onDisplayed.Invoke(new long[2] { newsFull.PostId, newsFull.Id });

        RefreshContents();

        PageManager.Instance.ChangePage(pagDetail);
    }

    private void RefreshContents()
    {
        for (int i = 0; i < contents.Length; i++)
        {
            Text txtScroll = contents[i].GetComponentInChildren<Text>();
            int lineCount = Mathf.CeilToInt((float)txtScroll.TextValue.Length / charsPerLine);
            float height = lineCount * lineHeight;

            contents[i].sizeDelta = new Vector2(contents[i].sizeDelta.x, height + contentPadding);
        }
    }
}