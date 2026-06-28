using UnityEngine;

using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;
using Leap.UI.Page;

public class PortraitUpdateAction : MonoBehaviour
{
    [Title("Action")]
    [SerializeField]
    Button btnDelete = null;

    [Title("Action")]
    [SerializeField]
    Page PagNext = null;

    AppUserService appUserService = null;
    Texture2D portrait = null;

    private void Awake()
    {
        appUserService = GetComponent<AppUserService>();
    }

    private void Start()
    {
        btnDelete?.AddAction(Delete);
    }

    public void Delete()
    {
        ChoiceDialog.Instance.Warning("Eliminar fotografía", "¿Estás seguro de borrar la fotografía de perfil?", DoDelete, null, "Sí", "No");
    }

    public void DoDelete()
    {
        ScreenDialog.Instance.Display();
        appUserService.DeletePortrait(StateManager.Instance.AppUser.Id);
    }

    public void ApplyDelete()
    {
        StateManager.Instance.Portrait.Destroy();
        StateManager.Instance.Portrait = null;
        ScreenDialog.Instance.Hide();
    }

    public void DoUpdate(Texture2D portrait)
    {
        this.portrait = portrait;
        appUserService.UpdatePortrait(StateManager.Instance.AppUser.Id, portrait.ToStrBase64(ImageType.JPG));
    }

    public void ApplyPortrait()
    {
        StateManager.Instance.Portrait.Destroy();
        StateManager.Instance.Portrait = portrait.CreateSprite("Portrait");
        PageManager.Instance.ChangePage(PagNext);
    }
}
