using System;
using UnityEngine;

using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;

public class PortraitUpdateAction : MonoBehaviour
{
    [Title("Portrait")]
    [SerializeField]
    Image imgPortrait = null;

    [Space]
    [Title("NextPage")]
    [SerializeField]
    Page nextPage = null;

    IdentityService identityService = null;

    private void Awake()
    {
        identityService = GetComponent<IdentityService>();
    }

    // Update

    public bool UpdatePortrait()
    {
        ScreenDialog.Instance.Display();
        identityService?.UpdatePortrait(StateManager.Instance.AppUser.Id, imgPortrait.Sprite.ToStrBase64(ImageType.JPG));

        return false;
    }

    // Page

    public void ChangePortraitPage()
    {
        StateManager.Instance.Portrait = imgPortrait.Sprite;
        imgPortrait.Sprite = null;

        PageManager.Instance.ChangePage(nextPage);
    }

    // Error

    public void ErrorPortrait(String message)
    {
        imgPortrait.Sprite = null;
        ChoiceDialog.Instance.Error(message);
    }
}
