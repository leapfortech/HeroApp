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

    [Header("Messages")]
    [SerializeField]
    String ShareSubject = "¡Mira esta App!";

    [SerializeField, TextArea(3, 6)]
    String ShareText = "Te recomiendo utilizar la aplicación de Héroes Migrantes. " +
                       "Recuerda utilizar el siguiente código al momento de tu registro: {0}.";

    [Space, SerializeField]
    String SuccessTitle = "Referencia";
    [SerializeField, TextArea(3, 6)]
    String SuccessMessage = "La información fue compartida.";
    [SerializeField]
    String ErrorTitle = "Referencia";
    [SerializeField, TextArea(3, 6)]
    String ErrorMessage = "La información no fue compartida, intenta de nuevo.";

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
        new NativeShare().SetSubject(ShareSubject)
                         .SetText(String.Format(ShareText, txtCode.TextValue))
                         .SetUrl(appDownloadUrl)
                         .SetCallback(ShareDone)
                         .Share();
    }

    private void ShareDone(NativeShare.ShareResult result, String shareTarget)
    {
        if (result == NativeShare.ShareResult.Shared)
            ChoiceDialog.Instance.Info(SuccessTitle, SuccessMessage);
        else
            ChoiceDialog.Instance.Error(ErrorTitle, ErrorMessage);
    }
}
