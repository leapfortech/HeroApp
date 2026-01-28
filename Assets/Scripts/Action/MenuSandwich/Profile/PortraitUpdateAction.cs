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

    public void UpdatePortrait()
    {
        ScreenDialog.Instance.Display();
        identityService.UpdatePortrait(StateManager.Instance.AppUser.Id, imgPortrait.Sprite.ToStrBase64(ImageType.JPG));
    }

    // Page

    public void ApplyPortrait()
    {
        StateManager.Instance.Portrait = imgPortrait.Sprite;
        imgPortrait.Sprite = null;

        PageManager.Instance.ChangePage(nextPage);
    }
}
