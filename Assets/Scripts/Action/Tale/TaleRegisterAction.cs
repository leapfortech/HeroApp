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
        post.CountryId = StateManager.Instance.InterestLocality.CountryId;
        post.StateId = StateManager.Instance.InterestLocality.StateId;

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        taleService.Register(new RegisterTaleRequest(new RegisterPostRequest(post, null, null, strImages)));
    }

    private void RegisterTest()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        if (testCounter == 10)
        {
            testCounter = 0;
            ScreenDialog.Instance.Hide();
            return;
        }

        ScreenDialog.Instance.Display();

        isTest = true;

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();

        if (images.Count == 0)
        {
            ChoiceDialog.Instance.Error("Imágenes", "Debes agregar al menos una imagen.");
            return;
        }

        Post post = dtmPost.BuildClass<Post>();

        post.AppUserId = StateManager.Instance.AppUser.Id;
        post.CountryId = StateManager.Instance.InterestLocality.CountryId;
        post.StateId = StateManager.Instance.InterestLocality.StateId;

        testCounter++;

        post.Title = $"{post.Title} {testCounter}";
        post.Summary = $"{post.Summary} {testCounter}";
        post.Description = $"{post.Description} {testCounter}";

        String[] strImages = null;

        if (images.Count > 0 && testCounter % images.Count == 0)
        {
            int imageIndex = (testCounter / images.Count - 1) % images.Count;

            Sprite selectedImage = images[imageIndex];

            strImages = new String[1];
            strImages[0] = selectedImage.ToStrBase64(ImageType.JPG);
        }

        taleService.Register(new RegisterTaleRequest(new RegisterPostRequest(post, null, null, strImages)));
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
}
