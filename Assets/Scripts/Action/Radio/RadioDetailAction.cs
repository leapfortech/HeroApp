using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.Core.Tools;
using Leap.Data.Collections;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.UI.Extensions;

using Sirenix.OdinInspector;

public class RadioDetailAction : MonoBehaviour
{
    [Serializable]
    public class ImagesEvent : UnityEvent<List<Sprite>> { }

    [Space, Title("Details")]
    [SerializeField]
    Text txtTitle = null;
    [SerializeField]
    Text txtPlace = null;
    [SerializeField]
    Text txtSummary = null;
    [SerializeField]
    Text txtDescription = null;

    [Title("Images")]
    [SerializeField]
    GameObject goEmptyImages = null;
    [SerializeField]
    GameObject goImages = null;

    [Title("ScrollView")]
    [SerializeField]
    UnityEngine.UI.ScrollRect scrollRect;
    [SerializeField]
    float contentPadding = 160f;

    [Title("List")]
    [SerializeField]
    ListScroller lstRadioType = null;
    [SerializeField]
    ListScroller lstRadioLanguage = null;

    [Title("Values")]
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

    [Title("Actions")]
    [SerializeField]
    Button btnRadio = null;
    [SerializeField]
    Button btnUpdate = null;
    [SerializeField]
    Toggle tglFavorite = null;
    [SerializeField]
    Toggle tglLike = null;
    [SerializeField]
    Toggle tglDislike = null;
    [SerializeField]
    Toggle tglReaction = null;
    [SerializeField]
    ComboAdapter cmbReaction = null;
    [SerializeField]
    ComboAdapter cmbPlaintType = null;

    [Title("Page")]
    [SerializeField]
    Page pagDetail;

    [Title("Events")]
    [SerializeField]
    ImagesEvent onImagesDisplay = null;
    [SerializeField]
    UnityBoolEvent onFavoriteChanged = null;
    [SerializeField]
    UnityBoolEvent onLikeChanged = null;
    [SerializeField]
    UnityBoolEvent onDislikeChanged = null;
    [SerializeField]
    UnityBoolEvent onReactionChanged = null;

    RadioService radioService;
    PostService postService;

    long postId = -1;
    String url = null;
    float contentInitialHeight = 0.0f;

    private void Awake()
    {
        radioService = GetComponent<RadioService>();
        postService = GetComponent<PostService>();
    }

    private void Start()
    {
        btnRadio?.AddAction(OpenRadio);

        RectTransform content = txtDescription.transform.parent.GetComponent<RectTransform>();
        contentInitialHeight = content.sizeDelta.y - txtDescription.TextHeight
                               - lstRadioType.GetComponent<RectTransform>().sizeDelta.y;
                               //- lstRadioLanguage.GetComponent<RectTransform>().sizeDelta.y;
    }

    private void OpenRadio()
    {
        if (String.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out Uri uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            ChoiceDialog.Instance.Error("Enlace no disponible", "La URL de la radio no es válida o no está disponible.");
            return;
        }

        Application.OpenURL(url);
    }

    public void Display(long postId)
    {
        ScreenDialog.Instance.Display();
        radioService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);
    }

    public void ApplyFull(RadioFull radioFull)
    {
        postId = radioFull.PostId;
        url = radioFull.LinkFulls[0].Url;

        // Post
        txtTitle.TextValue = $"<line-height=70%>{(String.IsNullOrWhiteSpace(radioFull.Title) ? "Radio" : radioFull.Title)}";

        String country = radioFull.PostCountryId == -1 ? "" : vllCountry.FindRecordCellString(radioFull.PostCountryId, "Name");
        String state = radioFull.PostStateId == -1 ? "" : vllState.FindRecordCellString(radioFull.PostStateId, "Name");
        txtPlace.TextValue = country + (!String.IsNullOrWhiteSpace(country) && !String.IsNullOrWhiteSpace(state) ? ", " : "") + state;

        if (txtSummary != null)
            txtSummary.TextValue = String.IsNullOrWhiteSpace(radioFull.Summary) ? "-" : radioFull.Summary;
        
        txtDescription.TextValue = String.IsNullOrWhiteSpace(radioFull.Description) ? "-" : radioFull.Description;

        // Radio Type
        lstRadioType.Clear();
        for (int i = 0; i < radioFull.RadioTypeFulls.Count; i++)
        {
            ListScrollerValue value = new ListScrollerValue(lstRadioType.ListItem, true);
            value.SetText(0, vllRadioType.FindRecordCellString(radioFull.RadioTypeFulls[i].RadioTypeId, "Name"));

            lstRadioType.AddValue(value);
        }
        lstRadioType.ApplyValues();

        float lstHeight = radioFull.RadioTypeFulls.Count * lstRadioType.ListItem.GetComponent<RectTransform>().sizeDelta.y;
        lstRadioType.GetComponent<RectTransform>().sizeDelta = new Vector2(lstRadioType.GetComponent<RectTransform>().sizeDelta.x, lstHeight);

        // Radio Language
        lstRadioLanguage.Clear();
        for (int i = 0; i < radioFull.RadioLanguageFulls.Count; i++)
        {
            ListScrollerValue value = new ListScrollerValue(lstRadioLanguage.ListItem, true);
            value.SetText(0, vllRadioLanguage.FindRecordCellString(radioFull.RadioLanguageFulls[i].LanguageId, "Name"));

            lstRadioLanguage.AddValue(value);
        }
        lstRadioLanguage.ApplyValues();

        lstRadioLanguage.GetComponent<RectTransform>().sizeDelta = new Vector2(lstRadioLanguage.GetComponent<RectTransform>().sizeDelta.x, radioFull.RadioLanguageFulls.Count * lstRadioLanguage.ListItem.GetComponent<RectTransform>().sizeDelta.y);

        // Images
        goEmptyImages.SetActive(radioFull.ImageSprites.Count == 0);
        goImages.SetActive(radioFull.ImageSprites.Count != 0);

        onImagesDisplay.Invoke(radioFull.ImageSprites);

        // Actions
        SetToggle(tglFavorite, radioFull.Favorite != 0);
        SetToggle(tglLike, radioFull.Like == 5);
        SetToggle(tglDislike, radioFull.Like == 1);
        SetToggle(tglReaction, radioFull.ReactionPhraseId != -1);

        RefreshContents(lstHeight);

        btnUpdate.gameObject.SetActive(radioFull.AppUserId == StateManager.Instance.AppUser.Id);

        PageManager.Instance.ChangePage(pagDetail);
    }

    // Favorite

    public void ApplyFavorite(bool check)
    {
        Favorite favorite = new Favorite(postId, StateManager.Instance.AppUser.Id);
        if (check)
            postService.RegisterFavorite(favorite);
        else
            postService.DeleteFavorite(favorite);
    }

    public void ApplyDetailFavorite()
    {
        onFavoriteChanged.Invoke(tglFavorite.Checked);
    }

    // Like

    public void ApplyLike(bool check)
    {
        Like like = new Like(postId, StateManager.Instance.AppUser.Id, 5);
        if (check)
        {
            tglDislike.Uncheck();
            postService.UpdateLike(like);
        }
        else
            postService.DeleteLike(like);
    }

    public void ApplyDislike(bool check)
    {
        Like like = new Like(postId, StateManager.Instance.AppUser.Id, 1);
        if (check)
        {
            tglLike.Uncheck();
            postService.UpdateLike(like);
        }
        else
        {
            like.Rank = -1;
            postService.DeleteLike(like);
        }
    }

    public void ApplyDetailLike()
    {
        onLikeChanged.Invoke(tglLike.Checked);
        onDislikeChanged.Invoke(tglDislike.Checked);
    }

    // Reaction

    public void ApplyReaction(bool check)
    {
        tglReaction.Uncheck();

        if (!check)
        {
            postService.DeleteReaction(new Reaction(-1, postId, StateManager.Instance.AppUser.Id));
            return;
        }

        cmbReaction.Combo.Click();
    }

    public void RegisterReaction()
    {
        long reactionPhraseId = cmbReaction.GetSelectedId();

        Reaction reaction = new Reaction(reactionPhraseId, postId, StateManager.Instance.AppUser.Id);
        postService.RegisterReaction(reaction);
        tglReaction.Check();
    }

    public void ApplyDetailReaction()
    {
        onReactionChanged.Invoke(tglReaction.Checked);
    }

    // Plaint

    public void DisplayPlaintTypes()
    {
        cmbPlaintType.Combo.Click();
    }

    public void ApplyPlaint()
    {
        ScreenDialog.Instance.Display();

        long plaintTypeId = cmbPlaintType.GetSelectedId();

        PostPlaint postPlaint = new PostPlaint(plaintTypeId, postId, StateManager.Instance.AppUser.Id);
        postService.RegisterPostPlaint(postPlaint);
    }

    public void PlaintRegistered()
    {
        ChoiceDialog.Instance.Info("Reporte", "Reporte registrado exitosamente.");
    }

    //

    private void SetToggle(Toggle toggle, bool value)
    {
        if (value)
            toggle.Check();
        else
            toggle.Uncheck();
    }

    private void RefreshContents(float lstHeight)
    {
        RectTransform content = txtDescription.transform.parent.GetComponent<RectTransform>();

        content.sizeDelta = new Vector2(content.sizeDelta.x, contentInitialHeight + lstHeight + txtDescription.TextHeight + contentPadding);

        scrollRect.verticalNormalizedPosition = 1f;
    }
}