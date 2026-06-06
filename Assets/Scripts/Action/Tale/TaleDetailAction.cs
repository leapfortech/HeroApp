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

public class TaleDetailAction : MonoBehaviour
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
    Text txtPlace = null;
    [SerializeField]
    Text txtSummary = null;
    [SerializeField]
    Text txtDescription = null;

    [Space, Title("Images")]
    [SerializeField]
    GameObject goEmptyImages = null;
    [SerializeField]
    GameObject goImages = null;

    [Space, Title("Comments")]
    [SerializeField]
    GameObject goEmptyComments = null;
    //[SerializeField]
    //GameObject goComments = null;

    [Title("Contents")]
    [SerializeField]
    int charsPerLine = 40;
    [SerializeField]
    int lineHeight = 15;
    [SerializeField]
    float contentPadding = 40f;
    [Space, SerializeField]
    RectTransform[] contents = null;

    [Title("Values")]
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    //[SerializeField]
    //ValueList vllCity = null;

    [Title("Page")]
    [SerializeField]
    Page pagDetail;

    [Title("Event")]
    [SerializeField]
    ImagesEvent onImagesDisplay = null;
    [SerializeField]
    UnityLongsEvent onDisplayed = null;

    TaleService taleService;

    private void Awake()
    {
        taleService = GetComponent<TaleService>();
    }

    public void Display(long postId)
    {
        ScreenDialog.Instance.Display();
        taleService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);
    }

    public void ApplyFull(TaleFull taleFull)
    {
        // Post
        txtAlias.TextValue = $"@{taleFull.AppUserAlias}";
        txtTitle.TextValue = String.IsNullOrWhiteSpace(taleFull.Title) ? "Noticia" : taleFull.Title;

        String country = taleFull.PostCountryId == -1 ? "" : vllCountry.FindRecordCellString(taleFull.PostCountryId, "Name");
        String state = taleFull.PostStateId == -1 ? "" : vllState.FindRecordCellString(taleFull.PostStateId, "Name");
        txtPlace.TextValue = country + (!String.IsNullOrWhiteSpace(country) && !String.IsNullOrWhiteSpace(state) ? ", " : "") + state;

        txtDateTime.TextValue = taleFull.PublicationDateTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        if (txtSummary != null)
            txtSummary.TextValue = String.IsNullOrWhiteSpace(taleFull.Summary) ? "-" : taleFull.Summary;

        txtDescription.TextValue = String.IsNullOrWhiteSpace(taleFull.Description) ? "-" : taleFull.Description;

        // Images
        goEmptyImages.SetActive(taleFull.Images.Length == 0);
        goImages.SetActive(taleFull.Images.Length != 0);

        List<Sprite> images = new List<Sprite>();
        for (int i = 0; i < taleFull.Images.Length; i++)
            images.Add(taleFull.Images[i].CreateSprite($"TaleImage_{i}"));
        onImagesDisplay.Invoke(images);
        onDisplayed.Invoke(new long[2] { taleFull.PostId, taleFull.Id });

        // Comments
        goEmptyComments.SetActive(true);

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