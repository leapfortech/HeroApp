using UnityEngine;

using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Dialog;
using Leap.UI.Page;

using Sirenix.OdinInspector;

public class PortraitUpdateAction : MonoBehaviour
{
    [Title("Action")]
    [SerializeField]
    Button btnDelete = null;

    AppUserService appUserService = null;
    Sprite sptPortrait = null;

    private void Awake()
    {
        appUserService = GetComponent<AppUserService>();
    }

    private void Start()
    {
        btnDelete?.AddAction(DoDelete);
    }

    public void DoDelete()
    {
        ChoiceDialog.Instance.Warning("Eliminar fotografía", "¿Estás seguro de borrar la fotografía de perfil?", () => Delete(), null, "Sí", "No");
    }

    public void Delete()
    {
        ScreenDialog.Instance.Display();
        appUserService.DeletePortrait(StateManager.Instance.AppUser.Id);
    }

    public void ApplyDelete()
    {
        StateManager.Instance.Portrait = null;
        ScreenDialog.Instance.Hide();
    }

    public void DoUpdate(Texture2D portrait)
    {
        ScreenDialog.Instance.Display();

        sptPortrait = portrait.CreateSprite("Portrait");

        appUserService.UpdatePortrait(StateManager.Instance.AppUser.Id, portrait.ToStrBase64(ImageType.JPG));
    }

    public void ApplyPortrait()
    {
        StateManager.Instance.Portrait = sptPortrait;
        ScreenDialog.Instance.Hide();
    }
}
