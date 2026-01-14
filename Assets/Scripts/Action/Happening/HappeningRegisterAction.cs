using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;

using Sirenix.OdinInspector;

public class HappeningRegisterAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmPost = null;
    [SerializeField]
    DataMapper dtmHappening = null;

    [Space]
    [Title("Images")]
    [SerializeField]
    int maxCount = 4;
    [SerializeField]
    String spriteName = "Happening";
    [SerializeField]
    ListScroller lstImages = null;
    [SerializeField]
    Text txtEmpty;

    [Title("Action")]
    [SerializeField]
    Button btnAddImage = null;

    [SerializeField]
    Button btnRegister = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    HappeningService happeningService = null;
    List<Texture2D> images = new List<Texture2D>();

    private void Awake()
    {
        happeningService = GetComponent<HappeningService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(Register);
    }

    public void Clear()
    {
        dtmPost.ClearElements();
        dtmHappening.ClearElements();
        images.Clear();
        lstImages.Clear();
    }

    public void RefreshImages()
    {
        lstImages.Clear();

        for (int i = 0; i < images.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(1, true);
            scrollerValue.SetSprite(0, images[i].CreateSprite($"{spriteName}_{i}"));
            lstImages.ApplyAddValue(scrollerValue);
        }

        if (images.Count > 0)
            txtEmpty.gameObject.SetActive(false);
        else
            txtEmpty.gameObject.SetActive(true);

        if (images.Count < maxCount)
            btnAddImage.gameObject.SetActive(true);
        else
            btnAddImage.gameObject.SetActive(false);
    }

    public void AddImage(Texture2D image)
    {
        images.Add(image);
        RefreshImages();
    }

    public void RemoveImage(int idx)
    {
        images.RemoveAt(idx);
        RefreshImages();
    }

    private void Register()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        Post post = dtmPost.BuildClass<Post>();

        post.AppUserId = StateManager.Instance.AppUser.Id;
        post.CountryId = StateManager.Instance.Identity.OriginCountryId;
        post.StateId = StateManager.Instance.Identity.OriginStateId;

        // RM WIP Fill All Params
        Happening happening = dtmHappening.BuildClass<Happening>();

        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].CreateSprite($"{spriteName}_{i}").ToStrBase64(ImageType.JPG);

        happeningService.Register(new RegisterHappeningRequest(new RegisterPostRequest(post, null, null, strImages), happening));
    }

    public void ApplyHappening(long happeningId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
