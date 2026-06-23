using System;

using UnityEngine;

using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Dialog;
using UnityEngine.Networking;

public class ReferredAction : MonoBehaviour
{
    [SerializeField]
    Text txtCode = null;

    [SerializeField]
    Button btnCopyLink = null;
    [SerializeField]
    Button btnShareLink = null;

    private void Start()
    {
        btnCopyLink?.AddAction(CopyLink);
        btnShareLink?.AddAction(ShareLink);
    }

    public void DisplayCode()
    {
        txtCode.TextValue = StateManager.Instance.AppUser.ReferringCode;
    }

    private void CopyLink()
    {
        String referringMessage = AppManager.Instance.GetParamValue("ReferringMessage");
        GUIUtility.systemCopyBuffer = referringMessage;
    }
    
    private void ShareLink()
    {
        String referringMessage = AppManager.Instance.GetParamValue("ReferringMessage");

        referringMessage = referringMessage.Replace("{REFERRING_CODE}", StateManager.Instance.AppUser.ReferringCode);

        String url = "https://wa.me/?text=" + UnityWebRequest.EscapeURL(referringMessage);

        Application.OpenURL(url);
    }
}
