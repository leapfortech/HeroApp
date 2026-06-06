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

public class RadioDetailAction : MonoBehaviour
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

    [Space, Title("Contents")]
    [SerializeField]
    int charsPerLine = 40;
    [SerializeField]
    int lineHeight = 15;
    [SerializeField]
    float contentPadding = 40f;
    [Space, SerializeField]
    RectTransform[] contents = null;

    [Space, Title("List")]
    [SerializeField]
    ListScroller lstRadioType = null;
    [SerializeField]
    ListScroller lstRadioLanguage = null;

    [Space, Title("Action")]
    [SerializeField]
    Button btnRadio = null;

    [Space, Title("Values")]
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    //[SerializeField]
    //ValueList vllCity = null;
    [SerializeField]
    ValueList vllRadioType = null;
    [SerializeField]
    ValueList vllRadioLanguage = null;

    [Space, Title("Event")]
    [SerializeField]
    ImagesEvent onImagesDisplay = null;
    [SerializeField]
    UnityLongsEvent onDisplayed = null;

    [Space, Title("Page")]
    [SerializeField]
    Page pagDetail;

    RadioService radioService;

    String url = null;

    private void Awake()
    {
        radioService = GetComponent<RadioService>();
    }

    private void Start()
    {
        btnRadio?.AddAction(OpenRadio);
    }

    private void OpenRadio()
    {
        Application.OpenURL(url);
    }

    public void Display(long postId)
    {
        ScreenDialog.Instance.Display();
        radioService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);
    }

    public void ApplyFull(RadioFull radioFull)
    {
        url = radioFull.LinkFulls[0].Url;

        // Post
        txtAlias.TextValue = $"@{radioFull.AppUserAlias}";
        txtTitle.TextValue = String.IsNullOrWhiteSpace(radioFull.Title) ? "Radio" : radioFull.Title;

        String country = radioFull.PostCountryId == -1 ? "" : vllCountry.FindRecordCellString(radioFull.PostCountryId, "Name");
        String state = radioFull.PostStateId == -1 ? "" : vllState.FindRecordCellString(radioFull.PostStateId, "Name");
        txtPlace.TextValue = country + (!String.IsNullOrWhiteSpace(country) && !String.IsNullOrWhiteSpace(state) ? ", " : "") + state;

        txtDateTime.TextValue = radioFull.PublicationDateTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        if (txtSummary != null)
            txtSummary.TextValue = String.IsNullOrWhiteSpace(radioFull.Summary) ? "-" : radioFull.Summary;
        
        txtDescription.TextValue = String.IsNullOrWhiteSpace(radioFull.Description) ? "-" : radioFull.Description;

        // Radio Type
        for (int i = 0; i < radioFull.RadioTypeFulls.Count; i++)
        {
            ListScrollerValue value = new ListScrollerValue(1, true);
            value.SetText(0, vllRadioType.FindRecordCellString(radioFull.RadioTypeFulls[i].RadioTypeId, "Name"));

            lstRadioType.AddValue(value);
        }

        // Radio Language
        for (int i = 0; i < radioFull.RadioLanguageFulls.Count; i++)
        {
            ListScrollerValue value = new ListScrollerValue(1, true);
            value.SetText(0, vllRadioLanguage.FindRecordCellString(radioFull.RadioLanguageFulls[i].LanguageId, "Name"));

            lstRadioLanguage.AddValue(value);
        }

        // Images
        goEmptyImages.SetActive(radioFull.Images.Length == 0);
        goImages.SetActive(radioFull.Images.Length != 0);

        List<Sprite> images = new List<Sprite>();
        for (int i = 0; i < radioFull.Images.Length; i++)
            images.Add(radioFull.Images[i].CreateSprite($"RadioImage_{i}"));
        onImagesDisplay.Invoke(images);
        onDisplayed.Invoke(new long[2] {radioFull.PostId, radioFull.Id});

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