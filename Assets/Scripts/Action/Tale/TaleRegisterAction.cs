using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;

using Sirenix.OdinInspector;

public class TaleRegisterAction : MonoBehaviour
{
    [Title("Test Images")]
    [SerializeField]
    private List<Sprite> testImages;

    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmPost = null;
    [SerializeField]
    DataMapper dtmImagesVLL = null;

    [Title("Action")]
    [SerializeField]
    Button btnRegister = null;
    [SerializeField]
    Button btnRegisterTest = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    TaleService taleService = null;

    private int testCounter = 0;
    private bool isTest = false;

    private void Awake()
    {
        taleService = GetComponent<TaleService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(Register);
        btnRegisterTest?.AddAction(RegisterTest);
    }

    public void Clear()
    {
        dtmPost.ClearElements();
        dtmImagesVLL.ClearElements();
    }

    private void Register()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        isTest = false;

        ScreenDialog.Instance.Display();

        Post post = dtmPost.BuildClass<Post>();
        post.AppUserId = StateManager.Instance.AppUser.Id;

        //RM REVIEW
        post.Title = "Historia";
        post.CountryId = StateManager.Instance.InterestLocality.CountryId;
        post.StateId = StateManager.Instance.InterestLocality.StateId;

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        taleService.Register(new RegisterTaleRequest(post, strImages, null));
    }

    List<String> srcImages = new List<String>();
    private void RegisterTest()
    {
        //if (!ElementHelper.Validate(elementValues))
        //    return;

        if (testCounter == 600)
        {
            testCounter = 0;
            ScreenDialog.Instance.Hide();
            return;
        }

        if (testCounter == 0)
        {
            ScreenDialog.Instance.Display();

            for (int i = 0; i < testImages.Count; i++)
                srcImages.Add(testImages[i].ToStrBase64(ImageType.JPG, 50));
        }

        isTest = true;

        //List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();

        //if (images.Count == 0)
        //{
        //    ChoiceDialog.Instance.Error("Imágenes", "Debes agregar al menos una imagen.");
        //    return;
        //}

        Post post = dtmPost.BuildClass<Post>();

        post.AppUserId = StateManager.Instance.AppUser.Id;
        post.CountryId = interestLocality ? StateManager.Instance.InterestLocality.CountryId : StateManager.Instance.CurrentLocality.CountryId;
        post.StateId = interestLocality ? StateManager.Instance.InterestLocality.StateId : StateManager.Instance.CurrentLocality.StateId;

        String[] strImages = testCounter % srcImages.Count == 3 ? new String[] { srcImages[((testCounter - 3) / srcImages.Count) % srcImages.Count] } : null;

        testCounter++;

        post.Title = $"{post.Title} {testCounter}";
        post.Summary = $"{post.Summary} {testCounter}";
        post.Description = $"{post.Description} {testCounter}";

        taleService.Register(new RegisterTaleRequest(post, strImages));
    }

    public void ApplyTale(long taleId)
    {
        if (!isTest)
        {
            Clear();
            PageManager.Instance.ChangePage(pagNext);
        }
        else
            RegisterTest();
    }

    // Locality

    bool interestLocality = true;

    public void ApplyLocality(bool interestLocality)
    {
        this.interestLocality = interestLocality;
    }
}
