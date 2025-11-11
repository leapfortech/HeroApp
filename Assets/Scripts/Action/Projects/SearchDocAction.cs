using UnityEngine;

using Leap.UI.Elements;

using Sirenix.OdinInspector;
using Leap.UI.Dialog.Gallery;
using Leap.Graphics.Tools;

public class SearchDocAction : MonoBehaviour
{
    [Title("Photo")]
    [SerializeField]
    Button btnDoc = null;

    [Title("Gallery")]
    [SerializeField]
    Vector2Int gallerySize = new Vector2Int(794, 560);

    Texture2D docPhoto = null;

    public void SearchDoc()
    {
        GalleryDialog.Instance.Search(gallerySize, false, ApplyDoc);
    }

    private void ApplyDoc(Texture2D photo)
    {
        docPhoto?.Destroy();
        docPhoto = photo;

        Sprite sprite = docPhoto?.CreateSprite(true);
        @btnDoc.Sprite = sprite;
    }
}
