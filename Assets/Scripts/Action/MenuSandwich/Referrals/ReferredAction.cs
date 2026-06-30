using System;
using UnityEngine;
using UnityEngine.Networking;

using Leap.UI.Elements;

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
        GUIUtility.systemCopyBuffer = GetMessage();
    }

    private void ShareLink()
    {
        String url = "https://wa.me/?text=" + UnityWebRequest.EscapeURL(GetMessage());
        Application.OpenURL(url);
    }

    private String GetMessage()
    {
        String referringMessage = AppManager.Instance.GetParamValue("ReferringMessage");
        referringMessage = referringMessage.Replace("{REFERRING_CODE}", StateManager.Instance.AppUser.ReferringCode);

        return referringMessage;
    }
}
