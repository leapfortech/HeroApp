using System;
using UnityEngine;

using Leap.UI.Elements;

using Sirenix.OdinInspector;

public class ExternalLinkAction : MonoBehaviour
{
    private enum LinkType { Web, Phone, SMS, Email }
    private String[] linkTypeNames = { "URL", "Phone", "Phone", "Address" };

    [SerializeField, Space]
    LinkType linkType = LinkType.Web;

    [Header("Link")]
    [SerializeField, LabelText("@" + nameof(GetLinkTypeName) + "()")]
    String link = null;

    [SerializeField, ShowIf(nameof(linkType), LinkType.Email)]
    String subject = null;

    [SerializeField, TextArea(2, 6), ShowIf("@" + nameof(linkType) + " == LinkType.SMS || " + nameof(linkType) + " == LinkType.Email")]
    String body = null;

    [Space]
    [Header("Action")]
    [SerializeField]
    Button btnLink = null;

    private void Start()
    {
        btnLink?.AddAction(DoLink);
    }

    private String GetLinkTypeName()
    {
        return linkTypeNames[(int)linkType];
    }

    private void DoLink()
    {
        if (String.IsNullOrEmpty(link))
        {
            Debug.LogError(name + " > External Link " + GetLinkTypeName() + " cannot be Empty.");
            return;
        }

        if (link[0] == '^')
            link = AppManager.Instance.GetParamValue(link.Substring(1));

        if (!String.IsNullOrEmpty(subject) && subject[0] == '^')
            subject = AppManager.Instance.GetParamValue(subject.Substring(1));

        if (!String.IsNullOrEmpty(body) && body[0] == '^')
            body = AppManager.Instance.GetParamValue(body.Substring(1));

        switch (linkType)
        {
            case LinkType.Web:
                Application.OpenURL(link);
                break;
            case LinkType.Phone:
                Application.OpenURL("tel://" + link);
                break;
            case LinkType.SMS:
                Application.OpenURL("sms:" + link + "?&body=" + Uri.EscapeDataString(body));
                break;
            case LinkType.Email:
                if (String.IsNullOrEmpty(body))
                    Application.OpenURL("mailto:" + link + "?subject=" + subject);
                else
                    Application.OpenURL("mailto:" + link + "?subject=" + subject + "&body=" + body);
                break;
        }
    }
}
