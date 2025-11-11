using System;

using UnityEngine;

using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Dialog;

public class ShareAction : MonoBehaviour
{
    [SerializeField]
    Text txtCode = null;
    
    [SerializeField]
    Button btnShare = null;

    private void Start()
    {
        btnShare?.AddAction(Share);
    }

    public void Share()
    {
        Invoke(nameof(DoShare), 0.1f);
    }

    private void DoShare()
    {
        String appDownloadUrl = AppManager.Instance.GetParamValue("AppDownloadUrl");
        new NativeShare().SetSubject("¡Mira esta App!")
                         .SetText("Te recomiendo utilizar la aplicación de Expande para invertir en activos inmobiliarios rentables. Recuerda utilizar el siguiente código al momento de tu registro: " + txtCode.TextValue + ".")
                         .SetUrl(appDownloadUrl)
                         .SetCallback(ShareDone)
                         .Share();
    }

    private void ShareDone(NativeShare.ShareResult result, String shareTarget)
    {
        ChoiceDialog.Instance.Info("Referencia", "La información fue compartida.");
    }
}
