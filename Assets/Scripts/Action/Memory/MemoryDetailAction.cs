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

public class MemoryDetailAction : MonoBehaviour
{
    [Serializable]
    public class ImagesEvent : UnityEvent<List<Sprite>> { }
    [Serializable]
    public class MemoryFullEvent : UnityEvent<MemoryFull> { }

    [Space, Title("Details")]
    //[SerializeField]
    //Image imgThumbnail = null;
    //[SerializeField]
    //Text txtAlias = null;
    //[SerializeField]
    //Text txtDateTime = null;
    [SerializeField]
    Text txtTitle = null;
    [SerializeField]
    Text txtSummary = null;
    [SerializeField]
    Text txtDescription = null;

    [SerializeField]
    Text txtMemoryType = null;
    [SerializeField]
    Text txtPlace = null;
    [SerializeField]
    Text txtDateTime = null;
    [SerializeField]
    Text txtLocation = null;

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

    [Title("Values")]
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    //[SerializeField]
    //ValueList vllCity = null;
    [SerializeField]
    ValueList vllMemoryType = null;

    [Title("Actions")]
    [SerializeField]
    Button btnUpdate = null;
    [SerializeField]
    Toggle tglFavorite = null;
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
    MemoryFullEvent onApplyUpdate = null;

    MemoryService memoryService;
    PostService postService;

    long postId = -1;
    float contentInitialHeight = 0.0f;
    MemoryFull memoryFull = null;

    private void Awake()
    {
        memoryService = GetComponent<MemoryService>();
        postService = GetComponent<PostService>();
    }

    private void Start()
    {
        RectTransform content = txtDescription.transform.parent.GetComponent<RectTransform>();
        contentInitialHeight = content.sizeDelta.y - txtDescription.TextHeight
                               - txtLocation.TextHeight;
    }

    public void Display(long postId)
    {
        ScreenDialog.Instance.Display();
        memoryService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);
    }

    public void ApplyFull(MemoryFull memoryFull)
    {
        this.memoryFull = memoryFull;

        postId = memoryFull.PostId;

        // Post
        //imgThumbnail.Sprite = memoryFull.ThumbnailSprite;

        //txtAlias.TextValue = $"@{memoryFull.AppUserAlias}";
        txtTitle.TextValue = $"<line-height=70%>{(String.IsNullOrWhiteSpace(memoryFull.Title) ? "Evento" : memoryFull.Title)}";
        //txtDateTime.TextValue = memoryFull.PublicationDateTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        if (txtSummary != null)
            txtSummary.TextValue = String.IsNullOrWhiteSpace(memoryFull.Summary) ? "-" : memoryFull.Summary;

        txtDescription.TextValue = String.IsNullOrWhiteSpace(memoryFull.Description) ? "-" : memoryFull.Description;

        // Memory
        txtMemoryType.TextValue = memoryFull.MemoryTypeId == -1 ? "-" : vllMemoryType.FindRecordCellString(memoryFull.MemoryTypeId, "Name");

        String country = memoryFull.PostCountryId == -1 ? "" : vllCountry.FindRecordCellString(memoryFull.PostCountryId, "Name");
        String state = memoryFull.PostStateId == -1 ? "" : vllState.FindRecordCellString(memoryFull.PostStateId, "Name");
        txtPlace.TextValue = country + (!String.IsNullOrWhiteSpace(country) && !String.IsNullOrWhiteSpace(state) ? ", " : "") + state;

        txtDateTime.TextValue = memoryFull.DateTime == null ? "-" : memoryFull.DateTime.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
        txtLocation.TextValue = String.IsNullOrWhiteSpace(memoryFull.Location) ? "-" : memoryFull.Location;

        // Images
        goEmptyImages.SetActive(memoryFull.ImageSprites.Count == 0);
        goImages.SetActive(memoryFull.ImageSprites.Count != 0);

        onImagesDisplay.Invoke(memoryFull.ImageSprites);

        // Actions
        SetToggle(tglFavorite, memoryFull.Favorite != 0);

        RefreshContents();

        btnUpdate.gameObject.SetActive(memoryFull.AppUserId == StateManager.Instance.AppUser.Id);

        PageManager.Instance.ChangePage(pagDetail);
    }

    public void ApplyUpdate()
    {
        onApplyUpdate.Invoke(memoryFull);
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

    private void RefreshContents()
    {
        RectTransform content = txtDescription.transform.parent.GetComponent<RectTransform>();

        float txtHeights = txtDescription.TextHeight + txtLocation.TextHeight;

        content.sizeDelta = new Vector2(content.sizeDelta.x, contentInitialHeight + txtHeights + contentPadding);

        scrollRect.verticalNormalizedPosition = 1f;
    }
}