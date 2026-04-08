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

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Space]
    [Title("NextPage")]
    [SerializeField]
    Page pagNext = null;

    [Title("Message")]
    [SerializeField, TextArea(2, 4)]
    String updatedMessage = "La información fue guardada exitosamente.";

    AppUserService appUserService = null;

    private void Awake()
    {
        appUserService = GetComponent<AppUserService>();
    }

    private void Start()
    {
        btnUpdate?.AddAction(DoUpdate);
    }

    public void Clear()
    {
        imgPortrait.Clear();
    }

    public void DoUpdate()
    {
        if (!PortraitChanged())
        {
            ChoiceDialog.Instance.Info("Sin cambios", "No se detectaron cambios en la imagen.");
            return;
        }

        ScreenDialog.Instance.Display();
        appUserService.UpdatePortrait(StateManager.Instance.AppUser.Id, imgPortrait.Sprite.ToStrBase64(ImageType.JPG));
    }

    public void ApplyPortrait()
    {
        StateManager.Instance.Portrait = imgPortrait.Sprite;

        ChoiceDialog.Instance.Info("Información actualizada", updatedMessage, () => PageManager.Instance.ChangePage(pagNext));
    }

    private bool PortraitChanged()
    {
        Sprite current = imgPortrait.Sprite;
        Sprite stored = StateManager.Instance.Portrait;

        if (current == null && stored == null)
            return false;

        if (current == null || stored == null)
            return true;

        return current.texture != stored.texture;
    }
}
