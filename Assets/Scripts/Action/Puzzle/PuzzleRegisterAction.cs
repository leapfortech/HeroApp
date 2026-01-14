using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;

using Sirenix.OdinInspector;

public class PuzzleRegisterAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmPost = null;
    [SerializeField]
    DataMapper dtmPuzzle = null;

    [Space]
    [Title("Images")]
    [SerializeField]
    int maxCount = 4;
    [SerializeField]
    String spriteName = "Puzzle";
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

    PuzzleService puzzleService = null;
    List<Texture2D> images = new List<Texture2D>();

    private void Awake()
    {
        puzzleService = GetComponent<PuzzleService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(Register);
    }

    public void Clear()
    {
        dtmPost.ClearElements();
        dtmPuzzle.ClearElements();
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

        Puzzle puzzle = dtmPuzzle.BuildClass<Puzzle>();

        // RM WIP Fill All Params
        List<PuzzleAnswer> puzzleAnswers = new List<PuzzleAnswer>();

        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].CreateSprite($"{spriteName}_{i}").ToStrBase64(ImageType.JPG);

        puzzleService.Register(new RegisterPuzzleRequest(new RegisterPostRequest(post, null, null, strImages), 
                                                         puzzle, puzzleAnswers));
    }

    public void ApplyPuzzle(long puzzleId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
