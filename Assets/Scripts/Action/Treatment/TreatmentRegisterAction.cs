using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;

using Sirenix.OdinInspector;
using UnityEngine.Events;

public class TreatmentRegisterAction : MonoBehaviour
{

    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmPost = null;
    [SerializeField]
    DataMapper dtmTreatment = null;
    [SerializeField]
    DataMapper dtmDiseaseVLL = null;

    [Space]
    [Title("Images")]
    [SerializeField]
    int maxCount = 4;
    [SerializeField]
    String spriteName = "Treatment";
    [SerializeField]
    ListScroller lstImage = null;
    [SerializeField]
    Text txtEmpty;

    [Title("Action")]
    [SerializeField]
    Button btnAddImage = null;

    [SerializeField]
    Button btnRegister = null;

    [Space]
    [Title("Action")]
    [SerializeField]
    private UnityEvent onRegistered = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    TreatmentService treatmentService = null;
    List<Texture2D> images = new List<Texture2D>();

    private void Awake()
    {
        treatmentService = GetComponent<TreatmentService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(Register);
    }

    public void Clear()
    {
        dtmPost.ClearElements();
        dtmTreatment.ClearElements();
        images.Clear();
        lstImage.Clear();
    }

    public void RefreshImages()
    {
        lstImage.Clear();

        for (int i = 0; i < images.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(1, true);
            scrollerValue.SetSprite(0, images[i].CreateSprite($"{spriteName}_{i}"));
            lstImage.ApplyAddValue(scrollerValue);
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

        Treatment treatment = dtmTreatment.BuildClass<Treatment>();
        List<Disease> diseases = dtmDiseaseVLL.BuildClassList<Disease>();

        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].CreateSprite($"{spriteName}_{i}").ToStrBase64(ImageType.JPG);

        treatmentService.Register(new RegisterTreatmentRequest(new RegisterPostRequest(post, null, null, strImages), treatment, diseases));
    }

    public void ApplyTreatment(long treatmentId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
        onRegistered.Invoke();
    }
}
