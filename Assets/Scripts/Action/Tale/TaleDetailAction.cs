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

public class TaleDetailAction : MonoBehaviour
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

    [Title("Values")]
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;
    //[SerializeField]
    //ValueList vllCity = null;

    [Title("Actions")]
    [SerializeField]
    Button btnUpdate = null;
    [SerializeField]
    Toggle tglReaction1 = null;
    [SerializeField]
    Toggle tglReaction2 = null;
    [SerializeField]
    Toggle tglReaction3 = null;
    [SerializeField]
    Toggle tglReaction4 = null;
    [SerializeField]
    ComboAdapter cmbPlaintType = null;

    [Title("Page")]
    [SerializeField]
    Page pagDetail;

    [Title("Events")]
    [SerializeField]
    ImagesEvent onImagesDisplay = null;
    [SerializeField]
    UnityBoolEvent onReaction1Changed = null;
    [SerializeField]
    UnityBoolEvent onReaction2Changed = null;
    [SerializeField]
    UnityBoolEvent onReaction3Changed = null;
    [SerializeField]
    UnityBoolEvent onReaction4Changed = null;

    TaleService taleService;
    PostService postService;

    long postId = -1;
    float contentInitialHeight = 0.0f;
    long reactionPhraseId = -1;

    private void Awake()
    {
        taleService = GetComponent<TaleService>();
        postService = GetComponent<PostService>();
    }

    private void Start()
    {
        RectTransform content = txtDescription.transform.parent.GetComponent<RectTransform>();
        contentInitialHeight = content.sizeDelta.y - txtDescription.TextHeight;
    }

    public void Display(long postId)
    {
        ScreenDialog.Instance.Display();
        taleService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);
    }

    public void ApplyFull(TaleFull taleFull)
    {
        postId = taleFull.PostId;

        // Post
        imgThumbnail.Sprite = taleFull.ThumbnailSprite;

        txtAlias.TextValue = $"@{taleFull.AppUserAlias}";
        txtTitle.TextValue = $"<line-height=70%>{(String.IsNullOrWhiteSpace(taleFull.Title) ? "Historia" : taleFull.Title)}";

        String country = taleFull.PostCountryId == -1 ? "" : vllCountry.FindRecordCellString(taleFull.PostCountryId, "Name");
        String state = taleFull.PostStateId == -1 ? "" : vllState.FindRecordCellString(taleFull.PostStateId, "Name");
        txtPlace.TextValue = country + (!String.IsNullOrWhiteSpace(country) && !String.IsNullOrWhiteSpace(state) ? ", " : "") + state;

        //txtDateTime.TextValue = taleFull.PublicationDateTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        if (txtSummary != null)
            txtSummary.TextValue = String.IsNullOrWhiteSpace(taleFull.Summary) ? "-" : taleFull.Summary;

        txtDescription.TextValue = String.IsNullOrWhiteSpace(taleFull.Description) ? "-" : taleFull.Description;

        // Images
        goEmptyImages.SetActive(taleFull.ImageSprites.Count == 0);
        goImages.SetActive(taleFull.ImageSprites.Count != 0);

        onImagesDisplay.Invoke(taleFull.ImageSprites);

        // Actions
        SetToggle(tglReaction1, taleFull.ReactionPhraseId == 1);
        SetToggle(tglReaction2, taleFull.ReactionPhraseId == 2);
        SetToggle(tglReaction3, taleFull.ReactionPhraseId == 3);
        SetToggle(tglReaction4, taleFull.ReactionPhraseId == 4);

        RefreshContents();

        btnUpdate.gameObject.SetActive(taleFull.AppUserId == StateManager.Instance.AppUser.Id);

        PageManager.Instance.ChangePage(pagDetail);
    }

    // Reaction
    public void ApplyReaction1(bool check)
    {
        reactionPhraseId = 1;
        ApplyReaction(check, reactionPhraseId);
    }

    public void ApplyReaction2(bool check)
    {
        reactionPhraseId = 2;
        ApplyReaction(check, reactionPhraseId);
    }

    public void ApplyReaction3(bool check)
    {
        reactionPhraseId = 3;
        ApplyReaction(check, reactionPhraseId);
    }

    public void ApplyReaction4(bool check)
    {
        reactionPhraseId = 4;
        ApplyReaction(check, reactionPhraseId);
    }

    public void ApplyReaction(bool check, long reactionPhraseId)
    {
        postService.DeleteReaction(new Reaction(reactionPhraseId, postId, StateManager.Instance.AppUser.Id));

        if (!check)
            return;

        UncheckOtherReactions(reactionPhraseId);
        postService.RegisterReaction(new Reaction(reactionPhraseId, postId, StateManager.Instance.AppUser.Id));
    }

    public void ApplyDetailReaction()
    {
        switch (reactionPhraseId)
        {
            case 1:
                onReaction1Changed.Invoke(tglReaction1.Checked);
                break;

            case 2:
                onReaction2Changed.Invoke(tglReaction2);
                break;

            case 3:
                onReaction3Changed.Invoke(tglReaction3);
                break;

            case 4:
                onReaction4Changed.Invoke(tglReaction4);
                break;
        }
    }

    private void UncheckOtherReactions(long reactionPhraseId)
    {
        if (reactionPhraseId != 1)
            tglReaction1.Uncheck();

        if (reactionPhraseId != 2)
            tglReaction2.Uncheck();

        if (reactionPhraseId != 3)
            tglReaction3.Uncheck();

        if (reactionPhraseId != 4)
            tglReaction4.Uncheck();
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

        content.sizeDelta = new Vector2(content.sizeDelta.x, contentInitialHeight + txtDescription.TextHeight + contentPadding);

        scrollRect.verticalNormalizedPosition = 1f;
    }
}