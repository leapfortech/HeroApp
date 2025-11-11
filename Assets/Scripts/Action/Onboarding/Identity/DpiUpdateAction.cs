using System;
using UnityEngine;

using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.UI.Dialog.Gallery;

using Sirenix.OdinInspector;

public class DpiUpdateAction : MonoBehaviour
{
    [Title("Dpi")]
    [SerializeField]
    Image imgDpi = null;

    [Space]
    [Title("NextPage")]
    [SerializeField]
    Page nextPage = null;

    IdentityService identityService = null;
    OnboardingService onboardingService = null;

    private void Awake()
    {
        identityService = GetComponent<IdentityService>();
        onboardingService = GetComponent<OnboardingService>();
    }

    // Update

    public void UpdateDpiFront(DpiFront dpiFront)
    {
        ScreenDialog.Instance.Display();
        identityService?.UpdateDpiFront(StateManager.Instance.AppUser.Id, imgDpi.Sprite.ToStrBase64(ImageType.JPG) + "|" + dpiFront.Portrait.ToStrBase64(ImageType.JPG));
        onboardingService?.UpdateDpiFront(StateManager.Instance.AppUser.Id, imgDpi.Sprite.ToStrBase64(ImageType.JPG) + "|" + dpiFront.Portrait.ToStrBase64(ImageType.JPG));
    }

    public void UpdateDpiBack(DpiBack _)
    {
        ScreenDialog.Instance.Display();
        identityService?.UpdateDpiBack(StateManager.Instance.AppUser.Id, imgDpi.Sprite.ToStrBase64(ImageType.JPG));
        onboardingService?.UpdateDpiBack(StateManager.Instance.AppUser.Id, imgDpi.Sprite.ToStrBase64(ImageType.JPG));
    }

    // Page

    public void ChangeDpiFrontPage()
    {
        //StateManager.Instance.DpiFront = imgDpi.Sprite;
        //imgDpi.Sprite = null;

        StateManager.Instance.OnboardingStage = 0;

        PageManager.Instance.ChangePage(nextPage);
    }

    public void ChangeDpiBackPage()
    {
        //StateManager.Instance.DpiBack = imgDpi.Sprite;
        //imgDpi.Sprite = null;

        StateManager.Instance.OnboardingStage = 0;

        PageManager.Instance.ChangePage(nextPage);
    }

    // Error

    public void ErrorDpi(String message)
    {
        imgDpi.Sprite = null;
        ChoiceDialog.Instance.Error(message);
    }
}
