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
using Leap.UI.Extensions;

using Sirenix.OdinInspector;

public class NewsDetailAction : MonoBehaviour
{
    [Serializable]
    public class ImagesEvent : UnityEvent<List<Sprite>> { }

    [Space, Title("Details")]
    [SerializeField]
    Image imgThumbnail = null;
    [SerializeField]
    Text txtAlias = null;
    //[SerializeField]
    //Text txtDateTime = null;
    [SerializeField]
    Text txtTitle = null;
    [SerializeField]
    Text txtSummary = null;
    [SerializeField]
    Text txtDescription = null;

    [SerializeField]
    Text txtNewsType = null;
    //[SerializeField]
    //Text txtPlace = null;
    [SerializeField]
    Text txtSource = null;
    [SerializeField]
    Text txtNewsDateTime = null;

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

    [Title("Actions")]
    [SerializeField]
    Button btnUpdate = null;
    [SerializeField]
    Button btnLink = null;
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

    [Title("Values")]
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    //[SerializeField]
    //ValueList vllCity = null;
    [SerializeField]
    ValueList vllNewsType = null;

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

    NewsService newsService;
    PostService postService;

    long postId = -1;
    String url = null;
    float contentInitialHeight;

    private void Awake()
    {
        newsService = GetComponent<NewsService>();
        postService = GetComponent<PostService>();
    }

    private void Start()
    {
        btnLink?.AddAction(OpenLink);

        RectTransform content = txtDescription.transform.parent.GetComponent<RectTransform>();
        contentInitialHeight = content.sizeDelta.y - txtDescription.TextHeight;
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
        postId = newsFull.PostId;

        btnLink.gameObject.SetActive(newsFull.LinkFulls != null && newsFull.LinkFulls.Count > 0);

        if (newsFull.LinkFulls != null && newsFull.LinkFulls.Count > 0)
            url = newsFull.LinkFulls[0].Url;

        // Post
        imgThumbnail.Sprite = newsFull.ThumbnailSprite;

        txtAlias.TextValue = $"@{newsFull.AppUserAlias}";
        txtTitle.TextValue = $"<line-height=70%>{(String.IsNullOrWhiteSpace(newsFull.Title) ? "Noticia" : newsFull.Title)}";
        //txtDateTime.TextValue = newsFull.PublicationDateTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        if (txtSummary != null)
            txtSummary.TextValue = String.IsNullOrWhiteSpace(newsFull.Summary) ? "-" : newsFull.Summary;

        txtDescription.TextValue = String.IsNullOrWhiteSpace(newsFull.Description) ? "-" : newsFull.Description;

        String country = newsFull.PostCountryId == -1 ? "" : vllCountry.FindRecordCellString(newsFull.PostCountryId, "Name");
        String state = newsFull.PostStateId == -1 ? "" : vllState.FindRecordCellString(newsFull.PostStateId, "Name");
        //txtPlace.TextValue = country + (!String.IsNullOrWhiteSpace(country) && !String.IsNullOrWhiteSpace(state) ? ", " : "") + state;
        //txtPlace.TextValue = String.IsNullOrWhiteSpace(newsFull.Place) ? "-" : newsFull.Place;

        txtNewsType.TextValue = newsFull.NewsTypeId == -1 ? "-" : vllNewsType.FindRecordCellString(newsFull.NewsTypeId, "Name");
        txtSource.TextValue = String.IsNullOrWhiteSpace(newsFull.Source) ? "-" : newsFull.Source;
        txtNewsDateTime.TextValue = newsFull.DateTime == null ? "-" : newsFull.DateTime.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        // Images
        goEmptyImages.SetActive(newsFull.ImageSprites.Count == 0);
        goImages.SetActive(newsFull.ImageSprites.Count != 0);

        onImagesDisplay.Invoke(newsFull.ImageSprites);

        // Actions
        SetToggle(tglFavorite, newsFull.Favorite != 0);
        SetToggle(tglLike, newsFull.Like == 5);
        SetToggle(tglDislike, newsFull.Like == 1);
        SetToggle(tglReaction, newsFull.ReactionPhraseId != -1);

        RefreshContents();

        btnUpdate.gameObject.SetActive(newsFull.AppUserId == StateManager.Instance.AppUser.Id);

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

    private void SetToggle(Toggle toggle, bool value)
    {
        if (value)
            toggle.Check();
        else
            toggle.Uncheck();
    }

    private void RefreshContents()
    {
        RectTransform content = txtDescription.transform.parent.GetComponent<RectTransform>();

        content.sizeDelta = new Vector2(content.sizeDelta.x, contentInitialHeight + txtDescription.TextHeight + contentPadding);

        scrollRect.verticalNormalizedPosition = 1f;
    }
}